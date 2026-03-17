using AutoMapper;
using BlogFusion_API.Models;
using BlogFusion_API.Models.Dto;
using BlogFusion_API.Repository.Interfaces;
using BlogFusion_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlogFusion_API.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IImageService _imageService;
        private readonly IAIContentService _aiService;
        private readonly IMapper _mapper;

        public BlogController(
            IBlogRepository blogRepo,
            ICommentRepository commentRepo,
            IImageService imageService,
            IAIContentService aiService,
            IMapper mapper)
        {
            _blogRepo = blogRepo;
            _commentRepo = commentRepo;
            _imageService = imageService;
            _aiService = aiService;
            _mapper = mapper;
        }

        // POST /api/blog/add
        // Node.js: blogRouter.post("/add", upload.single('image'), auth, createBlog)
        // Accepts multipart/form-data with fields + an image file
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDTO blogDto, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = { "Image file is required." }
                });

            var imageUrl = await _imageService.UploadImageAsync(image);
            var blog = _mapper.Map<Blog>(blogDto);
            blog.Image = imageUrl;
            blog.CreatedAt = DateTime.UtcNow;
            blog.UpdatedAt = DateTime.UtcNow;
            blog.IsPublished = blogDto.IsPublished ?? false;

            var created = await _blogRepo.CreateBlogAsync(blog);
            return StatusCode(201, new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Result = _mapper.Map<BlogDTO>(created)
            });
        }

        // GET /api/blog/all
        // Node.js: blogRouter.get("/all", getAllBlogs)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogRepo.GetAllPublishedBlogsAsync();
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<IEnumerable<BlogDTO>>(blogs)
            });
        }

        // GET /api/blog/{blogId}
        // Node.js: blogRouter.get("/:blogId", getBlogById)
        [HttpGet("{blogId:int}")]
        public async Task<IActionResult> GetBlogById(int blogId)
        {
            var blog = await _blogRepo.GetBlogByIdAsync(blogId);
            if (blog == null)
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = { "Blog not found." }
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<BlogDTO>(blog)
            });
        }

        // DELETE /api/blog/delete/{id}
        // Node.js: blogRouter.post("/delete", auth, deleteBlogById)
        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var deleted = await _blogRepo.DeleteBlogAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = { "Blog not found." }
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Result = "Blog deleted successfully."
            });
        }

        // PUT /api/blog/toggle-publish/{id}
        // Node.js: blogRouter.post('/toggle-publish', auth, togglePublish)
        [HttpPut("toggle-publish/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var toggled = await _blogRepo.TogglePublishAsync(id);
            if (!toggled)
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = { "Blog not found." }
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Result = "Blog publish status updated."
            });
        }

        // POST /api/blog/add-comment
        // Node.js: blogRouter.post('/add-comment', addComment)
        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });

            var comment = _mapper.Map<Comment>(dto);
            comment.CreatedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            var created = await _commentRepo.AddCommentAsync(comment);
            return StatusCode(201, new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Result = _mapper.Map<CommentDTO>(created)
            });
        }

        // GET /api/blog/{blogId}/comments
        // Node.js: blogRouter.post('/comments', getBlogComments)
        [HttpGet("{blogId:int}/comments")]
        public async Task<IActionResult> GetBlogComments(int blogId)
        {
            var comments = await _commentRepo.GetApprovedCommentsByBlogIdAsync(blogId);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<IEnumerable<CommentDTO>>(comments)
            });
        }

        // POST /api/blog/generate
        // Node.js: blogRouter.post('/generate', auth, generateContent)
        [HttpPost("generate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateContent([FromBody] GenerateContentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse { IsSuccess = false });

            var content = await _aiService.GenerateContentAsync(dto.Prompt);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = content
            });
        }
    }
}