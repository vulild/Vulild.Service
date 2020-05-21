# Vulild.Service
将第三方服务抽象成服务对象，与业务层解耦，同样适用于微服务中服务变更场景

一、初始化服务信息。

```c#
Vulild.Core.Assmblys.AssmblyUtil.SearchAllAssmbly(ServiceUtil.TypeDeal);
```

二、添加服务配置信息（以kafka为例）

```c#
ServiceUtil.InitService(
    "kafkatest",
    new KafkaServiceOption
    {
        Hosts = new List<string> { "199.199.199.64:9091", "199.199.199.64:9092", "199.199.199.64:9093" },
        GroupId = "vulild"
    });
```

三、调用服务

kafka支持Queue模式的发布订阅。代码如下：

1）生产者

```c#
var produceService = ServiceUtil.GetService<IQueueProducerService>();//获取生产者服务，此处依赖IQueueProducerService，不依赖具体实现。
produceService.SendQueueMessage("vulild", new TestData { Ticks = ticks });
```

2）消费者

```c#
public void ConsumerTest()
{
    var consumerService = ServiceUtil.GetService<IQueueConsumerService>();
    CancellationTokenSource cts = new CancellationTokenSource();
    consumerService.OnMessage += OnMessage;
    consumerService.SubscribeQueue(new List<string> { "vulild" }, cts.Token);
}

static void OnMessage(string topic, string message)
{
    Console.WriteLine(message);
}

```
