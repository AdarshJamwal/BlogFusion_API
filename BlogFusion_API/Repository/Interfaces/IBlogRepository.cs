using BlogFusion_API.Models;

namespace BlogFusion_API.Repository.Interfaces
{
    public interface IBlogRepository
    {
        // Public-facing: only published blogs
        Task<IEnumerable<Blog>> GetAllPublishedBlogsAsync();

        // Admin panel: all blogs including drafts
        Task<IEnumerable<Blog>> GetAllBlogsAdminAsync();

        Task<Blog?> GetBlogByIdAsync(int blogId);

        Task<Blog> CreateBlogAsync(Blog blog);

        // Returns false if blog not found
        Task<bool> DeleteBlogAsync(int blogId);

        // Flips IsPublished: true→false or false→true
        Task<bool> TogglePublishAsync(int blogId);

        // For dashboard stats
        Task<int> GetBlogsCountAsync();
        Task<int> GetDraftsCountAsync();
        Task<IEnumerable<Blog>> GetRecentBlogsAsync(int count = 5);
    }
}