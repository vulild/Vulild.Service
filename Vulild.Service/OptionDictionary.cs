using Vulild.Service.Attributes;
using Vulild.Service.Exceptions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service
{
    public class OptionDictionary : ConcurrentDictionary<string, Option>
    //where TOption : Option
    {

        public Dictionary<Type /*服务*/, Type /*配置*/> ServiceOptionTypeMap = new Dictionary<Type, Type>();

        /// <summary>
        /// 设置默认服务配置，若有多个配置，则默认使用第一个
        /// </summary>
        /// <param name="key"></param>
        public void SetDefaultOption(string key)
        {
            if (!this.ContainsKey(key))
            {
                throw new OptionNotFoundException();
            }

            if (!this[key].IsDefault)
            {
                this[key].IsDefault = true;
            }
        }

        /// <summary> 
        /// 获取所有配置信息中第一个IsDefault=true的<typeparamref name="Option"></typeparamref>
        /// 若没有IsDefault=true的<typeparamref name="Option"></typeparamref>，则取第一个的<typeparamref name="Option"></typeparamref>
        /// </summary>
        /// <returns></returns>
        public Option GetDefaultOption()
        {
            //var def = this.Values.FirstOrDefault(a => a.IsDefault);
            //if (def == null)
            //{
            //    def = this.Values.FirstOrDefault();
            //}
            //if (def == null)
            //{
            //    throw new OptionNotFoundException();
            //}

            var option = GetOption(this.Values);
            if (option != null)
            {
                return option;
            }
            throw new OptionNotFoundException();
        }

        /// <summary>
        /// 根据服务类型获取配置信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Option GetOption(Type serviceType)
        {

            if (ServiceOptionTypeMap.ContainsKey(serviceType))
            {
                var optionType = ServiceOptionTypeMap[serviceType];

                //匹配类型
                var options = Values.Where(a => a.GetType().Equals(optionType));

                if (options.Any())
                {
                    return GetOption(options);
                }
            }
            List<Option> tempOptions = new List<Option>();
            foreach (var map in ServiceOptionTypeMap)
            {
                if (serviceType.IsAssignableFrom(map.Key))
                {
                    var opTypes = this.Values.Where(a => a.GetType().Equals(map.Value));
                    if (opTypes.Any())
                    {
                        tempOptions.AddRange(opTypes);
                        //return GetOption(opTypes);
                    }
                }
            }

            if (tempOptions.Any())
            {
                return GetOption(tempOptions);
            }

            throw new OptionNotFoundException();
        }

        /// <summary>
        /// 取所有配置中的默认配置，如果没有设置默认配置，则取第一个配置项
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private Option GetOption(IEnumerable<Option> options)
        {
            //取默认配置
            var defaultOptions = options.Where(a => a.IsDefault);
            if (defaultOptions.Any())
            {
                return defaultOptions.FirstOrDefault();
            }
            else
            {
                return options.FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据 <typeparamref name="TOption"/>获取第一个IsDefault=true的配置实例
        /// 若没有IsDefault=true的配置，则返回第一个该类型的配置实例
        /// </summary>
        /// <typeparam name="TOption"></typeparam>
        /// <returns></returns>
        public Option GetDefaultOption<TService>() where TService : IService
        {
            return GetOption(typeof(TService));
        }
    }
}
