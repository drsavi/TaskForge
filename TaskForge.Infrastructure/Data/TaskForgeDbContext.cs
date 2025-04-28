using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Identity;

namespace TaskForge.Infrastructure.Data
{
    public class TaskForgeDbContext : IdentityDbContext<ApplicationUser>
    {
        public TaskForgeDbContext(DbContextOptions<TaskForgeDbContext> opts) : base(opts) { }

        public DbSet<Project> Projects { get; set; } = null!;
    }
}