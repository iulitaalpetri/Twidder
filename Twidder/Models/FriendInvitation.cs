using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace Twidder.Models
{
    public class FriendInvitation
    {
        [Key]
        public int Id { get; set; } 

        public virtual ApplicationUser? AccountId { get; set; } 

        public string? Email { get; set; }// mail de invitare

        
        public string GUID { get; set; } // ptr email, allow the user to create a new account

        public DateTime CreateDate { get; set; } 

        public int BecameAccountId { get; set; } 

        public Timestamp Timestamp { get; set; }



    }
}
