using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingAppApi.DTOs;
using DatingAppApi.Entities;
using DatingAppApi.Helpers;
using DatingAppApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class UserRepository(DataContext dataContext, IMapper mapper) : IUserRepository
    {
        public async Task<MemberDto?> GetMemberByUserNameAsync(string userName)
        {
            return await dataContext.Users
                .Where(u => u.UserName == userName)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            //var query = dataContext.Users.Where(u => u.UserName != userParams.CurrentUserName && u.Gender == userParams.Gender)
            //             .ProjectTo<MemberDto>(mapper.ConfigurationProvider);   //ok also

            var query = dataContext.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUserName);

            if (userParams.Gender != null)
            {
                query = query.Where(u => u.Gender == userParams.Gender);
            }

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await dataContext.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUserNameAsync(string userName)
        {
            return await dataContext.Users
                 .Include(x => x.Photos)
                 .SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await dataContext.Users
                .Include(x => x.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }

        public void Update(AppUser appUser)
        {
            dataContext.Entry(appUser).State = EntityState.Modified;
        }
    }
}
