using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        // afiseaza grupuri
        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var groups = db.Groups;
            ViewBag.Groups = groups;
            ViewBag.CurrentUser = db.Users.Find(_userManager.GetUserId(User));
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Group grup = new Group();
            grup.CreatorId = _userManager.GetUserId(User).ToString();
            db.SaveChanges();


            return View(grup);
        }

        // creeaza grup nou

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(Group grup)
        {
            if (_userManager.GetUserId(User) == null)
                return RedirectToAction("Index");


            // iau ID-ul userului care creeaza grupul
            grup.CreatorId = _userManager.GetUserId(User);

            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(grup);
                    db.SaveChanges();
                    // adauga creatorul ca membru al grupului
                    ApplicationUser user = db.Users.Find(_userManager.GetUserId(User));
                    grup.Users.Add(user);

                    //adauga grupul crea la grupurile userului
                    user.Groups.Add(grup);

                    db.SaveChanges();
                    TempData["message"] = "Grupul a fost adaugat cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(grup);
                }
            }
            catch (Exception e)
            {
                return View(grup);
            }

        }


        public IActionResult Show(int id)
        {
            Group grup = db.Groups.Include("Users").Include("Posts")
                                 .Where(grup => grup.Id == id)
                                 .First();

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            return View(grup);
        }



        [HttpPost]
        public IActionResult Show([FromForm] Post post)
        {
            post.Date = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + post.Id);
            }

            else
            {
                Group grup = db.Groups.Include("User").Include("Posts")
                               .Where(grup => grup.Id == post.Id)
                               .First();



                return View(grup);
            }
        }


        // Editare grup
        public IActionResult Edit(int id)
        {

            Group grup = db.Groups.Include("User")
                                        .Where(grup => grup.Id == id)
                                        .First();

            return View();

        }

        // Se adauga grupul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(int id, Group requestGroup)
        {
            Group grup = db.Groups.Find(id);

            if (ModelState.IsValid)
            {

                grup.GroupName = requestGroup.GroupName;
                grup.GroupDescription = requestGroup.GroupDescription;
                TempData["message"] = "Grupul a fost modificat";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestGroup);
            }
        }

        // Stergerea unui grup
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            TempData["message"] = "Grupul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /*



        public ActionResult Index()


        {
            var groups = db.Groups.Include("User");


            ViewBag.Groups = groups;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }


            return View();
        }

        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);
            return View(group);
        }

        public ActionResult New()
        {
            return View();
        }



        public ActionResult Edit(int id)
        {
            Group group = db.Groups.Find(id);
            return View( group);
        }

        [HttpPost]
        public ActionResult Edit(int id, Group requestGroup)
        {
            Group category = db.Groups.Find(id);

            if (ModelState.IsValid)
            {

                category.GroupName = requestGroup.GroupName;
                db.SaveChanges();
                TempData["message"] = "Grupul a fost modificata!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestGroup);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            TempData["message"] = "Grupul a fost sters";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */


    }
}
