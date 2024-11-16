using System.Security.Claims;

namespace DatingAppApi.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var userName = user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("can not get username from token");

            return userName;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("Cannot get userId from token"));

            return userId;
        }

    }
}
