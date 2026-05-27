namespace LibraryManagement.Events
{
    /// <summary>
    /// Base contract for all domain events.
    /// Every event must expose when it occurred.
    /// </summary>
    public interface IEvent
    {
        DateTime OccurredAt { get; }
    }
}
