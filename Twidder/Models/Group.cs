using System.ComponentModel.DataAnnotations;


namespace Twidder.Models
{
    public class Group
    {

         [Key]
        public int Id { get; set; }

        
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        [StringLength(30, ErrorMessage = "Numele grupului nu poate avea mai mult de 30 de caractere")]
        
        public string GroupDescription { get; set; }

        public string? CreatorId { get; set; }
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }

}

