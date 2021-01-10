using System;
using Vulild.Service.AssemblyService;
using Vulild.Service.Services;

namespace Vulild.Service.Extension
{
    public static class ServiceUtil
    {
        public static void InitService(string configFile)
        {
            Vulild.Service.ServiceUtil.InitService("ServiceInitService", new ServiceInitServiceOption()
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory + configFile
            });
            Vulild.Service.ServiceUtil.InitService(options =>
            {
                var serviceInit = Vulild.Service.ServiceUtil.GetService<IServiceInitService>();
                var ops = serviceInit.GetOptions();
                if (ops != null)
                {
                    foreach (var op in ops)
                    {
                        options.Add(op.Key, op.Value);
                    }
                }
            });
            var ass = Vulild.Service.ServiceUtil.GetService<IAssemblySearch>();
            ass.TypeDeal += Vulild.Service.ServiceUtil.TypeDeal;
            ass.Search();
        }
    }
}
