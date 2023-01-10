using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Twidder.Models;

namespace Twidder.Models
{

    

        public class ApplicationUser : IdentityUser
        {
        public ApplicationUser()
        {
            Groups = new List<Group>();
            Friends = new List<ApplicationUser>(); 
            SentFriends= new List<ApplicationUser>();
            ReceivedFriends = new List<ApplicationUser>();

        }



        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public string? ProfilePictureFilePath { get; set; }

        public virtual ICollection<Group> Groups { get; set; }

        public bool PrivateProfile { get; set; }

        
        //public bool DeletedByAdmin { get; set; }

        public virtual ICollection<Post> Posts { get; set; }


        public virtual ICollection<ApplicationUser> Friends { get; set; }


        public virtual ICollection<ApplicationUser> SentFriends { get; set; }


        public virtual ICollection<ApplicationUser> ReceivedFriends { get; set; }

    }

    


}
