using DataAccessLogic.DatabaseModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLogic
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Antiques> Antiques { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<SavedList> SavedLists { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base (options)
        {
            Database.Migrate();
        }
    }
}