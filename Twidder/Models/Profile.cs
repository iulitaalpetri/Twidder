using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Twidder.Models
{
    public class Profile : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul username-ului este obligatoriu")]
        [StringLength(50, ErrorMessage = "Username trebuie să contină maaxim de 50 de caractere")]
        public string Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string Email { get; set; }

        public bool isPrivate { get; set; }

        public string ProfilePicture { get; set; }


        public virtual ICollection<Post>? Posts { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}



