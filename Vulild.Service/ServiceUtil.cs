using Vulild.Service.Attributes;
using Vulild.Service.Exceptions;
using Vulild.Service.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vulild.Core.Assmblys;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Vulild.Core.FormatConversion;

namespace Vulild.Service
{
    public static class ServiceUtil
    {
        private static OptionDictionary _Options = new OptionDictionary();

        private static Dictionary<string /*远程类型*/, string /*本地类型*/> _MessageDataTypeMap = new Dictionary<string, string>();

        private static Dictionary<Type /*服务*/, Type /*配置*/> _ServiceOptionTypeMap = new Dictionary<Type, Type>();

        private static List<TypeRela> _RepoServiceTypes = new List<TypeRela>();

        /// <summary>
        /// 获取<typeparamref name="TService"/>实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService GetService<TService>()
            where TService : IService
        {
            var option = _Options.GetDefaultOption<TService>();

            return (TService)option.Build();
        }

        public static TService GetService<TService>(string key)
        {
            if (_Options.ContainsKey(key))
            {
                var option = _Options[key];
                //根据option获取servicetype
                var serviceType = _ServiceOptionTypeMap.Where(a => a.Value == option.GetType()).FirstOrDefault().Key;

                //是否找到servicetype并且该service是从TService继承
                if (serviceType != null && typeof(TService).IsAssignableFrom(serviceType))
                {
                    return (TService)option.Build();
                }
            }
            throw new OptionNotFoundException();
        }

        public static void InitService(Action<OptionDictionary> options)
        {
            if (_Options == null)
            {
                _Options = new OptionDictionary();
            }
            options(_Options);
            foreach (var option in _Options)
            {
                option.Value.MessageDataTypeMap = _MessageDataTypeMap;
            }
            _Options.ServiceOptionTypeMap = _ServiceOptionTypeMap;
        }

        public static void SearchAssmbly(params Action<Type>[] actions)
        {
            Type baseMessageDataType = typeof(XlCloudMessageData);
            Type baseServiceType = typeof(IService);
            AssmblyUtil.SearchAllAssmbly(
                type =>
                {
                    MateMessageDataTypeMap(type, baseMessageDataType);
                },
                type =>
                {
                    MateServiceOptionTypeMap(type, baseServiceType);
                }, type => { TypeAction(type, actions); });
        }

        static void TypeAction(Type type, params Action<Type>[] actions)
        {
            foreach (var action in actions)
            {
                action?.Invoke(type);
            }
        }

        public static void TypeDeal(Type type)
        {
            Type baseMessageDataType = typeof(XlCloudMessageData);
            Type baseServiceType = typeof(IService);
            MateMessageDataTypeMap(type, baseMessageDataType);
            MateServiceOptionTypeMap(type, baseServiceType);
        }

        /// <summary>
        /// 匹配消息类型映射
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        static void MateMessageDataTypeMap(Type type, Type baseType)
        {
            //消息类型，根据该映射关系反序列化
            if (baseType.IsAssignableFrom(type) && type.IsDefined(typeof(TypeMapAttribute)))
            {
                var attr = type.GetCustomAttribute<TypeMapAttribute>(true);
                if (attr != null)
                {
                    if (!_MessageDataTypeMap.ContainsKey(attr.RemoteKey))
                    {
                        _MessageDataTypeMap.Add(attr.RemoteKey, type.FullName);
                    }
                    else
                    {
                        throw new TypeRepeatException();
                    }
                }
            }
        }


        static void MateServiceOptionTypeMap(Type type, Type baseType)
        {
            //服务和配置的对应关系
            if (baseType.IsAssignableFrom(type) && type.IsDefined(typeof(ServiceOptionAttribute)))
            {
                var attr = type.GetCustomAttribute<ServiceOptionAttribute>(true);
                if (attr != null)
                {
                    if (!_ServiceOptionTypeMap.ContainsKey(type))
                    {
                        _ServiceOptionTypeMap.Add(type, attr.Type);
                    }
                    else
                    {
                        throw new ServiceOptionRepeatException();
                    }
                }
            }
        }

        static void RepoServiceTypes(Type type)
        {

        }

        /// <summary>
        /// 初始化服务配置信息，每次调用初始化方法会添加一个配置
        /// </summary>
        /// <typeparam name="TOption"></typeparam>
        /// <param name="option"></param>
        public static void InitService(string key, Option option)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new OptionKeyException();
            }
            if (_Options.ContainsKey(key))
            {
                throw new OptionKeyException();
            }
            InitService(options =>
            {
                options.TryAdd(key, option);
            });

        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="fileName"></param>
        public static void InitConfig(string fileName="ServiceConfig")
        {
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }

            filePath = Path.Combine(filePath, $"{fileName}.json");
            if (!System.IO.File.Exists(filePath))
            {
                throw new ConfigException("未找到配置文件!");
            }

            var configHelper = new JsonConfigHelper(filePath);
            if (configHelper.jObject == null)
            {
                throw new ConfigException("配置文件初始化异常!"); 
            }

            foreach (var item in configHelper.jObject)
            {
                var obj= item as JObject;
                if(obj == null)
                {
                    throw new ConfigException("配置文件格式异常!");
                }

                string key = obj.SelectToken("Key")?.ToString();
                string type = obj.SelectToken("Type")?.ToString();
                if (string.IsNullOrEmpty(type))
                {
                    throw new ConfigException($"key:{key},配置项类型为空!");
                }

                Type optType = type.ToType();
                if (optType == null)
                {
                    throw new ConfigException($"key:{key}, 配置项类型未找到!");
                }

                string opt = obj.SelectToken("Option")?.ToString();
                var option =  JsonConvert.DeserializeObject(opt, optType) as Option;

                InitService(key, option);
            }
        }
    }

}
