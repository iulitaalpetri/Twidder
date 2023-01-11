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




        //[Authorize(Roles = "User,Admin")]
        //public ActionResult Friends(int id)
        //{
        //    var profile = db.Users.Find(id);
        //    ViewBag.currentUser = _userManager.GetUserId(User);
        //    ViewBag.UserId = profile.Id.ToString();
        //    /* ViewBag.friends = profile.Friends;*/
        //    var userId = profile.Id;
        //    var friendships1 = db.Friends.Where(f => f.User1_Id == userId);
        //    List<ApplicationUser> friends1 = new List<ApplicationUser>();
        //    foreach (var friendship in friendships1)
        //    {
        //        friends1.Add(friendship.User2);
        //    }
        //    var friendships2 = db.Friends.Where(f => f.User2_Id == userId);
        //    List<ApplicationUser> friends2 = new List<ApplicationUser>();
        //    foreach (var friendship in friendships2)
        //    {
        //        friends2.Add(friendship.User1);
        //    }
        //    var friends = friends1.Union(friends2);

        //    List<ApplicationUser> Friends = new List<ApplicationUser>();
        //    foreach (var friend in friends)
        //    {
        //        Friends.Add(friend);
        //    }
        //    ViewBag.friends = Friends;
        //    ViewBag.Length = friends.Count();
        //    ViewBag.profileId = profile.Id;
        //    return View(profile);
        //}


        //// Send Friend Request
        //[HttpPost]
        //[Authorize(Roles = "User,Admin")]
        //public ActionResult SendRequest(int id)
        //{
        //    var profile = db.Users.Find(id);
        //    var user = User;
        //    var currentUserId = _userManager.GetUserId(User);
        //    var prof = db.Users.Where(p => p.Id == currentUserId);

        //    // Daca utilizatorul nu e logat -> trimite-l la login
        //    if (prof.Count() == 0)
        //    {
        //        TempData["friend"] = "Creeaza-ti un profil pentru a adauga prieteni!";
        //        return RedirectToAction("New");
        //    }
        //    if (currentUserId == profile.Id)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var currentUserProfile = prof.FirstOrDefault();
        //    var currentUser = db.Users.Find(currentUserId);

        //    // Daca a fost deja trimis friend request-ul -> trimite-l inapoi la lista de utilizatori
        //    if (currentUserProfile.SentFriends.Contains(profile) || currentUserProfile.Friends.Contains(profile))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    if (profile.SentFriends.Contains(currentUser))
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    // Friend Request
        //    currentUserProfile.SentFriends.Add(profile);
        //    profile.ReceivedFriends.Add(currentUser);
        //    db.SaveChanges();
        //    return RedirectToAction("Show/" + id.ToString());
        //}


        //// Afiseaza friend request-urile primite de utilizator
        //[Authorize(Roles = "User,Admin")]
        //public ActionResult FriendRequests(int id)
        //{
        //    var profile = db.Users.Find(id);
        //    if (profile.Id != _userManager.GetUserId(User))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<ApplicationUser> friendRequests = new List<ApplicationUser>();
        //    foreach (var user in profile.ReceivedFriends)
        //    {
        //        friendRequests.Add(user);
        //    }
        //    ViewBag.profileId = profile.Id;
        //    ViewBag.FriendRequests = friendRequests;
        //    ViewBag.Length = friendRequests.Count();
        //    return View(profile);
        //}

        //// Accepta friend request-ul primit
        //[Authorize(Roles = "User,Admin")]
        //public ActionResult AddFriend(int id, int id2)
        //{

        //    var profile = db.Users.Find(id);
        //    if (profile.Id != _userManager.GetUserId(User))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<ApplicationUser> friendRequests = new List<ApplicationUser>();
        //    foreach (var user in profile.ReceivedFriends)
        //    {
        //        friendRequests.Add(user);
        //    }
        //    for (int j = 0; j < friendRequests.Count(); j++)
        //    {
        //        var user = friendRequests[j];
        //        if (id2 == j)
        //        {
        //            profile.Friends.Add(user);
        //            var userId = user.Id;
        //            var userProfile = db.Users.Where(p => p.Id == userId).FirstOrDefault();
        //            userProfile.Friends.Add(profile);

        //            Friend friendship = new Friend();
        //            friendship.User1_Id = profile.Id;
        //            friendship.User1 = profile;
        //            friendship.User2_Id = userId;
        //            friendship.User2 = user;
        //            db.Friends.Add(friendship);

        //            profile.ReceivedFriends.Remove(user);
        //            userProfile.SentFriends.Remove(profile);


        //            db.SaveChanges();
        //            break;
        //        }
        //    }
        //    return RedirectToAction("FriendRequests/" + id.ToString());
        //}



        //// Refuza friend request-ul
        //[Authorize(Roles = "User,Admin")]
        //public ActionResult DeclineFriend(int id, int id2)
        //{
        //    var profile = db.Users.Find(id);
        //    if (profile.Id != _userManager.GetUserId(User))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<ApplicationUser> friendRequests = new List<ApplicationUser>();
        //    foreach (var user in profile.ReceivedFriends)
        //    {
        //        friendRequests.Add(user);
        //    }
        //    for (int j = 0; j < friendRequests.Count(); j++)
        //    {
        //        var user = friendRequests[j];
        //        if (id2 == j)
        //        {
        //            var userId = user.Id;
        //            var userProfile = db.Users.Where(p => p.Id == userId).FirstOrDefault();

        //            profile.ReceivedFriends.Remove(user);
        //            userProfile.SentFriends.Remove(profile);

        //            db.SaveChanges();
        //            break;
        //        }
        //    }
        //    return RedirectToAction("FriendRequests/" + id.ToString());
        //}

        //// Sterge prieten
        //[Authorize(Roles = "User,Admin")]
        //public ActionResult DeleteFriend(int id, int id2)
        //{

        //    var profile = db.Users.Find(id);
        //    if (profile.Id != _userManager.GetUserId(User))
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    var userId = profile.Id;
        //    var friendships1 = db.Friends.Where(f => f.User1_Id == userId);
        //    List<ApplicationUser> friends1 = new List<ApplicationUser>();
        //    foreach (var friendship in friendships1)
        //    {
        //        friends1.Add(friendship.User2);
        //    }
        //    var friendships2 = db.Friends.Where(f => f.User2_Id == userId);
        //    List<ApplicationUser> friends2 = new List<ApplicationUser>();
        //    foreach (var friendship in friendships2)
        //    {
        //        friends2.Add(friendship.User1);
        //    }
        //    var friends = friends1.Union(friends2);
        //    List<ApplicationUser> Friends = new List<ApplicationUser>();
        //    foreach (var friend in friends)
        //    {
        //        Friends.Add(friend);
        //    }

        //    for (int j = 0; j < Friends.Count(); j++)
        //    {
        //        var user = Friends[j];
        //        if (id2 == j)
        //        {
        //            var user2Id = user.Id;
        //            var userProfile = db.Users.Where(p => p.Id == user2Id).FirstOrDefault();

        //            var friendShips = db.Friends.Where(f => (f.User1_Id == userId && f.User2_Id == user2Id) || (f.User2_Id == userId && f.User1_Id == user2Id));
        //            foreach (var friendship in friendShips)
        //            {
        //                db.Friends.Remove(friendship);
        //            }
        //            db.SaveChanges();
        //            break;
        //        }
        //    }
        //    return RedirectToAction("Friends/" + id.ToString());
        //}


        // ---------------------------------------- GRUPURI --------------------------------------------------------
        [Authorize(Roles = "User,Admin")]
        public ActionResult JoinedGroups(int id)
        {
            var profile = db.Users.Find(id);
            var user = profile;
            ViewBag.joinedGroups = user.Groups;
            return View(profile);
        }


       /* public IActionResult Index()
        {
            return View();
        }
       */
    }
}
