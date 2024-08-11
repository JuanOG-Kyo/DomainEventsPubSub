namespace DomainEventsPubSub;

/// <summary>
/// Interface for the event bus service.
/// </summary>
public interface IDomainEventBus
{
    /// <summary>
    /// Enqueues the event to the request queue.
    /// </summary>
    /// <param name="domainEvent">The <see cref="IDomainEvent"/> subscription event.</param>
    void Enqueue(IDomainEvent domainEvent);

    /// <summary>
    /// Executes all the subscribers based on the enqueued events.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> of an async operation.</returns>
    ValueTask ExecuteAllAsync();
}
