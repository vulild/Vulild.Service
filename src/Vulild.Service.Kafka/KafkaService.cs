using Vulild.Service.Attributes;
using Vulild.Service.Exceptions;
using Vulild.Service.Model;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vulild.Core.FormatConversion;
using Vulild.Service.MQ;

namespace Vulild.Service.Kafka
{
    [ServiceOption(Type = typeof(KafkaServiceOption))]
    public class KafkaService : ITopicProducerService, ITopicConsumerService
    {
        public event MessageQueueReceived OnMessage;
        public event MessageOriginQueueReceived OnOriginMessage;
        public Option Option { get; set; }

        private KafkaServiceOption _ThisOption
        {
            get
            {
                return (KafkaServiceOption)this.Option;
            }
        }

        public IProducer<string, string> Producer { get; set; }

        public IConsumer<Ignore, string> Consumer { get; set; }

        public event MessageQueueReceivedError OnError;

        public void SendTopicMessage<T>(string channel, T message)
        {
            SendTopicMessage<T>(channel, null, message);
        }

        public void SendTopicMessage<T>(string channel, string sendKey, T message)
        {
            var result = Producer.ProduceAsync(
                        channel,
                        new Message<string, string>
                        {
                            Key = sendKey,
                            Value = JsonConvert.SerializeObject(message)
                        }).Result;
        }

        public void SubscribeTopic(IEnumerable<string> channels, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                Consumer.Subscribe(channels);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = Consumer.Consume(cancellationToken);

                            OnOriginMessage?.Invoke(consumeResult.Topic, consumeResult.Value);
                            DoReciveMessage(consumeResult.Topic, consumeResult.Value);

                            Consumer.Commit(consumeResult);
                        }
                        catch (OperationCanceledException)
                        {
                            //取消线程，异常抛出，跳出循环
                            throw;
                        }
                        catch (Exception ex)
                        {
                            OnError?.Invoke(new MessageReceivedException { ErrorCode = "ConsumerError", ErrorMsg = ex.Message, InnerException = ex });
                        }
                    }
                }
                catch (OperationCanceledException oce)
                {
                    OnError?.Invoke(new MessageReceivedException { ErrorCode = "ConsumerCancel", ErrorMsg = oce.Message, InnerException = oce });
                }
                finally
                {
                    Consumer.Close();
                }
            });

        }

        protected virtual void DoReciveMessage(string channel, string message)
        {
            OnMessage?.Invoke(channel, message);
        }
    }
}
