using DatingAppApi.Entities;

namespace DatingAppApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
