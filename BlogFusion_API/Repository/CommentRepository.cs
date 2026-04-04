using BlogFusion_API.Data;
using BlogFusion_API.Models;
using BlogFusion_API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogFusion_API.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        public CommentRepository(ApplicationDbContext db) { _db = db; }

     
        public async Task<IEnumerable<Comment>> GetApprovedCommentsByBlogIdAsync(int blogId) =>
            await _db.Comments
                .Where(c => c.BlogId == blogId && c.IsApproved)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

       
        public async Task<IEnumerable<Comment>> GetAllCommentsWithBlogAsync() =>
            await _db.Comments
                .Include(c => c.Blog)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

     
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

     
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return false;
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return true;
        }

     
        public async Task<bool> ApproveCommentAsync(int commentId)
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return false;
            comment.IsApproved = true;
            comment.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

       
        public async Task<int> GetCommentsCountAsync() =>
            await _db.Comments.CountAsync();
    }
}
