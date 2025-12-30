using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using My_Store.Application.DTOs.User;
using My_Store.Domain.Entities;

namespace My_Store.Application.Mappings
{
    public  class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDto>();

            CreateMap<RegisterUserDto, User>()
           .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        }

    }
}
