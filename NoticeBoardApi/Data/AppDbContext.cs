using Microsoft.EntityFrameworkCore;
using NoticeBoardApi.Models;

namespace NoticeBoardApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Notice> Notices => Set<Notice>();


        public DbSet<User> Users => Set<User>();

    }
}
