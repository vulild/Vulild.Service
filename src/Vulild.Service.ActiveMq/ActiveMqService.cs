using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using Vulild.Service.Attributes;
using Vulild.Service.Model;
using Vulild.Service.MQ;

namespace Vulild.Service.ActiveMq
{
    [ServiceOption(Type = typeof(ActiveMqServiceOption))]
    public class ActiveMqService :
        ITopicProducerService, ITopicConsumerService,
        IQueueProducerService, IQueueConsumerService
    {
        public ISession Session { get; set; }
        public event MessageQueueReceived OnMessage;
        public event MessageQueueReceivedError OnError;
        public Option Option { get; set; }

        private ActiveMqServiceOption _ThisOption
        {
            get
            {
                return (ActiveMqServiceOption)Option;
            }
        }

        public void SendQueueMessage<T>(string channel, T message)
        {
            SendMessage(channel, message, 0);
        }

        public void SendTopicMessage<T>(string channle, T message)
        {
            SendMessage(channle, message, 1);
        }

        private void SendMessage<T>(string channel, T message, int qt)
        {
            IDestination destination = null;
            if (qt == 1)
            {
                destination = Session.GetTopic(channel);
            }
            else if (qt == 0)
            {
                destination = Session.GetQueue(channel);
            }
            var producer = Session.CreateProducer(destination);
            var txtMessage = producer.CreateTextMessage(JsonConvert.SerializeObject(message));
            producer.Send(destination, txtMessage);
        }

        public void SubscribeQueue(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            foreach (var channel in channels)
            {
                Subscribe(channel, 0);
            }
        }

        public void SubscribeTopic(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            foreach (var channel in channels)
            {
                Subscribe(channel, 1);
            }

        }

        private void Subscribe(string channel, int qt)
        {
            IDestination destination = null;
            string connStr = $"{channel}?{ (_ThisOption.PrefetchSize != null ? $"consumer.prefetchSize={_ThisOption.PrefetchSize}" : "")}";
            if (qt == 0)
            {
                destination = new ActiveMQQueue(connStr);
            }
            else if (qt == 1)
            {
                destination = new ActiveMQTopic(connStr);
            }
            IMessageConsumer consumer = Session.CreateConsumer(destination);
            //注册监听事件
            consumer.Listener += message =>
            {
                string text = ((ITextMessage)message).Text;
                OnMessage?.Invoke(channel, text);
            };
        }

    }
}
