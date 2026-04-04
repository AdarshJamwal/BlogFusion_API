using AutoMapper;
using BlogFusion_API.Models;
using BlogFusion_API.Models.Dto;
using BlogFusion_API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace BlogFusion_API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]   
    public class AdminController : ControllerBase
    {
        private readonly IBlogRepository _blogRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminController(
            IBlogRepository blogRepo,
            ICommentRepository commentRepo,
            IMapper mapper,
            IConfiguration configuration )
        {
            _blogRepo = blogRepo;
            _commentRepo = commentRepo;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult AdminLogin([FromBody] AdminLoginDTO model)
        {
            var adminEmail = _configuration["AdminSettings:Email"];
            var adminPassword = _configuration["AdminSettings:Password"];

            if (model.Email != adminEmail || model.Password != adminPassword)
            {
                return Ok(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = { "Invalid Credentials" }
                });
            }

            var key = _configuration.GetValue<string>("ApiSetting:Secret");

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.Email, model.Email),
                new(System.Security.Claims.ClaimTypes.Role, "Admin")
             };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = new { token = tokenString }
            });
        }









        [HttpGet("blogs")]
        public async Task<IActionResult> GetAllBlogsAdmin()
        {
            var blogs = await _blogRepo.GetAllBlogsAdminAsync();
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<IEnumerable<BlogDTO>>(blogs)
            });
        }

      
        [HttpGet("comments")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepo.GetAllCommentsWithBlogAsync();
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<IEnumerable<CommentWithBlogDTO>>(comments)
            });
        }

   
        [HttpDelete("comment/{id:int}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var deleted = await _commentRepo.DeleteCommentAsync(id);
            if (!deleted)
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = { "Comment not found." }
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = "Comment deleted successfully."
            });
        }

   
        [HttpPut("comment/approve/{id:int}")]
        public async Task<IActionResult> ApproveComment(int id)
        {
            var approved = await _commentRepo.ApproveCommentAsync(id);
            if (!approved)
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = { "Comment not found." }
                });

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = "Comment approved successfully."
            });
        }

     
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            // Run sequentially — EF Core DbContext is NOT thread-safe
            var totalBlogs = await _blogRepo.GetBlogsCountAsync();
            var totalComments = await _commentRepo.GetCommentsCountAsync();
            var totalDrafts = await _blogRepo.GetDraftsCountAsync();
            var recentBlogs = await _blogRepo.GetRecentBlogsAsync(5);

            var dashboard = new DashboardDTO
            {
                TotalBlogs = totalBlogs,
                TotalComments = totalComments,
                TotalDrafts = totalDrafts,
                RecentBlogs = _mapper.Map<List<BlogDTO>>(recentBlogs)
            };

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = dashboard
            });
        }
    }
}
