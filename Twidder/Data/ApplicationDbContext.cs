using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Twidder.Models;

namespace Twidder.Data
{
   


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=YourServer;Database=YourDatabase;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
        */


        public DbSet<ApplicationUser> Users { get; internal set; }
        public DbSet<Comment> Comments { get; internal set; }
        public DbSet<Group> Groups { get; internal set; }
        public DbSet<Post> Posts { get; internal set; }

        public DbSet<Friend> Friends { get; internal set; }


        


        protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.HasDefaultSchema("Identity");
    builder.Entity<IdentityUser>(entity =>
    {
        entity.ToTable(name: "User");
    });
    builder.Entity<IdentityRole>(entity =>
    {
        entity.ToTable(name: "Role");
    });
    builder.Entity<IdentityUserRole<string>>(entity =>
    {
        entity.ToTable("UserRoles");
    });
    builder.Entity<IdentityUserClaim<string>>(entity =>
    {
        entity.ToTable("UserClaims");
    });
    builder.Entity<IdentityUserLogin<string>>(entity =>
    {
        entity.ToTable("UserLogins");
    });
    builder.Entity<IdentityRoleClaim<string>>(entity =>
    {
        entity.ToTable("RoleClaims");
    });
    builder.Entity<IdentityUserToken<string>>(entity =>
    {
        entity.ToTable("UserTokens");
    });
}
        
    }

    


}

