using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vulild.Service.Attributes;

namespace Vulild.Service.AssemblyService
{

    [ServiceOption(Type = typeof(AssemblySearchOption))]
    public class AssemblySearch : IAssemblySearch
    {
        public event AssemblySearchAssmblyDeal AssemblyDeal;

        public event AssemblySearchTypeDeal TypeDeal;

        private IEnumerable<string> _Paths;

        public IEnumerable<string> Paths
        {
            get
            {
                if (_Paths == null)
                {
                    _Paths = new List<string>();
                }
                return _Paths;
            }
            set
            {
                _Paths = value;
            }
        }

        private IEnumerable<string> _ExcludePaths;

        public IEnumerable<string> ExcludePaths
        {
            get
            {
                if (_ExcludePaths == null)
                {
                    _ExcludePaths = new List<string>();
                }
                return _ExcludePaths;
            }
            set { _ExcludePaths = value; }
        }

        public Option Option { get; set; }

        public AssemblySearch()
        {
            this._Paths = new string[] { AppDomain.CurrentDomain.BaseDirectory };
        }
        public AssemblySearch(IEnumerable<string> path, IEnumerable<string> excludePath)
        {
            this._Paths = path;
            this._ExcludePaths = excludePath;
        }

        /// <summary>
        /// 搜索多个文件程序集
        /// </summary>
        /// <param name="filenames"></param>
        void SearchFiles(IEnumerable<string> filenames)
        {
            foreach (var filename in filenames)
            {
                if (!IsExcludePath(filename))
                {
                    SearchFile(filename);
                }
            }
        }

        /// <summary>
        /// 搜索单个文件程序集
        /// </summary>
        /// <param name="fileName"></param>
        private void SearchFile(string fileName)
        {
            try
            {
                var assembly = Assembly.LoadFrom(fileName);

                AssemblyDeal?.Invoke(assembly);

                //非IL程序集不能读取
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    TypeDeal?.Invoke(type);
                }
            }
            catch (BadImageFormatException)
            {

                //throw;
            }
        }

        public void Search()
        {
            foreach (var path in _Paths)
            {
                if (Directory.Exists(path))
                {
                    SearchDirectoryAssmbly(path);
                }
                if (File.Exists(path))
                {
                    SearchFile(path);
                }
            }
        }

        /// <summary>
        /// 搜索目录下所有文件及子目录程序集
        /// </summary>
        /// <param name="dirPath"></param>
        public void SearchDirectoryAssmbly(string dirPath)
        {
            string[] dirs = Directory.GetDirectories(dirPath);
            foreach (var dir in dirs)
            {
                if (!IsExcludePath(dir))//排除的文件夹
                {
                    SearchDirectoryAssmbly(Path.Combine(dirPath, dir));
                }

            }

            IEnumerable<string> fis = Directory.GetFiles(dirPath).Where(a => Path.GetExtension(a) == ".dll");

            SearchFiles(fis);
        }

        /// <summary>
        /// 判断路径是否排除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsExcludePath(string path)
        {
            return this.ExcludePaths != null && this.ExcludePaths.Contains(path);
        }
    }
}
