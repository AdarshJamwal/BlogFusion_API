using BlogFusion_API.Models;

namespace BlogFusion_API.Repository.Interfaces
{
    public interface ICommentRepository
    {
        // Public: only approved comments for a specific blog
        Task<IEnumerable<Comment>> GetApprovedCommentsByBlogIdAsync(int blogId);

        // Admin: ALL comments with blog info joined (mirrors SQL JOIN query)
        Task<IEnumerable<Comment>> GetAllCommentsWithBlogAsync();

        Task<Comment> AddCommentAsync(Comment comment);

        // Returns false if comment not found
        Task<bool> DeleteCommentAsync(int commentId);
        Task<bool> ApproveCommentAsync(int commentId);

        // For dashboard stats
        Task<int> GetCommentsCountAsync();
    }
}