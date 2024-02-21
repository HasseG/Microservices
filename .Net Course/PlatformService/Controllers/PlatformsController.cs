using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    //[controller] Tager navnet på klassen og sletter "controller". Route = api/platforms
    [Route("api/[controller]")]
    
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo repo, IMapper mapper)
        {
            _repo = repo; 
            _mapper = mapper;
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
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            //Her mapper vi vores create Dto med platform model
            var platformModel =_mapper.Map<Platform>(platformCreateDto);
            //Opretter platform med vores nye platform model
            _repo.CreatePlatform(platformModel);
            //Kald for at gemme ændringerne
            _repo.SaveChanges();

            //Når man opretter en ressourcer returnere man den med 201, selve ressourcen og et URI til ressourcen
            //!HUSK vi bruger ReadDto for at sende ressourcer ud!
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            //CreateAtRoute returnere 201, et URI til ressourcen (som er vores GetPlatformById) med vores nye
            //ReadDto's id (new {Id = platformReadDto.Id}) og selve ressourcen. Dette er REST best practice.
            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }

}