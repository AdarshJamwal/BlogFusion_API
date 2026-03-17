using BlogFusion_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogFusion_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        // Two new tables added alongside existing Identity tables
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required — sets up Identity tables

            // blogs table — maps C# PascalCase props to SQL snake_case columns
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.ToTable("blogs");
                entity.Property(b => b.Id).HasColumnName("id");
                entity.Property(b => b.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
                entity.Property(b => b.SubTitle).HasColumnName("subtitle").HasMaxLength(255);
                entity.Property(b => b.Description).HasColumnName("description").IsRequired();
                entity.Property(b => b.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
                entity.Property(b => b.Image).HasColumnName("image").IsRequired();
                entity.Property(b => b.IsPublished).HasColumnName("ispublished").HasDefaultValue(false);
                entity.Property(b => b.CreatedAt).HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(b => b.UpdatedAt).HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // comments table
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.BlogId).HasColumnName("blog_id").IsRequired();
                entity.Property(c => c.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
                entity.Property(c => c.Content).HasColumnName("content").IsRequired();
                entity.Property(c => c.IsApproved).HasColumnName("is_approved").HasDefaultValue(false);
                entity.Property(c => c.CreatedAt).HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(c => c.UpdatedAt).HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // CASCADE DELETE: deleting a blog removes its comments too
                entity.HasOne(c => c.Blog)
                      .WithMany(b => b.Comments)
                      .HasForeignKey(c => c.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}