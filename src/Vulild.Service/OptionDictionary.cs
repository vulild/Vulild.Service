using Vulild.Service.Attributes;
using Vulild.Service.Exceptions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Vulild.Service
{
    public class OptionDictionary : IDictionary<string, Option>
    //where TOption : Option
    {
        private ConcurrentDictionary<string, Option> _Options = new ConcurrentDictionary<string, Option>();

        public Dictionary<Type /*服务*/, Type /*配置*/> ServiceOptionTypeMap = new Dictionary<Type, Type>();

        public ICollection<string> Keys => _Options.Keys;

        public ICollection<Option> Values => _Options.Values;

        public int Count => _Options.Count;

        public bool IsReadOnly => false;

        public Option this[string key] { get => _Options[key]; set => _Options[key] = value; }

        /// <summary>
        /// 设置默认服务配置，若有多个配置，则默认使用第一个
        /// </summary>
        /// <param name="key"></param>
        public void SetDefaultOption(string key)
        {
            if (!this._Options.ContainsKey(key))
            {
                throw new OptionNotFoundException();
            }

            if (!this._Options[key].IsDefault)
            {
                this._Options[key].IsDefault = true;
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

            var option = GetOption(this._Options.Values);
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
                var options = this._Options.Values.Where(a => a.GetType().Equals(optionType));

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
                    var opTypes = this._Options.Values.Where(a => a.GetType().Equals(map.Value));
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

        public void Add(string key, Option value)
        {
            Type type = value.GetType();
            var attr = type.GetCustomAttribute<ServiceOptionAttribute>();
            if (attr != null)
            {
                this.ServiceOptionTypeMap.Add(attr.Type, type);
            }
            _Options.TryAdd(key, value);
        }

        public bool ContainsKey(string key) => _Options.ContainsKey(key);


        public bool Remove(string key) => _Options.TryRemove(key, out Option option);

        public bool TryGetValue(string key, out Option value) => _Options.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, Option> item) { this.Add(item.Key, item.Value); }

        public void Clear()
        {
            _Options.Clear();
        }

        public bool Contains(KeyValuePair<string, Option> item) => _Options.Contains(item);

        public void CopyTo(KeyValuePair<string, Option>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, Option> item) => _Options.TryRemove(item.Key, out Option option);

        public IEnumerator<KeyValuePair<string, Option>> GetEnumerator() => _Options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Options.GetEnumerator();
    }
}
