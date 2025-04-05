using Microsoft.EntityFrameworkCore;
using MinimalApi.Data.Entities;

namespace MinimalApi.Data.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}