namespace DomainEventsPubSub;

/// <summary>
/// Interface of a subscriber.
/// </summary>
/// <typeparam name="TEvent">The subscription event.</typeparam>
public interface IDomainSubscribers<in TEvent>
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the Subscription Event.
    /// </summary>
    /// <param name="domainEvent">The <see cref="TEvent"/> subscription event.</param>
    /// <returns>A <see cref="Task"/> of an async operation.</returns>
    Task HandleAsync(TEvent domainEvent);
}
