using BlogFusion_API.Models;

namespace BlogFusion_API.Repository.Interfaces
{
    public interface ICommentRepository
    {
       
        Task<IEnumerable<Comment>> GetApprovedCommentsByBlogIdAsync(int blogId);

      
        Task<IEnumerable<Comment>> GetAllCommentsWithBlogAsync();

        Task<Comment> AddCommentAsync(Comment comment);

       
        Task<bool> DeleteCommentAsync(int commentId);
        Task<bool> ApproveCommentAsync(int commentId);


        Task<int> GetCommentsCountAsync();
    }
}
