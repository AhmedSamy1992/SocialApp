using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;

namespace DatingAppApi.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
        Task<PagedList<MemberDto>> GetUserLikes(LikeParams likeParams);
        void AddLike(UserLike userLike);
        void DeleteLiKe(UserLike userLike);
        Task<bool> SaveChanges();
    }
}
