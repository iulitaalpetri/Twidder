using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Twidder.Models
{

    public class Friend
    {
        public int FriendshipId { get; set; }
        [ForeignKey("User1")]
        public string User1_Id { get; set; }
        public virtual ApplicationUser User1 { get; set; }

        [ForeignKey("User2")]
        public string User2_Id { get; set; }
        public virtual ApplicationUser User2 { get; set; }

    }

}
