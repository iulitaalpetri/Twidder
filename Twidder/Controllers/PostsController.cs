using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        public PostsController(ApplicationDbContext context){
            db= context; 
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

        // Se afiseaza un singur articol in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate comentariile asociate unui articol
        // HttpGet implicit
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("User").Include("Comments")
                                 .Where(pst => pst.Id == id)
                                 .First();

            return View(post);
        }

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;

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
        public IActionResult New(Article article)
        {
            article.Date = DateTime.Now;
            article.Categ = GetAllCategories();

            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(article);
            }

            /*
             try
             {
                 db.Articles.Add(article);
                 db.SaveChanges();
                 TempData["message"] = "Articolul a fost adaugat";
                 return RedirectToAction("Index");
             }

             catch (Exception)
             {
                 return View(article);
             }
             */
        }

    }
}
