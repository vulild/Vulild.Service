using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vulild.Core.FormatConversion;
using Vulild.Service.Attributes;

namespace Vulild.Service.Services
{
    [ServiceOption(Type = typeof(ServiceInitServiceOption))]
    public class ServiceInitFromJsonFile : IServiceInitService
    {
        public string FileName { get; set; }
        public Option Option { get; set; }

        public Dictionary<string, Option> GetOptions()
        {
            Dictionary<string, Option> options = new Dictionary<string, Option>();
            FileInfo fi = new FileInfo(FileName);
            var fs = fi.OpenRead();

            JArray settings = null;

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            settings = (JArray)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(buffer));


            foreach (var setting in settings)
            {
                string typeStr = setting["TypeName"].ToString();
                string key = setting["Key"].ToString();

                object optionObj = JsonConvert.DeserializeObject(setting["Option"].ToString(), typeStr.ToType());
                if (optionObj is Option option)
                {
                    options.Add(key, option);
                }
            }

            fs.Close();
            fs.Dispose();
            return options;
        }
    }
}
