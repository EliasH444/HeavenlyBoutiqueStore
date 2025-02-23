using Microsoft.AspNetCore.Identity;

namespace learning.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }  // ✅ Custom property
    }
}
