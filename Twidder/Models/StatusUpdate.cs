using MessagePack;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Twidder.Models
{
    public class StatusUpdate
    {
        [Key]
        public int Id { get; set; } 

        public DateTime CreateDate { get; set; } 

        public string? Status { get; set; } 
        
        public virtual ApplicationUser? AccountId { get; set;  } 

        public Timestamp Timestamp { get; set; } 










    }
}
