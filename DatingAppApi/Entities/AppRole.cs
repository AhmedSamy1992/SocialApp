using Microsoft.AspNetCore.Identity;

namespace DatingAppApi.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; } = [];
    }
}
