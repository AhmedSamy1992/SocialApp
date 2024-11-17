using DatingAppApi.Data;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            int currentUserId = User.GetUserId();

            if (currentUserId == targetUserId) return BadRequest("can not like your self");

            var existingLike = await likesRepository.GetUserLike(currentUserId, targetUserId);

            if(existingLike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = currentUserId,
                    TargetUserId = targetUserId
                };

                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLile(existingLike);
            }

            if (await likesRepository.SaveChanges()) return Ok();

            return BadRequest("failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes(string predicate)
        {
            var users = await likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);   
        }
    }
}
