﻿using Users.Api.Data.Entities;
using Users.Api.Models;

namespace Users.Api.Infrastructure.Helpers
{
    public class Mapper
    {
        public UserDto MapToUserDto(User user)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Photo=user.Photo
            };

            return userDto;
        }
    }
}