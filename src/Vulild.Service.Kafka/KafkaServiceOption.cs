using Vulild.Service.Attributes;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Kafka
{
    public class KafkaServiceOption : RemoteServiceOption
    {
        public IEnumerable<string> Hosts { get; set; }

        private IProducer<string, string> _Producer;
        public IProducer<string, string> Producer
        {
            get
            {
                if (_Producer == null)
                {
                    var config = new ProducerConfig
                    {
                        BootstrapServers = string.Join(",", this.Hosts),

                    };
                    _Producer = new ProducerBuilder<string, string>(config).Build();
                }
                return _Producer;
            }
        }

        private IConsumer<Ignore, string> _Consumer;

        public IConsumer<Ignore, string> Consumer
        {
            get
            {
                if (_Consumer == null)
                {
                    var config = new ConsumerConfig
                    {
                        GroupId = this.GroupId,
                        BootstrapServers = string.Join(",", this.Hosts),
                        EnableAutoCommit = false
                    };
                    _Consumer = new ConsumerBuilder<Ignore, string>(config)
                        .Build();
                }
                return _Consumer;
            }
        }
        public string GroupId { get; set; }
        public override IService CreateService()
        {
            return new KafkaService()
            {
                Producer = Producer,
                Consumer = Consumer
            };
        }
    }
}
