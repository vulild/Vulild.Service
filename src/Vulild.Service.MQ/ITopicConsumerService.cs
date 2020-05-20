using Vulild.Service.Exceptions;
using Vulild.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vulild.Service.MQ
{
    public delegate void MessageQueueReceived(string topic, string messge);
    public delegate void MessageOriginQueueReceived(string topic, string messge);

    public delegate void MessageQueueReceivedError(MessageReceivedException e);
    public interface ITopicConsumerService : IRemoteService
    {
        /// <summary>
        /// 封装成XlCloudMessageData的消息回调
        /// </summary>
        event MessageQueueReceived OnMessage;

        /// <summary>
        /// 错误回调
        /// </summary>
        event MessageQueueReceivedError OnError;

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="cancellationToken"></param>
        void SubscribeTopic(IEnumerable<string> channels, CancellationToken cancellationToken);
    }
}
