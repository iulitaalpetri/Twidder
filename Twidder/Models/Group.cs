using System.ComponentModel.DataAnnotations;


namespace Twidder.Models
{
    public class Group
    {

         [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        [StringLength(30, ErrorMessage = "Numele nu poate avea mai mult de 30 de caractere")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string GroupDescription { get; set; }
        public string CreatorId { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }

}

