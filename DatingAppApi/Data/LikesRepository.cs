using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;
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

        public void DeleteLiKe(UserLike userLike)
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

        public async Task<PagedList<MemberDto>> GetUserLikes(LikeParams likeParams)
        {
            var likes = dataContext.Likes.AsQueryable();
            IQueryable<MemberDto> query;

            switch (likeParams.Predicate)
            {
                case "liked":
                    query = likes.Where(l => l.SourceUserId == likeParams.UserId)
                        .Select(l => l.TargetUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;

                case "likedBy":
                    query = likes.Where(l => l.TargetUserId == likeParams.UserId)
                        .Select(l => l.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;

                default:
                    var likesIds = await GetCurrentUserLikeIds(likeParams.UserId);

                    query = dataContext.Likes
                                .Where(l => l.TargetUserId == likeParams.UserId && likesIds.Contains(l.SourceUserId))
                                .Select(l => l.SourceUser)
                                .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                                break;

            }

            return await PagedList<MemberDto>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<bool> SaveChanges()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
