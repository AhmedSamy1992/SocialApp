

using DatingAppApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
