using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfilesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        // pt imagine
        public IActionResult UploadImage()
        {
            return View();
        }






//-------------------------------------------------------------------
        // Facem upload la fisier si salvam modelul in baza de date
        [HttpPost]
        public async Task<IActionResult> UploadImage(Profile profile,
        IFormFile ProfileImage)
        {
            // Verificam daca exista imaginea in request (daca a fost incarcata o imagine)
            if (ProfileImage.Length > 0)
            {
                // Generam calea de stocare a fisierului
                var storagePath = Path.Combine(
                _env.WebRootPath, // Luam calea folderului wwwroot
                "images", // Adaugam calea folderului images
                ProfileImage.FileName // Numele fisierului
                );

                // General calea de afisare a fisierului care va fi stocata in baza de date
                 var databaseFileName = "/images/" + ProfileImage.FileName;
                // Uploadam fisierul la calea de storage
                using (var fileStream = new FileStream(storagePath,
               FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(fileStream);
                }
            }
            // Salvam storagePath-ul in baza de date
            profile.ProfilePicture = databaseFileName;
            _context.Profiles.Add(profile);
            _context.SaveChanges();
            return View();
        }
//--------------------------------------------------------------------------------






            public IActionResult Index()
        {
          var profiles = db.Profiles.Include("User");


           ViewBag.Profiles = profiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        [HttpPost]

        public IActionResult Settings([FromForm] Comment comment)
        {

                return View();
            
        }

        // Profil nou
        public IActionResult New()
        {
            Post post = new Post();



            return View(post);
        }


        // Se adauga profilul in baza de date
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
    }
}
