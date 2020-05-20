using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.MQ
{
    public interface IQueueProducerService : IRemoteService
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        void SendQueueMessage<T>(string channel, T message);
    }
}
