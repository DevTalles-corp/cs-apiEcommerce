using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<User, CreateUserDto>().ReverseMap();
    CreateMap<User, UserLoginDto>().ReverseMap();
    CreateMap<User, UserLoginResponseDto>().ReverseMap();
    CreateMap<ApplicationUser, UserDataDto>().ReverseMap();
  }
}
