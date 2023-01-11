using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Twidder.Models
{
    public class Friend
    {

        [Key]
        public int id { get; set; }

        public int FriendshipId { get; set; }

        [ForeignKey("RequestFrom")]
        public string? RequestFrom_Id { get; set; }
        public virtual ApplicationUser RequestFrom { get; set; }


        [ForeignKey("RequestTo")]
        public string? RequestTo_Id { get; set; }
        public virtual ApplicationUser RequestTo { get; set; }

        public bool friends { get; set; }

    }
}
