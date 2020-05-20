using Vulild.Service.Attributes;
using Vulild.Service.Model;
using CSRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static CSRedis.CSRedisClient;
using Vulild.Service.MQ;
using Vulild.Service.Store;

namespace Vulild.Service.Redis
{
    [ServiceOption(Type = typeof(RedisServiceOption))]
    public class RedisService :
        ITopicProducerService, ITopicConsumerService,
        IStringStoreService, IHashSetStoreService,
        IGeo
    {
        private readonly CSRedisClient client;

        public RedisService(CSRedisClient client)
        {
            this.client = client;
        }

        public event MessageQueueReceived OnMessage;
        public event MessageQueueReceivedError OnError;
        public Option Option { get; set; }

        public string GetStringValue(string key)
        {
            return client.Get(key);
        }

        public T GetStringValue<T>(string key)
        {
            return client.Get<T>(key);
        }

        public string[] GetStringValues(IEnumerable<string> keys)
        {
            return client.MGet(keys.ToArray());
        }

        public void SetStringValues(Dictionary<string, object> kvs)
        {
            object[] pams = new object[kvs.Count * 2];
            for (int i = 0; i < kvs.Count; i++)
            {
                pams[i * 2] = kvs.ElementAt(i).Key;
                pams[i * 2 + 1] = kvs.ElementAt(i).Value;
            }
            client.MSet(pams);
        }

        public void SendTopicMessage<T>(string channel, T message)
        {
            client.Publish(channel, JsonConvert.SerializeObject(message));
        }

        public void SetStringValue(string key, object value)
        {
            client.Set(key, value);
        }

        public void SubscribeTopic(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            Tuple<string, Action<SubscribeMessageEventArgs>>[] tuples = new Tuple<string, Action<SubscribeMessageEventArgs>>[channels.Count()];
            for (int i = 0; i < channels.Count(); i++)
            {
                string channel = channels.ElementAt(i);
                tuples[i] =
                    new Tuple<string, Action<SubscribeMessageEventArgs>>(
                        channel,
                        msg =>
                        {
                            DoReceiveMessage(channel, msg.Body);
                        });
            }
            client.Subscribe(tuples.Select(a => (a.Item1, a.Item2)).ToArray());
        }

        protected void DoReceiveMessage(string channel, string message)
        {
            OnMessage?.Invoke(channel, message);
        }

        public void SetHashSetValue(string key, string field, object value)
        {
            client.HSet(key, field, value);
        }

        public void SetHashSetValues(string key, Dictionary<string, object> kvs)
        {
            object[] pams = new object[kvs.Count * 2];
            for (int i = 0; i < kvs.Count; i++)
            {
                pams[i * 2] = kvs.ElementAt(i).Key;
                pams[i * 2 + 1] = kvs.ElementAt(i).Value;
            }
            client.HMSet(key, pams);
        }

        public string GetHashSetValue(string key, string field)
        {
            return client.HGet(key, field);
        }

        public T GetHashSetValue<T>(string key, string field)
        {
            return client.HGet<T>(key, field);
        }

        public Dictionary<string, string> GetHashSetAllValues(string key)
        {
            return client.HGetAll(key);
        }

        public bool GeoAdd(string key, decimal longitude, decimal latitude, object member)
        {
            return client.GeoAdd(key, longitude, latitude, member);
        }

        public long GeoAdd(string key, (decimal longitude, decimal latitude, object member)[] values)
        {
            return client.GeoAdd(key, values);
        }

        public decimal? GeoDist(string key, object member1, object member2)
        {
            return client.GeoDist(key, member1, member2);
        }

        public string[] GeoHash(string key, object[] members)
        {
            return client.GeoHash(key, members);
        }

        public (decimal longitude, decimal latitude)?[] GeoPos(string key, object[] members)
        {
            return client.GeoPos(key, members);
        }

        public string[] GeoRadius(string key, decimal longitude, decimal latitude, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadius(key, longitude, latitude, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public T[] GeoRadius<T>(string key, decimal longitude, decimal latitude, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadius<T>(key, longitude, latitude, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public (string member, decimal dist)[] GeoRadiusWithDist(string key, decimal longitude, decimal latitude, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusWithDist(key, longitude, latitude, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public (T member, decimal dist)[] GeoRadiusWithDist<T>(string key, decimal longitude, decimal latitude, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusWithDist<T>(key, longitude, latitude, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public string[] GeoRadiusByMember(string key, object member, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusByMember(key, member, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public T[] GeoRadiusByMember<T>(string key, object member, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusByMember<T>(key, member, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public (string member, decimal dist)[] GeoRadiusByMemberWithDist(string key, object member, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusByMemberWithDist(key, member, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public (T member, decimal dist)[] GeoRadiusByMemberWithDist<T>(string key, object member, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusByMemberWithDist<T>(key, member, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        public (string member, decimal dist, decimal longitude, decimal latitude)[] GeoRadiusByMemberWithDistAndCoord(string key, object member, decimal radius, long? count = null, bool? desc = null)
        {
            return client.GeoRadiusByMemberWithDistAndCoord(key, member, radius, GeoUnit.m, count, GetOrderByTrans(desc));
        }

        /// <summary>
        /// 根据bool值转换排序枚举
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        GeoOrderBy? GetOrderByTrans(bool? desc)
        {
            GeoOrderBy? orderby = null;
            if (desc.HasValue)
            {
                if (desc.Value)
                {
                    orderby = GeoOrderBy.DESC;
                }
                else
                {
                    orderby = GeoOrderBy.ASC;
                }
            }
            return orderby;
        }
    }
}
