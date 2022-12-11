using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Twidder.Models.ArticlesApp.Models;

namespace Twidder.Models
{
    public class Post
    {

        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Continutul postării este obligatoriu")]
        [StringLength(100, ErrorMessage = "Conținutul postării trebuie să conșină mai mult de 100 de caractere")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ApplicationUser User { get; set; }

        // [NotMapped]

    }



}
