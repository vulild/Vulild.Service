using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Vulild.Service.MQ
{
    public interface IQueueConsumerService : IRemoteService
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
        void SubscribeQueue(IEnumerable<string> channels, CancellationToken cancellationToken);
    }
}
