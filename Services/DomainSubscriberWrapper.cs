using Microsoft.Extensions.DependencyInjection;

namespace DomainEventsPubSub;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEvent">The subscription event.</typeparam>
internal class DomainSubscriberWrapper<TEvent> : DomainSubscriberWrapperBase
    where TEvent : IDomainEvent
{
    /// <inheritdoc/>
    public override async Task ExecuteAsync(IDomainEvent domainEvent, IServiceProvider serviceProvider)
    {
        var subscribers = serviceProvider.GetServices<IDomainSubscribers<TEvent>>();
        foreach (var subscriber in subscribers)
        {
            await subscriber.HandleAsync((TEvent)domainEvent);
        }
    }
}

/// <summary>
/// Base class used to execute all the subscribed handlers of an event.
/// </summary>
internal abstract class DomainSubscriberWrapperBase
{
    /// <summary>
    /// Executes all the subscribed handlers of an event.
    /// </summary>
    /// <param name="domainEvent">Instance of a domain event.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to instansiate subscribers from the IOC Container.</param>
    /// <returns>A <see cref="Task"/> of an async operation.</returns>
    public abstract Task ExecuteAsync(IDomainEvent domainEvent, IServiceProvider serviceProvider);
}
