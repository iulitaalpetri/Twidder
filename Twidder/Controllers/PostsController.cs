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

            return View();

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
                return View(requestPost);
            }
        }


        // Se sterge un articol din baza de date 
        [HttpPost]
        public ActionResult Delete(int id)
        {

            Post post = db.Posts.Find(id);
            
            db.Posts.Remove(post);
            db.SaveChanges();
            TempData["message"] = "Postarea a fost stearsa";
            return RedirectToAction("Index");
        }

        [NonAction]
       // public IEnumerable<SelectListItem> GetAllCategories()
        //{
            // generam o lista de tipul SelectListItem fara elemente
            //var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
           // var users = from u in db.
                            // select cat;

            // iteram prin categorii
           // foreach (var category in categories)
            //{
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                //selectList.Add(new SelectListItem
                //{
                    //Value = category.Id.ToString(),
                  //  Text = category.CategoryName.ToString()
                //});
            //}
            /* Sau se poate implementa astfel: 
             * 
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.Id.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
             }*/


            // returnam lista de categorii
            //return selectList;
        //}

        public IActionResult IndexNou()
        {
            return View();
        }

    }
}
