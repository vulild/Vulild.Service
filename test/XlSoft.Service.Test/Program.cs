﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vulild.Core.FormatConversion;
using Vulild.Service.ActiveMq;
using Vulild.Service.DataBase;
using Vulild.Service.Kafka;
using Vulild.Service.Log;
using Vulild.Service.Model;
using Vulild.Service.MQ;
using Vulild.Service.MySql;
using Vulild.Service.NLogService;
using Vulild.Service.Redis;
using Vulild.Service.Store;

namespace Vulild.Service.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            InitService();

            //queuemqtest();

            nlogtest();

            //mysqltest();

            Console.ReadLine();
        }

        static void InitService()
        {
            Vulild.Core.Assmblys.AssmblyUtil.SearchAllAssmbly(ServiceUtil.TypeDeal);
            //ServiceUtil.SearchAssmbly();
            //ServiceUtil.InitService(
            //    "kafkatest",
            //    new KafkaServiceOption
            //    {
            //        Hosts = new List<string> { "199.199.199.64:9091", "199.199.199.64:9092", "199.199.199.64:9093" },
            //        GroupId = "vulild"
            //    });
            //ServiceUtil.InitService(
            //    "kafkatest1",
            //    new KafkaServiceOption
            //    {
            //        Hosts = new List<string> { "199.199.199.64:9091", "199.199.199.64:9092", "199.199.199.64:9093" },
            //        GroupId = "vulild"
            //    });

            //ServiceUtil.InitService("redistest",
            //    new RedisServiceOption
            //    {
            //        MasterHost = "127.0.0.1:6379",
            //        Password = "密码",
            //        Name = "用户名",
            //        DefaultDatabase = 1,
            //        //IsDefault = true
            //    });

            //ServiceUtil.InitService("activemqtest",
            //    new ActiveMqServiceOption()
            //    {
            //        Hosts = new List<string> { "tcp://127.0.0.1:61616" },
            //        Password = "用户名",
            //        UserName = "密码"
            //    });

            //ServiceUtil.InitConfig();

            ServiceUtil.InitService("nlogtest",
                new NLoggerOption());

            //var logService = ServiceUtil.GetService<ILogService>();
            //while (true)
            //{
            //    logService.WriteLog($"日志测试{DateTime.Now.Ticks.ToString()}");
            //}
        }

        static void mysqltest()
        {
            ServiceUtil.InitService("mysqltest",
                new MySqlServiceOption
                {
                    Host = "vulild.top",
                    Port = 3306,
                    DataBase = "vulild",
                    UserName = "user",
                    Password = "pwd",
                    DbFreeCloseTimer = 10000
                });

            for (int i = 0; i < 20; i++)
            {
                Task.Run(() =>
                {
                    mysqltestthread();
                });
                Thread.Sleep(100);
            }

        }

        static void mysqltestthread()
        {
            while (true)
            {
                var service = ServiceUtil.GetService<IDataBaseService>("");
                service.ExecuteQuery($"select * from device where deviceid={service.GetParameterName("deviceid")}", new Dictionary<string, object> { { "deviceid", 1 } },
                    dr =>
                  {
                      while (dr.Read())
                      {
                          int deviceid = dr["deviceid"].ToInt();
                          string name = dr["name"].ToString();
                          //Console.WriteLine($"{deviceid}:{name}");
                      }

                  });
                service.ExecuteQuery("select * from device where deviceid=1", null, dr =>
                {
                    while (dr.Read())
                    {
                        int deviceid = dr["deviceid"].ToInt();
                        string name = dr["name"].ToString();
                        //Console.WriteLine($"{deviceid}:{name}");
                    }

                });
                Thread.Sleep(100);
            }
        }


        static void nlogtest()
        {
            Task.Run(() =>
            {
                nlogtestthread();
            });
            Task.Run(() =>
            {
                nlogtestthread();
            });
            Task.Run(() =>
            {
                nlogtestthread();
            });
            Task.Run(() =>
            {
                nlogtestthread();
            });
        }

        static void nlogtestthread()
        {
            while (true)
            {
                var logService = ServiceUtil.GetService<ILogService>();
                logService.WriteLog($"日志测试{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            }
        }

        static void queuemqtest()
        {
            var consumerService = ServiceUtil.GetService<IQueueConsumerService>();
            CancellationTokenSource cts = new CancellationTokenSource();
            consumerService.OnMessage += OnMessage;
            consumerService.SubscribeQueue(new List<string> { "vulild" }, cts.Token);

            Task.Run(() =>
            {
                ProduceQueue();
            });

            Task.Run(() =>
            {
                ProduceQueue();
            });
        }

        static void ProduceQueue()
        {
            var produceService = ServiceUtil.GetService<IQueueProducerService>();
            while (true)
            {
                long ticks = DateTime.Now.Ticks;
                produceService.SendQueueMessage("vulild", new TestData { Ticks = ticks });
                Console.WriteLine($"发送{ticks}");
                Thread.Sleep(200);
            }
        }

        static void topicmqtest()
        {
            var consumerService = ServiceUtil.GetService<ITopicConsumerService>();
            CancellationTokenSource cts = new CancellationTokenSource();
            consumerService.OnMessage += OnMessage;
            consumerService.SubscribeTopic(new List<string> { "vulild" }, cts.Token);

            Task.Run(() =>
            {
                Produce();
            });

            Task.Run(() =>
            {
                Produce();
            });
        }

        static void StringStoreTest()
        {
            var stringStoreService = ServiceUtil.GetService<IStringStoreService>();

            stringStoreService.SetStringValue("vulild", "vulildSet");
            string vulild = stringStoreService.GetStringValue("vulild");

            stringStoreService.SetStringValue("vulild", new TestData
            {
                Ticks = DateTime.Now.Ticks
            });
            TestData td = stringStoreService.GetStringValue<TestData>("vulild");

            stringStoreService.SetStringValues(new Dictionary<string, object>
            {
                { "vulild1",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+1
                    }
                },
                { "vulild2",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+2
                    }
                },
                { "vulild3",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+3
                    }
                },
            });

            string[] vulilds = stringStoreService.GetStringValues(new string[] { "vulild1", "vulild2", "vulild3" });

        }

        static void HashSetStoreTest()
        {
            var hashSetService = ServiceUtil.GetService<IHashSetStoreService>();
            hashSetService.SetHashSetValue("vulildhashset", "vulild", "vulild");
            var get = hashSetService.GetHashSetValue("vulildhashset", "vulild");
            hashSetService.SetHashSetValue("vulildhashset", "vulildT", new TestData
            {
                Ticks = DateTime.Now.Ticks
            });
            var testdata = hashSetService.GetHashSetValue<TestData>("vulildhashset", "vulildT");
            hashSetService.SetHashSetValues("vulildhashset", new Dictionary<string, object>
            {
                { "vulild1",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+1
                    }
                },
                { "vulild2",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+2
                    }
                },
                { "vulild3",
                    new TestData{
                        Ticks=DateTime.Now.Ticks+3
                    }
                },
            });
            var allData = hashSetService.GetHashSetAllValues("vulildhashset");
        }

        static void Produce()
        {
            var produceService = ServiceUtil.GetService<ITopicProducerService>();
            while (true)
            {
                long ticks = DateTime.Now.Ticks;
                produceService.SendTopicMessage("vulild", new TestData { Ticks = ticks });
                Console.WriteLine($"发送{ticks}");
                Thread.Sleep(200);
            }
        }

        static void OnMessage(string topic, string message)
        {
            Console.WriteLine(message);
        }
    }
}
