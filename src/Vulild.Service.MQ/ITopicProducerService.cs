using Vulild.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.MQ
{
    public interface ITopicProducerService : IRemoteService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        void SendTopicMessage<T>(string channel, T message);
    }
}
