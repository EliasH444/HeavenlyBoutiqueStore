using System.ComponentModel.DataAnnotations;

namespace learning.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
