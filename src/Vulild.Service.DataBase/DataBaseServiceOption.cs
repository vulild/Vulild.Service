using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vulild.Service.DataBase.Exceptions;
using Vulild.Service.Log;

namespace Vulild.Service.DataBase
{
    public abstract class DataBaseServiceOption : RemoteServiceOption
    {
        public DataBaseServiceOption()
        {
            Task.Run(DbCheck);
        }

        /// <summary>
        /// 数据库地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 数据库监听的端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 要连接的数据库名称
        /// </summary>
        public string DataBase { get; set; }

        /// <summary>
        /// 连接数据库使用的用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 连接数据使用的密码
        /// </summary>
        public string Password { get; set; }

        #region 数据库连接池

        /// <summary>
        /// 一段时间内新增的数据库连接的限制,与DbMaxTimeLimit配合使用
        /// </summary>
        public int DbMaxConnection { get; set; } = 20;

        /// <summary>
        /// 一段时间内新增的数据库连接的限制,与DbMaxConnection配合使用,毫秒
        /// </summary>
        public int DbMaxTimeLimit { get; set; } = 100;

        private readonly object _DbMaxLock = new object();

        /// <summary>
        /// 获取或设置在多少时间后关闭空闲连接池的连接。该值为 0 时表示不使用连接池而直接关闭空闲连接，默认为 5 分钟。
        /// </summary>
        public int DbFreeCloseTimer { get; set; } = 300000;

        /// <summary>
        /// 获取或设置在多少时间后关闭当前连接池的连接。该值为 0 时表示始终不强行关闭数据库连接，默认为 90 秒。
        /// </summary>
        public int DbConnectionCloseTimer { get; set; } = 90000;
        /// <summary>
        /// 当前可用连接
        /// </summary>
        private readonly ConcurrentDictionary<long, IDbConnection> DbFreePool = new ConcurrentDictionary<long, IDbConnection>();

        /// <summary>
        /// 每创建一个连接，在该集合添加一条数据，启动一个线程判断该数据是否过期
        /// </summary>
        private ConcurrentQueue<long> _DbConnectionCache;

        /// <summary>
        /// 每创建一个连接，在该集合添加一条数据，启动一个线程判断该数据是否过期
        /// </summary>
        private ConcurrentQueue<long> DbConnectionCache
        {
            get
            {
                if (_DbConnectionCache == null)
                {
                    _DbConnectionCache = new ConcurrentQueue<long>();
                    Task.Run(DbConnectionCacheOverTime_Check_Thrad);
                }
                return _DbConnectionCache;
            }
        }

        /// <summary>
        /// 创建新连接信号量，每次创建新的连接，会触发该信号量，检测线程开始检测
        /// </summary>
        readonly AutoResetEvent autoResetEvent_DbConnectionCacheOverTime = new AutoResetEvent(true);

        /// <summary>
        /// 检查_DbConnectionCache中的数据库标识是否过期
        /// </summary>
        private void DbConnectionCacheOverTime_Check_Thrad()
        {
            while (true)
            {
                DbConnectionCacheOverTime_Check();
                //最多等待限制时间的1/2，或者获取数据库连接时主动通知执行检查任务
                autoResetEvent_DbConnectionCacheOverTime.WaitOne(this.DbMaxTimeLimit / 2);
            }
        }

        /// <summary>
        /// 检查新创建的数据库标记是否超时，超时则从集合中移除，注意，保证该方法只有一个线程调用，多线程调用会得到错误的结果
        /// </summary>
        private void DbConnectionCacheOverTime_Check()
        {
            try
            {
                while (DbConnectionCache != null && DbConnectionCache.Any())
                {
                    long timeTick = DbConnectionCache.FirstOrDefault();
                    long overTick = DateTime.Now.AddMilliseconds(-this.DbMaxTimeLimit).Ticks;
                    if (timeTick <= overTick)
                    {
                        DbConnectionCache.TryDequeue(out long tick);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 空闲连接池锁对象，防止多线程频繁操作空闲连接池失败
        /// </summary>
        private readonly object _LockDbFreePool = new object();

        /// <summary>
        /// 检查空闲连接池中的连接并关闭超过设定期限的连接。
        /// </summary>
        private void DbCheck()
        {
            while (true)
            {
                FreeDbOverTime_Check();
                Thread.Sleep(1000);
            }
        }

        private void FreeDbOverTime_Check()
        {
            if (this.DbFreeCloseTimer > 0)
            {
                try
                {
                    if (Monitor.TryEnter(_LockDbFreePool, 15000))
                    {
                        try
                        {
                            var overTimes = this.DbFreePool.Where(a => (DateTime.Now - new DateTime(a.Key)).TotalMilliseconds > this.DbFreeCloseTimer).ToList();
                            foreach (var over in overTimes)
                            {
                                if (this.DbFreePool.TryRemove(over.Key, out IDbConnection conn))
                                {
                                    conn.Close();
                                }
                            }
                        }
                        finally
                        {
                            Monitor.Exit(_LockDbFreePool);
                        }
                    }
                }
                catch { }

            }
        }

        #endregion

        /// <summary>
        /// 获取数据库连接，先从连接池中获取，获取失败则创建新的连接
        /// </summary>
        /// <returns></returns>
        public virtual IDbConnection GetDbConnection()
        {
            return GetDbConnection(true);
        }

        /// <summary>
        /// 获取数据库连接，先从连接池中获取，获取失败则创建新的连接
        /// </summary>
        /// <param name="fromDbPool">是否从连接池获取连接</param>
        /// <returns></returns>
        public virtual IDbConnection GetDbConnection(bool fromDbPool)
        {
            IDbConnection conn = null;
            if (fromDbPool)
            {
                conn = GetDbFromPool();
            }

            if (conn == null)
            {
                if (Monitor.TryEnter(_DbMaxLock, 15000))
                {
                    try
                    {
                        autoResetEvent_DbConnectionCacheOverTime.Set();
                        ///一段时间内创建的连接数不能超过一定数量
                        if (this.DbMaxTimeLimit > 0 && this.DbMaxConnection > 0 && this.DbConnectionCache != null && this.DbConnectionCache.Count >= this.DbMaxConnection)
                        {
                            throw new DbConnectionOutOfLimitException($"{this.DbMaxTimeLimit}ms内连续创建连接超过{this.DbConnectionCache.Count}/{this.DbMaxConnection}");
                        }
                        conn = GetRealDb();
                        long key = DateTime.Now.Ticks;
                        this.DbConnectionCache.Enqueue(key);
                    }
                    finally
                    {
                        Monitor.Exit(_DbMaxLock);
                    }
                }
                else
                {
                    throw new DbConnectionNullException("获取数据库连接时锁超时");
                }
            }

            return conn;
        }

        /// <summary>
        /// 从空闲连接池获取连接
        /// </summary>
        /// <returns></returns>
        protected IDbConnection GetDbFromPool()
        {
            IDbConnection conn = null;
            if (DbFreeCloseTimer > 0)
            {
                if (Monitor.TryEnter(this._LockDbFreePool, 15000))
                {
                    try
                    {
                        if (DbFreePool.Any())
                        {
                            var key = DbFreePool.Max(a => a.Key);
                            DbFreePool.TryRemove(key, out conn);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_LockDbFreePool);
                    }
                }
                else
                {
                    var log = ServiceUtil.GetService<ILogService>();
                    log.WriteLog($"获取空闲连接锁失败");
                }
            }
            return conn;
        }

        /// <summary>
        /// 创建新连接
        /// </summary>
        /// <returns></returns>
        protected abstract IDbConnection GetRealDb();

        /// <summary>
        /// 释放连接，尝试放入空闲连接池，放入失败则直接关闭连接
        /// </summary>
        /// <param name="conn"></param>
        protected void FreeDbConnection(IDbConnection conn)
        {
            try
            {
                if (DbFreeCloseTimer > 0)
                {
                    long key = DateTime.Now.Ticks;
                    if (!DbFreePool.TryAdd(key, conn))
                    {
                        conn.Close();
                    }
                }
                else
                {
                    conn.Close();
                }
            }
            catch (Exception)
            {
                conn.Close();
            }
        }

        public override IService CreateService()
        {
            DataBaseService service = GetService();
            service.OnConnectionFree += conn =>
            {
                FreeDbConnection(conn);
            };
            return service;
        }

        protected abstract DataBaseService GetService();
    }
}
