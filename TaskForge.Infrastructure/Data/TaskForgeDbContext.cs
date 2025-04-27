using Microsoft.EntityFrameworkCore;
using TaskForge.Domain.Entities;

namespace TaskForge.Infrastructure.Data
{
    public class TaskForgeDbContext : DbContext
    {
        public TaskForgeDbContext(DbContextOptions<TaskForgeDbContext> opts) : base(opts) { }
        public DbSet<Project> Projects { get; set; } = null!;
    }
}