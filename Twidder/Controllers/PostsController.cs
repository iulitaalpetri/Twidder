using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{

    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db= context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        public IActionResult Index()
        {
            // includ si userul
            
            var posts = db.Posts.Include("User");


            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }


            return View();
        }


        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("User").Include("Comments")
                                 .Where(pst => pst.Id == id)
                                 .First();

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            return View(post);
        }


        
        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }

            else
            {
                Post pst = db.Posts.Include("User").Include("Comments")
                               .Where(pst => pst.Id == comment.PostId)
                               .First();

                

                return View(pst);
            }
        }

        // Se afiseaza formularul in care se vor completa datele unui articol
        // impreuna cu selectarea categoriei din care face parte
        // HttpGet implicit

        public IActionResult New()
        {
            Post post = new Post();

            

            return View(post);
        }


        // Se adauga articolul in baza de date
        [HttpPost]
        public IActionResult New(Post post)
        {
            post.Date = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }

            
        }


        public IActionResult Edit(int id)
        {

            Post post = db.Posts.Include("User")
                                        .Where(pst => pst.Id == id)
                                        .First();

            //article.Categ = GetAllCategories();

            return View(post);

        }

        // Se adauga articolul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);
            //requestPost.Categ = GetAllCategories();

            if (ModelState.IsValid)
            {
                
                post.Content = requestPost.Content;
                post.User = requestPost.User;
                TempData["message"] = "Postarea a fost modificata";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Show", new { id = post.Id });
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        public ActionResult Delete(int id)
        {

            Post post = db.Posts.Include("Comments")
                                         .Where(art => art.Id == id)
                                         .First();

            db.Posts.Remove(post);
            db.SaveChanges();
            TempData["message"] = "Postarea a fost stearsa";
            return RedirectToAction("Index");
        }

        public IActionResult IndexNou()
        {
            return View();
        }

    }
}
