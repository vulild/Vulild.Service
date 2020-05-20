using CSRedis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Redis
{
    public class RedisServiceOption : RemoteServiceOption
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string MasterHost { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DefaultDatabase { get; set; } = 0;

        /// <summary>
        /// 连接池大小
        /// </summary>
        public int PoolSize { get; set; } = 50;

        /// <summary>
        /// 连接超时设置(毫秒)
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// 发送/接收超时设置(毫秒)
        /// </summary>
        public int SyncTimeout { get; set; } = 10000;

        /// <summary>
        /// 连接池内元素空闲时间(毫秒)，适用连接远程redis-server
        /// </summary>
        public int IdleTimeout { get; set; } = 20000;

        /// <summary>
        /// 预热连接，接收数值如 preheat=5 预热5个连接
        /// </summary>
        public bool Preheat { get; set; } = true;

        /// <summary>
        /// 跟随系统退出事件自动释放
        /// </summary>
        public bool AutoDispose { get; set; } = true;

        /// <summary>
        /// 是否开启加密传输
        /// </summary>
        public bool SSL { get; set; } = false;

        /// <summary>
        /// 是否尝试集群模式，阿里云、腾讯云集群需要设置此选项为 false
        /// </summary>
        public bool TestCluster { get; set; } = true;

        /// <summary>
        /// 异步方法写入缓冲区大小(字节)
        /// </summary>
        public int WriteBuffer { get; set; } = 10240;

        /// <summary>
        /// 执行命令出错，尝试重试的次数
        /// </summary>
        public int TryIt { get; set; } = 0;

        /// <summary>
        /// 连接名称，可以使用 Client List 命令查看
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// key前辍，所有方法都会附带此前辍，csredis.Set(prefix + "key", 111);
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 从机地址
        /// </summary>
        public IEnumerable<string> SlaveHosts { get; set; } = new List<string>();

        private CSRedisClient client;

        public override IService CreateService()
        {
            if (client == null)
            {
                //连接字符串
                string connStr = $@"{MasterHost}{(!string.IsNullOrWhiteSpace(Password) ? $",password={Password}" : "")}{(!string.IsNullOrWhiteSpace(Name) ? $",name={Name}" : "")}{(!string.IsNullOrWhiteSpace(Prefix) ? $",prefix={Prefix}" : "")},defaultDatabase={DefaultDatabase},poolsize={PoolSize},connectTimeout={ConnectTimeout},syncTimeout={SyncTimeout},idleTimeout={IdleTimeout},preheat={Preheat.ToString().ToLower()},autoDispose={AutoDispose.ToString().ToLower()},ssl={SSL.ToString().ToLower()},testcluster={TestCluster.ToString().ToLower()},writerBuffer={WriteBuffer},tryit={TryIt}";

                if (SlaveHosts != null && SlaveHosts.Any())
                {
                    client = new CSRedisClient(connStr, SlaveHosts.ToArray());
                }
                else
                {
                    client = new CSRedisClient(connStr);
                }
            }
            RedisService rs = new RedisService(client);
            return rs;
        }
    }
}
