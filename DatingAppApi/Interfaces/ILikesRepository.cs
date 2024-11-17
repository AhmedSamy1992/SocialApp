using DatingAppApi.DTOs;
using DatingAppApi.Entities;

namespace DatingAppApi.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId);
        void AddLike(UserLike userLike);
        void DeleteLile(UserLike userLike);
        Task<bool> SaveChanges();
    }
}
