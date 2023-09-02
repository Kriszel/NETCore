using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class AnimalMappings : Profile
    {
        public AnimalMappings()
        {
            CreateMap<AnimalEntity, AnimalDto>().ReverseMap();
            CreateMap<AnimalEntity, AnimalUpdateDto>().ReverseMap();
            CreateMap<AnimalEntity, AnimalCreateDto>().ReverseMap();
        }
    }
}
