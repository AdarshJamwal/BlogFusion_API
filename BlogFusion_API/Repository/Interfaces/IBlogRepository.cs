using BlogFusion_API.Models;

namespace BlogFusion_API.Repository.Interfaces
{
    public interface IBlogRepository
    {
  
        Task<IEnumerable<Blog>> GetAllPublishedBlogsAsync();

      
        Task<IEnumerable<Blog>> GetAllBlogsAdminAsync();

        Task<Blog?> GetBlogByIdAsync(int blogId);

        Task<Blog> CreateBlogAsync(Blog blog);

      
        Task<bool> DeleteBlogAsync(int blogId);

       
        Task<bool> TogglePublishAsync(int blogId);

      
        Task<int> GetBlogsCountAsync();
        Task<int> GetDraftsCountAsync();
        Task<IEnumerable<Blog>> GetRecentBlogsAsync(int count = 5);
    }
}
