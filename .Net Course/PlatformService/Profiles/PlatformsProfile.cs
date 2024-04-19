
using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            //Her mapper vi vores Dto'er med vores model klasse
            // source -> target
            //Fordi vores Dto'er og model klasses prop hedder det samme
            //skal vi ikke configuere yderligere (det hedder jo AutoMapper >_>)
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishDto>();
        }
    }
}