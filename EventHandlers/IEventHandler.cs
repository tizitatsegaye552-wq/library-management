using LibraryManagement.Events;

namespace LibraryManagement.EventHandlers
{
    /// <summary>
    /// Generic handler contract. Implement this for every
    /// event type you want to react to.
    /// </summary>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent eventData);
    }
}
