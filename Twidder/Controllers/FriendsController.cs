using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Twidder.Data;
using Twidder.Models;
namespace Twidder.Controllers
{
    public class FriendsController : Controller
    {
        private ApplicationDbContext db;
        private int _perpage = 100;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public FriendsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        // Vizualizeaza prietenii utilizatorului
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> Index()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            List<ApplicationUser> friendsFrom = new List<ApplicationUser>();
            List<ApplicationUser> friendsTo = new List<ApplicationUser>();
            List<ApplicationUser> friends = new List<ApplicationUser>();

            friendsFrom = db.Friends.Where(f => (f.RequestFrom_Id == currentUser.Id) && f.friends== true).Include("RequestTo").Select(u => u.RequestTo).ToList();
            friendsTo = db.Friends.Where(f => (f.RequestTo_Id == currentUser.Id) && f.friends == true).Include("RequestFrom").Select(u => u.RequestFrom).ToList();
            friends = friendsFrom.Union(friendsTo).ToList();
            ViewBag.Friends = friends;

            List<ApplicationUser> requestFrom = new List<ApplicationUser>();
            requestFrom = db.Friends.Where(f => (f.RequestTo_Id == currentUser.Id) && f.friends == false).Include("RequestFrom").Select(u => u.RequestFrom).ToList();
            ViewBag.RequestFrom = requestFrom;


            return View(currentUser);
        }

        // Send Friend Request
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> SendRequest(string id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            var requestSent = db.Friends.Where(u =>
                                     u.RequestFrom_Id == _userManager.GetUserId(User) &&
                                     u.RequestTo_Id == id)
                             .FirstOrDefault();

            if (requestSent != null)
                return View("Index");

            var alreadyFriends = db.Friends.Where(u =>
                                        u.RequestFrom_Id == _userManager.GetUserId(User) &&
                                        u.RequestTo_Id == id &&
                                        u.friends == true)
                                .FirstOrDefault();
            if(alreadyFriends != null)
                return View("Index");

            var requestFrom = db.Friends.Where(u =>
                            u.RequestFrom_Id == id  &&
                            u.RequestTo_Id == _userManager.GetUserId(User))
                    .FirstOrDefault();

            if (requestFrom != null)
            {
                requestFrom.friends = true;
                db.SaveChanges();
                return View("Index");
            }

            Friend friend = new Friend();
            friend.RequestFrom_Id = currentUser.Id;
            friend.RequestTo_Id = id;
            db.Friends.Add(friend);
            db.SaveChanges();
            return View("Index");
        }

        // Accept Friend Request
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> AcceptRequest(string id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            var requestSent = db.Friends.Where(u =>
                                     u.RequestFrom_Id == id &&
                                     u.RequestTo_Id == _userManager.GetUserId(User) && u.friends == false)
                             .FirstOrDefault();

            if (requestSent == null)
                return View("Index");

            requestSent.friends = true;
            db.SaveChanges();
            return View("Index");
        }

        // Accept Friend Request
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> DeleteRequest(string id)
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            var requestSent = db.Friends.Where(u =>
                                     ((u.RequestFrom_Id == id &&
                                        u.RequestTo_Id == _userManager.GetUserId(User)) ||
                                     ((u.RequestFrom_Id == _userManager.GetUserId(User) &&
                                        u.RequestTo_Id == id)) && 
                                     u.friends == true)
                                     )
                             .FirstOrDefault();

            if (requestSent == null)
                return View("Index");
            db.Friends.Remove(requestSent);
            db.SaveChanges();
            return View("Index");
        }

    }
}
