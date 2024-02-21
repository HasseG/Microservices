using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.Controllers
{
    //[controller] Tager navnet på klassen og sletter "controller". Route = api/Platforms
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

        [HttpGet("{id}")]
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
    }

}