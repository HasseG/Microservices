namespace CommandsService.EventProcessing;

public interface IEventProcesser
{
    void PorcessEvent(string message);
}