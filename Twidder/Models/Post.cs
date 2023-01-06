using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Twidder.Models
{
    public class Post
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(40, ErrorMessage = "Titlul nu poate avea mai mult de 40 de caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul postării este obligatoriu")]
        [StringLength(100, ErrorMessage = "Conținutul postării trebuie să contină mai mult de 100 de caractere")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Group Group { get; set; }
        
        public virtual Profile Profile { get; set; }

        // [NotMapped]

    }



}