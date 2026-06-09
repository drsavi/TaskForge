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
        public DbSet<TaskItem> TaskItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.OwnerId).IsRequired().HasMaxLength(450);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.HasIndex(p => p.OwnerId);
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(p => p.Tasks)
                    .WithOne()
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).HasMaxLength(2000);
                entity.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
                entity.HasIndex(t => t.ProjectId);
            });
        }
    }
}
