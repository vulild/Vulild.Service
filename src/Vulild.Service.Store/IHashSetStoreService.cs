using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Store
{
    public interface IHashSetStoreService : IRemoteService
    {
        /// <summary>
        /// 根据<paramref name="key"/>存储<paramref name="value"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        void SetHashSetValue(string key, string field, object value);

        /// <summary>
        /// 批量存储字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        void SetHashSetValues(string key, Dictionary<string, object> kvs);

        /// <summary>
        /// 根据<paramref name="key"/>获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetHashSetValue(string key, string field);

        /// <summary>
        /// 根据<paramref name="keys"/>批量获取字符串
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        Dictionary<string, string> GetHashSetAllValues(string key);

        /// <summary>
        /// 根据<paramref name="key"/>获取字符串，
        /// 获取到的字符串会自动反序列化为<typeparamref name="T"/>实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        T GetHashSetValue<T>(string key, string field);
    }
}
