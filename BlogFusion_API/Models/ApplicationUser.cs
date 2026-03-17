using Microsoft.AspNetCore.Identity;

namespace BlogFusion_API.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}



