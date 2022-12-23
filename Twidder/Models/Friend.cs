using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twidder.Models
{
    public class Friend
    {
        [Key] 
        public int Id { get; set; }

        public virtual ApplicationUser? AccountId { get; set; }

        public virtual ApplicationUser? MyFriendAccountId { get; set; }

        public DateTime CreateDate { get; set; }

        public Timestamp Timestamp { get; set; }
    }
}
