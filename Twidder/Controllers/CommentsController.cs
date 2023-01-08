using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(

            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        
        
        // Adaugarea unui comentariu asociat unei postari in baza de date
        [HttpPost]
        public IActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;

            if(ModelState.IsValid)
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }

            else
            {
                return Redirect("/Posts/Show/" + comm.PostId);
            }

        }

        
        


        // Stergerea unui comentariu asociat unei postari din baza de date
        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
                return RedirectToAction("Index", "Posts");
            }
        }

        // Se editeaza un comentariu existent intr-o pagina View separata
        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            if(comm == null)
            {
                return Redirect("/Posts");

            }
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                return RedirectToAction("Index", "Posts");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);
            if (comm == null)
            {
                return Redirect("/Posts");

            }

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
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
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Posts");
            }
        }
    }

}
