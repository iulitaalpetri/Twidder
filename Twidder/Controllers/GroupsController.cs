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
        public IActionResult Index(string searchString, int currentPage = 0)
        {
            List<Group> groups = db.Groups.ToList();
            ViewBag.CurrentUser = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            
            // search group by name
            if (searchString is not null)
                groups = groups.Where(g => 
                                g.GroupName.ToLower().Contains(searchString.ToLower()))
                                .ToList();
            
            // pagination
            ViewBag.searchString = searchString;
            ViewBag.currentPage = currentPage;
            ViewBag.lastPage = Math.Ceiling(groups.Count() / 5d);

           ViewBag.Groups = groups.Skip(currentPage * 5).Take(5).ToList();

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
        public async Task<IActionResult> NewAsync(Group grup)
        {
            if (User == null)
            {
                return RedirectToAction("Index");
            }

            // iau ID-ul userului care creeaza grupul
            grup.CreatorId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Groups.Add(grup);
                db.SaveChanges();

                //adauga grupul crea la grupurile userului
                ApplicationUser user = await _userManager.GetUserAsync(User);

                grup.Users.Add(user);
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

        // detalii grup
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Show(int id)
        {
            Group grup = db.Groups.Include("Users").Include("Posts")
                                 .Where(grup => grup.Id == id)
                                 .First();
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.IsInGroup = grup.Users.Contains(await _userManager.GetUserAsync(User));
            var user = _userManager.FindByIdAsync(grup.CreatorId);
            ViewBag.CreatorUser = user.Result.FirstName.ToString() + " " + user.Result.LastName.ToString();
            return View(grup);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Show([FromForm] Post post, int GroupId)
        {
            post.Date = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);

            Group grup = db.Groups.Where(g => g.Id == GroupId)
                .FirstOrDefault();

            if (ModelState.IsValid)
            {

                db.Posts.Add(post);
                post.Group = grup;
                grup.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Show", new { id = grup.Id });
            }

            else
            {
                Group group = db.Groups.Include("User").Include("Posts")
                               .Where(group => group.Id == post.Id)
                               .First();



                return View(group);
            }
        }


        // Editare grup
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Group grup = db.Groups
                                        .Where(grup => grup.Id == id)
                                        .First();

            return View(grup);

        }

        // Se adauga grupul modificat in baza de date
        [Authorize(Roles = "User,Admin")]
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
        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            TempData["message"] = "Grupul a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // ---------------------------------------- GRUPURI --------------------------------------------------------
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> JoinGroup(int id)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            Group grup = db.Groups.Include("Users").Include("Posts")
                                 .Where(grup => grup.Id == id)
                                 .First();

            grup.Users.Add(user);
            user.Groups.Add(grup);
            db.SaveChanges();
            // return RedirectToAction("Index");
            return RedirectToAction("Show", new { id = grup.Id });
        }
      }
}
