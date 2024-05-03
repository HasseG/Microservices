using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;


public class EventProcessor : IEventProcesser
{
    private readonly IServiceScopeFactory _factory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _factory = scopeFactory;
        _mapper = mapper;
        
    }
    
    public void PorcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                addPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published event detected");
                return EventType.PlatformPublished;
            default:
                System.Console.WriteLine("--> Could not determin event type");
                return EventType.Undetermined;
        }
    }

    private void addPlatform(string platformPublishedMessage)
    {
        using(var scope = _factory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetService<ICommandRepo>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDto);

                if(!repo.ExternalPlatformIdExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    Console.WriteLine("--> Platform added!");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-->Could not add platoform to DB {ex.Message}");
            }
        }
    }

}

enum EventType
{
    PlatformPublished,
    Undetermined
}
