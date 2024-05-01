namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcesser
{
    public EventProcessor(Parameters)
    {
        
    }
    
    public void PorcessEvent(string message)
    {
        throw new NotImplementedException();
    }

}

enum EventType
{
    PlatformPublished,
    Undetermined
}
