using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class LikesRepository(DataContext dataContext, IMapper mapper) : ILikesRepository
    {
        public void AddLike(UserLike userLike)
        {
            dataContext.Likes.Add(userLike);
        }

        public void DeleteLile(UserLike userLike)
        {
            dataContext.Likes.Remove(userLike);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
        {
            return await dataContext.Likes
                                .Where(l => l.SourceUserId == currentUserId)
                                .Select(l => l.TargetUserId)
                                .ToListAsync();
        }

        public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await dataContext.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
        {
            var likes = dataContext.Likes.AsQueryable();

            switch (predicate)
            {
                case "liked":
                    return await likes.Where(l => l.SourceUserId == userId)
                        .Select(l => l.TargetUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

                case "likedBy":
                    return await likes.Where(l => l.TargetUserId == userId)
                        .Select(l => l.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .ToListAsync();

                default:
                    var likesIds = await GetCurrentUserLikeIds(userId);

                    return await dataContext.Likes
                                .Where(l => l.TargetUserId == userId && likesIds.Contains(l.SourceUserId))
                                .Select(l => l.SourceUser)
                                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                                .ToListAsync();

            }
        }

        public async Task<bool> SaveChanges()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
