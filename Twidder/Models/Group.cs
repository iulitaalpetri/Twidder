using System.ComponentModel.DataAnnotations;


namespace Twidder.Models
{
    public class Group
    {
        public Group()
        {
            Users = new List<ApplicationUser>();
            Posts = new List<Post>();
        }
        [Key]
        public int Id { get; set; }

        
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [StringLength(30, ErrorMessage = "Numele grupului nu poate avea mai mult de 30 de caractere")]
        
        public string GroupDescription { get; set; }
        [StringLength(1000, ErrorMessage = "Descrierea grupului nu poate avea mai mult de 1000 de caractere")]
        public string? CreatorId { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }

}

