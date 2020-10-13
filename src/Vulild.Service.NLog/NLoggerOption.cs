using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vulild.Service.NLogService
{
    public class NLoggerOption : Option
    {
        private Logger _log;

        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string FileName { get; set; } = $"${{basedir}}{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}log{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";

        /// <summary>
        /// 备份路径及名称规则,不带扩展名
        /// </summary>
        public string ArchiveFileName { get; set; } = "${basedir}{Path.DirectorySeparatorChar}Logs{Path.DirectorySeparatorChar}${shortdate}{Path.DirectorySeparatorChar}log.{#####}";

        /// <summary>
        /// 备份文件大小
        /// </summary>
        public int ArchiveAboveSize { get; set; } = 209715200;

        /// <summary>
        /// 对文件档案进行编号的方式
        /// </summary>
        public ArchiveNumberingMode ArchiveNumbering { get; set; } = ArchiveNumberingMode.Rolling;

        /// <summary>
        /// 是否允许使用通过多个进程的方式，将日志信息并行写入文件中。其取值类型为Boolean，默认为true。
        /// 备注：这使得多进程记录日志成为可能。NLog使用一种特别的技术使用文件保持打开状态以备写入。
        /// </summary>
        public bool ConcurrentWrites { get; set; } = false;

        /// <summary>
        /// 滚动日志文件上限数，滚动日志文件数达到上限，新的文件内容会覆盖旧文件内容,nlog中默认为9,程序默认为100
        /// </summary>
        public int MaxArchiveFiles { get; set; } = 100;

        /// <summary>
        /// 日志文件是否压缩
        /// </summary>
        public bool EnableArchiveFileCompression { get; set; } = true;

        /// <summary>
        /// 是否保持日志文件处于打开状态，以代替其在每次日志写事件发生时频繁打开和关闭。其取值类型为Boolean，默认值为false。
        /// 备注：设置此属性为true，有助于提高性能。
        /// </summary>
        public bool KeepFileOpen { get; set; } = true;
        public override IService CreateService()
        {
            if (_log == null)
            {
                var config = new NLog.Config.LoggingConfiguration();

                string archaiveFileName = $"{this.ArchiveFileName}.txt";
                if (EnableArchiveFileCompression)
                {
                    archaiveFileName = $"{this.ArchiveFileName}.zip";
                }

                var logfile = new NLog.Targets.FileTarget("logfile")
                {
                    FileName = FileName,
                    ArchiveAboveSize = ArchiveAboveSize,
                    ArchiveFileName = archaiveFileName,
                    ArchiveNumbering = ArchiveNumbering,
                    ConcurrentWrites = ConcurrentWrites,
                    MaxArchiveFiles = MaxArchiveFiles,
                    EnableArchiveFileCompression = true,
                    KeepFileOpen = KeepFileOpen
                };
                //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                // Rules for mapping loggers to targets            
                //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole, "NLOG");
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile, "", true);

                // Apply config           
                NLog.LogManager.Configuration = config;


                _log = LogManager.GetLogger("");
            }

            return new NLogger(_log);

        }
    }
}
