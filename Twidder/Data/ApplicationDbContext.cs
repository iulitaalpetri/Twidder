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

        public DbSet<Post> Posts { get; internal set; }
        public DbSet<Comment> Comments { get; internal set; }

        public DbSet<Group> Groups{ get; internal set; }
    }
}