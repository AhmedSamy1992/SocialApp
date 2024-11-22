﻿using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingAppApi.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();

            registerDto.DateOfBirth = registerDto.DateOfBirth?.Substring(0, 10);
            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            //var user = new AppUser
            //{
            //    UserName = registerDto.Username,
            //    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //    PasswordSalt = hmac.Key
            //};

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = tokenService.CreateToken(user),
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Gender = user.Gender,
                Token = tokenService.CreateToken(user),
                Photourl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
            };
        }

        private async Task<Boolean> UserExists(string username)
        {
            return await context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
        }
    }
}
