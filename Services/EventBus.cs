using LibraryManagement.Events;
using LibraryManagement.EventHandlers;

namespace LibraryManagement.Services
{
    /// <summary>
    /// In-process event bus. Resolves all registered IEventHandler&lt;TEvent&gt;
    /// implementations from DI within a new scope and invokes them sequentially.
    /// </summary>
    public class EventBus
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EventBus> _logger;

        public EventBus(IServiceScopeFactory scopeFactory, ILogger<EventBus> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var eventName = typeof(TEvent).Name;

            using var scope = _scopeFactory.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>().ToList();

            _logger.LogDebug(
                "🚌 [EventBus] Publishing {EventName} to {HandlerCount} handler(s).",
                eventName, handlers.Count);

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(eventData);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "❌ [EventBus] Handler {Handler} failed for event {EventName}.",
                        handler.GetType().Name, eventName);
                    // Continue to next handler — one failure does not stop others
                }
            }
        }
    }
}
