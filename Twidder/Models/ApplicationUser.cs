using Microsoft.AspNetCore.Identity;
using Twidder.Models;

namespace Twidder.Models
{

    

        public class ApplicationUser : IdentityUser
        {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public string? ProfilePictureFilePath { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

    }

    


}
