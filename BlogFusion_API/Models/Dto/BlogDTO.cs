using System.ComponentModel.DataAnnotations;

namespace BlogFusion_API.Models.Dto
{
  
    public class CreateBlogDTO
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? SubTitle { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public bool? IsPublished { get; set; }
    }

  
    public class BlogDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? SubTitle { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

   
    public class AddCommentDTO
    {
        [Required] public int BlogId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }


    public class CommentDTO
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

  
    public class CommentWithBlogDTO
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }

        // Joined blog fields
        public string BlogTitle { get; set; } = string.Empty;
        public string? BlogSubTitle { get; set; }
        public string BlogCategory { get; set; } = string.Empty;
        public string BlogImage { get; set; } = string.Empty;
    }

    // Admin dashboard stats
    public class DashboardDTO
    {
        public int TotalBlogs { get; set; }
        public int TotalComments { get; set; }
        public int TotalDrafts { get; set; }
        public List<BlogDTO> RecentBlogs { get; set; } = new();
    }

    // AI content generation
    public class GenerateContentDTO
    {
        [Required]
        public string Prompt { get; set; } = string.Empty;
    }
}
