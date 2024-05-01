using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    //[controller] Tager navnet på klassen og sletter "controller". Route = api/platforms
    [Route("api/[controller]")]
    
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _dataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient dataClient, IMessageBusClient messageBusClient)
        {
            _repo = repo; 
            _mapper = mapper;
            _dataClient = dataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            //For debugging
            Console.WriteLine("--> Getting platforms...");

            var paltformItems = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(paltformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById (int id)
        {
            //For debuging
            Console.WriteLine($"--> Getting platform for id: {id}");

            var platformItem = _repo.GetPlatformById(id);

            if(platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            // Her mapper vi vores create Dto med platform model
            var platformModel =_mapper.Map<Platform>(platformCreateDto);
            // Opretter platform med vores nye platform model
            _repo.CreatePlatform(platformModel);
            // Kald for at gemme ændringerne
            _repo.SaveChanges();

            // Når man opretter en ressourcer returnere man den med 201, selve ressourcen og et URI til ressourcen
            // !HUSK vi bruger ReadDto for at sende ressourcer ud!
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Send sync message
            try
            {
                await _dataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // Send Async message
            try
            {
                var platformPulish = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPulish.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPulish);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send async: {ex.Message}");
            }

            // CreateAtRoute returnere 201, et URI til ressourcen (som er vores GetPlatformById) med vores nye
            // ReadDto's id (new {Id = platformReadDto.Id}) og selve ressourcen. Dette er REST best practice.
            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }

}