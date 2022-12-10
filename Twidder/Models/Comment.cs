using System.ComponentModel.DataAnnotations;
using Twidder.Models.ArticlesApp.Models;

namespace Twidder.Models
{
    public class Comment
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? PostId { get; set; }

        public virtual Post? Post { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}
