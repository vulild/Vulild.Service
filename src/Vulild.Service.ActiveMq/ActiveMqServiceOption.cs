using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Linq;

namespace Vulild.Service.ActiveMq
{
    public class ActiveMqServiceOption : RemoteServiceOption, IDisposable
    {
        public string Host { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int? PrefetchSize { get; set; }

        public string ClientId { get; set; }

        IConnectionFactory _Factory;
        IConnection _Connection;
        ISession _Session;

        protected IConnectionFactory Factory
        {
            get
            {
                if (_Factory == null)
                {
                    string url = $"{Host}";
                    if (!string.IsNullOrWhiteSpace(ClientId))
                    {
                        _Factory = new ConnectionFactory(url, ClientId);
                    }
                    else
                    {
                        _Factory = new ConnectionFactory(url);
                    }
                }
                return _Factory;
            }
        }

        protected IConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
                    {
                        _Connection = Factory.CreateConnection(UserName, Password);
                    }
                    else
                    {
                        _Connection = Factory.CreateConnection();
                    }
                    _Connection.Start();
                }
                return _Connection;
            }
        }

        protected ISession Session
        {
            get
            {
                if (_Session == null)
                {
                    _Session = Connection.CreateSession();
                }
                return _Session;
            }
        }

        public override IService CreateService()
        {
            return new ActiveMqService
            {
                Session = this.Session
            };
        }

        public void Dispose()
        {
            _Session?.Close();
            _Connection?.Close();
        }

        //~ActiveMqServiceOption()
        //{
        //    _Session?.Close();
        //    _Connection?.Close();
        //}
    }
}
