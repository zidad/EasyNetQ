using EasyNetQ.IntegrationTests.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNetQ.IntegrationTests.Scheduler;

[Collection("RabbitMQ")]
public class When_publish_and_subscribe_with_delay_using_dead_letter_exchange : IDisposable
{
    private readonly ServiceProvider serviceProvider;
    private readonly IBus bus;

    public When_publish_and_subscribe_with_delay_using_dead_letter_exchange(RabbitMQFixture fixture)
    {
        var connectionString = $"host={fixture.Host};prefetchCount=1;timeout=-1";
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEasyNetQ(connectionString)
            .UseDelayedExchangeScheduler();

        serviceProvider = serviceCollection.BuildServiceProvider();
        bus = serviceProvider.GetRequiredService<IBus>();
    }

    public void Dispose()
    {
        serviceProvider?.Dispose();
    }

    private const int MessagesCount = 10;

    [Fact]
    public async Task Test()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var subscriptionId = Guid.NewGuid().ToString();
        var messagesSink = new MessagesSink(MessagesCount);
        var messages = MessagesFactories.Create(MessagesCount);

        using (await bus.PubSub.SubscribeAsync<Message>(subscriptionId, messagesSink.Receive, cts.Token))
        {
            await bus.Scheduler.FuturePublishBatchAsync(messages, TimeSpan.FromSeconds(5), "#", cts.Token);

            await messagesSink.WaitAllReceivedAsync(cts.Token);
            messagesSink.ReceivedMessages.Should().Equal(messages);
        }
    }
}
