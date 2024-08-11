using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace DomainEventsPubSub;

internal class DomainEventBus : IDomainEventBus
{
    private static readonly ConcurrentDictionary<Type, DomainSubscriberWrapperBase> _wrappers = new();
    private static readonly Action<ILogger, string, Exception> LogException =
        LoggerMessage.Define<string>(LogLevel.Error, 0, "An exception has occured while executing the request with {Domain Event}");

    private readonly IServiceProvider _serviceProvider;
    private Queue<IDomainEvent>? _eventQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBus"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DomainEventBus(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public void Enqueue(IDomainEvent domainEvent) => (_eventQueue ??= new Queue<IDomainEvent>()).Enqueue(domainEvent);

    /// <inheritdoc/>
    public async ValueTask ExecuteAllAsync()
    {
        if (_eventQueue is null)
        {
            return;
        }

        while (_eventQueue.TryDequeue(out var nextEvent))
        {
            var nextEventType = nextEvent.GetType();
            var wrapper = _wrappers.GetOrAdd(nextEventType, static eventType =>
            {
                var wrapperType = typeof(DomainSubscriberWrapper<>).MakeGenericType(eventType);
                var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for event {eventType}");
                return (DomainSubscriberWrapperBase)wrapper;
            });
            try
            {
                await wrapper.ExecuteAsync(nextEvent, _serviceProvider);
            }
            catch (Exception ex)
            {
                LogException(_serviceProvider.GetRequiredService<ILogger<DomainEventBus>>(), JsonSerializer.Serialize(nextEvent, nextEventType), ex);
                throw;
            }
        }
    }
}

public static class EventBusExtensions
{
    public static IServiceCollection AddDomainEventBus(this IServiceCollection services)
        => services.AddScoped<IDomainEventBus, DomainEventBus>();
}