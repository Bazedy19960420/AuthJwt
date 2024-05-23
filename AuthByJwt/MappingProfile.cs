using AuthByJwt.Models;
using AutoMapper;
using static AuthByJwt.Models.Dtos;

namespace AuthByJwt
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegisterDto, User>();
        }
    }
}
