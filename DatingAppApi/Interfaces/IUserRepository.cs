using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;

namespace DatingAppApi.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser appUser);

        Task<AppUser?> GetUserByIdAsync(int id);

        Task<AppUser?> GetUserByUserNameAsync(string userName);

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

        Task<MemberDto?> GetMemberByUserNameAsync(string userName);

        Task<bool> SaveAllAsync();
    }
}
