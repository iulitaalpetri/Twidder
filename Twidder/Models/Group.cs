using System.ComponentModel.DataAnnotations;


namespace Twidder.Models
{
    public class Group
    {

         [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        public string GroupName { get; set; }

        public virtual ICollection<ApplicationUser>? ApplicationUser { get; set; }
    }

}

