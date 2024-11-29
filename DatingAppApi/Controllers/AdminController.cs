using DatingAppApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {
        [Authorize(policy: "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    x.UserName,
                    Role = x.UserRoles.Select(r => r.Role.Name)
                }).ToListAsync();

            return Ok(users);

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string userName, string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return BadRequest("User not found");

            var currentUserRoles = await userManager.GetRolesAsync(user);

            var result  = await userManager.AddToRolesAsync(user, selectedRoles.Except(currentUserRoles));
            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user, currentUserRoles.Except(selectedRoles));
            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await userManager.GetRolesAsync(user));
        }
    }
}
