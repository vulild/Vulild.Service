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

        public IEnumerable<Option> GetOptions()
        {
            List<Option> options = new List<Option>();
            FileInfo fi = new FileInfo(FileName);
            var fs = fi.OpenRead();
            JArray settings = null;
#if NETFRAMEWORK
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

#else
            Span<byte> buffer = new Span<byte>();
            fs.Read(buffer);
#endif

            settings = (JArray)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(buffer));


            foreach (var setting in settings)
            {
                string typeStr = setting["TypeName"].ToString();

                object optionObj = JsonConvert.DeserializeObject(setting["option"].ToString(), typeStr.ToType());
                if (optionObj is Option option)
                {
                    options.Add(option);
                }
            }
            return options;
        }
    }
}
