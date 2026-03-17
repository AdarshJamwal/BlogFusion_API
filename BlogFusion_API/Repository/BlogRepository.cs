using BlogFusion_API.Data;
using BlogFusion_API.Models;
using BlogFusion_API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFusion_API.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _db;

        // ApplicationDbContext is injected via Dependency Injection
        public BlogRepository(ApplicationDbContext db) { _db = db; }

        // SELECT * FROM blogs WHERE ispublished = true ORDER BY created_at DESC
        public async Task<IEnumerable<Blog>> GetAllPublishedBlogsAsync() =>
            await _db.Blogs
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        // SELECT * FROM blogs ORDER BY created_at DESC
        public async Task<IEnumerable<Blog>> GetAllBlogsAdminAsync() =>
            await _db.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        // SELECT * FROM blogs WHERE id = $1
        public async Task<Blog?> GetBlogByIdAsync(int blogId) =>
            await _db.Blogs.FirstOrDefaultAsync(b => b.Id == blogId);

        // INSERT INTO blogs (...) VALUES (...) RETURNING *
        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return blog;
        }

        // DELETE FROM blogs WHERE id = $1
        public async Task<bool> DeleteBlogAsync(int blogId)
        {
            var blog = await _db.Blogs.FindAsync(blogId);
            if (blog == null) return false;
            _db.Blogs.Remove(blog);
            await _db.SaveChangesAsync();
            return true;
        }

        // UPDATE blogs SET ispublished = NOT ispublished WHERE id = $1
        public async Task<bool> TogglePublishAsync(int blogId)
        {
            var blog = await _db.Blogs.FindAsync(blogId);
            if (blog == null) return false;
            blog.IsPublished = !blog.IsPublished;
            blog.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        // SELECT COUNT(*) FROM blogs
        public async Task<int> GetBlogsCountAsync() =>
            await _db.Blogs.CountAsync();

        // SELECT COUNT(*) FROM blogs WHERE ispublished = false
        public async Task<int> GetDraftsCountAsync() =>
            await _db.Blogs.CountAsync(b => !b.IsPublished);

        // SELECT * FROM blogs ORDER BY created_at DESC LIMIT 5
        public async Task<IEnumerable<Blog>> GetRecentBlogsAsync(int count = 5) =>
            await _db.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}