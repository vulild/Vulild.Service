using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service
{
    public class JsonConfigHelper
    {
        public JArray jObject = null;
       
        public JsonConfigHelper(string path)
        {
            jObject = new JArray();
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObject = JArray.Load(reader);
                }
            };
        }
    }
}
