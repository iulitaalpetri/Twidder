using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{
    public class ApplicationUsersController : Controller
    {

        private ApplicationDbContext db;
        private int _perpage = 100;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUsersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }


        public ActionResult MyProfile()
        {

            string x = _userManager.GetUserId(User);
            string uid = x.ToString();
            var prof = db.Users.Where(p => p.Id == uid);


            ViewBag.myProfile = true;
            if (prof.Count() == 0)
            {
                return RedirectToAction("New", "ApplicationUsers");//de schimbat aici
            }
            else
            {
                string pid = prof.FirstOrDefault().Id;   // Current user -> profile id
                return RedirectToAction("Show/" + pid);
            }
        }

        public ActionResult Index(string searchString, int currentPage = 0) {
            List<ApplicationUser> allUsers = null;
            if(searchString is not null)
                allUsers = _userManager.Users.Where(u => String.Concat(u.FirstName, " ", u.LastName).Contains(searchString)).ToList();
            else
                allUsers = _userManager.Users.ToList();



            ViewBag.searchString = searchString;
            ViewBag.currentPage = currentPage;
            ViewBag.lastPage = Math.Ceiling(allUsers.Count() / 5d);

            allUsers = allUsers.Skip(currentPage * 5).Take(5).ToList();

            return View(allUsers);
        }



        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser targetUser = db.Users.Find(id);
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            ViewBag.UserId = _userManager.GetUserId(User);
            ViewBag.sameuser = currentUser.Id == targetUser.Id;
            var friends = db.Friends.Where(u => 
                                        u.RequestFrom_Id == _userManager.GetUserId(User) &&
                                        u.RequestTo_Id == id &&
                                        u.friends == true)
                                .FirstOrDefault();
            

            List<Post> targetUserPosts = new List<Post>();
            if (!targetUser.PrivateProfile || (targetUser.PrivateProfile && friends is not null ))
                targetUserPosts = db.Posts.Where(p => p.UserId == targetUser.Id).ToList();

            ViewBag.Posts = targetUserPosts;

            /**  if (profile.DeletedByAdmin == true && profile.UserId == User.Identity.GetHashCode().ToString())
              {
                  ViewBag.DeletedByAdmin = false;
                  db.SaveChanges();

                  TempData["warning"] = "O postare a fost stearsa din cauza continutului care nu respecta politica aplicatiei!";

                  ViewBag.Warning = TempData["warning"];
              } **/
            // pt stergerea de cate un admin a unui cont


            //string currentUserid = _userManager.GetUserId(User);
            //var currentUser = db.Users.Find(currentUserid);
            //var currentUserProfile = db.Users.Where(p => p.Id == currentUserid).FirstOrDefault();
            

            return View(targetUser);
        }
        

    }
}
