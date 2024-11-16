using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DatingAppApi.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository userRepository, DataContext dataContext, IMapper mapper, IPhotoService photoService) : BaseApiController
    { 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            //var users = await userRepository.GetUsersAsync();
            //var usersToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
            //return Ok(usersToReturn);

            userParams.CurrentUserName = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var users = await userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users);

            return Ok(users);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetUser(int id)
        {
            var user = await userRepository.GetUserByIdAsync(id);

            if (user == null) return NotFound();

            return mapper.Map<MemberDto>(user);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            //var user = await userRepository.GetUserByUserNameAsync(username);

            //if (user == null) return NotFound();

            //return mapper.Map<MemberDto>(user);

            var member = await userRepository.GetMemberByUserNameAsync(username);
            if (member == null) return NotFound();
            return member;
        }

        [HttpPut]
        public async Task<ActionResult> updateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (username == null) return BadRequest("no username found in token");

            var user = await userRepository.GetUserByUserNameAsync(username);
            if(user == null) return BadRequest("could not find user");

            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);     //not neccessary

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("failed to update user");
        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (username == null) return BadRequest("no username found in token");

            var user = await userRepository.GetUserByUserNameAsync(username);
            if (user == null) return BadRequest("could not find user");

            var result = await photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };
            if(user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);

            if (await userRepository.SaveAllAsync()) //return mapper.Map<PhotoDto>(photo);
                return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));  //create response 201 with location in body

            return BadRequest("Problem in uploaded photo");
        }


        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (username == null) return BadRequest("no username found in token");

            var user = await userRepository.GetUserByUserNameAsync(username);
            if (user == null) return BadRequest("could not find user");

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Can not use this as main photo");

            var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
            if (currentMainPhoto != null) currentMainPhoto.IsMain = false;
            photo.IsMain = true;

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("problem setting main photo");

        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (username == null) return BadRequest("no username found in token");

            var user = await userRepository.GetUserByUserNameAsync(username);
            if (user == null) return BadRequest("could not find user");

            var photo = user.Photos.FirstOrDefault(p =>p.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Can not delete this photo");

            if(photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if(await userRepository.SaveAllAsync())  return Ok(); 

            return BadRequest("problem deleting photo");
        }

        [HttpGet("server-error")]
        public ActionResult<AppUser> getuserbyid() 
        {
            var user = dataContext.Users.Find(-1) ?? throw new Exception("bad thing is happened");
            return user;
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> getuserbyNo()
        {
            var user = dataContext.Users.Find(-1);
            if (user == null) return NotFound();

            return user;
        }
    }
}
