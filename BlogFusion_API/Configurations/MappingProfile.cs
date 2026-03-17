using AutoMapper;
using BlogFusion_API.Models;
using BlogFusion_API.Models.Dto;

namespace BlogFusion_API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Blog → BlogDTO (reading)
            CreateMap<Blog, BlogDTO>();

            // CreateBlogDTO → Blog (creating)
            // Image is ignored — controller sets it after uploading to ImageKit
            CreateMap<CreateBlogDTO, Blog>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            // Comment → CommentDTO
            CreateMap<Comment, CommentDTO>();

            // AddCommentDTO → Comment
            CreateMap<AddCommentDTO, Comment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Blog, opt => opt.Ignore());

            // Comment → CommentWithBlogDTO
            // AutoMapper can't auto-map Blog.Title → BlogTitle (different names),
            // so we tell it explicitly using MapFrom
            CreateMap<Comment, CommentWithBlogDTO>()
                .ForMember(d => d.BlogTitle,
                    o => o.MapFrom(s => s.Blog != null ? s.Blog.Title : string.Empty))
                .ForMember(d => d.BlogSubTitle,
                    o => o.MapFrom(s => s.Blog != null ? s.Blog.SubTitle : null))
                .ForMember(d => d.BlogCategory,
                    o => o.MapFrom(s => s.Blog != null ? s.Blog.Category : string.Empty))
                .ForMember(d => d.BlogImage,
                    o => o.MapFrom(s => s.Blog != null ? s.Blog.Image : string.Empty));
        }
    }
}