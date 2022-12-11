using Microsoft.AspNetCore.Mvc;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{
    public class CommentsController : Controller
    {


        private readonly ApplicationDbContext db;

        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }

        // Stergerea unui comentariu asociat unui articol din baza de date
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Posts/Show/" + comm.PostId);
        }

        // In acest moment vom implementa editarea intr-o pagina View separata
        // Se editeaza un comentariu existent

        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            return View(comm);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);

            if (ModelState.IsValid)
            {

                comm.Content = requestComment.Content;

                db.SaveChanges();

                return Redirect("/Posts/Show/" + comm.PostId);
            }
            else
            {
                return View(requestComment);
            }

        }



       
    }
}
