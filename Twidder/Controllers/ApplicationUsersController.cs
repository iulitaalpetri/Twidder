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
                return RedirectToAction("New", "Profiles");//de schimbat aici
            }
            else
            {
                string pid = prof.FirstOrDefault().Id;   // Current user -> profile id
                return RedirectToAction("Show/" + pid);
            }
        }


        public ActionResult Index()
        {
            // daca user ul nu are profil, trebuie sa apara butonul cu adauga profil
            string uid = _userManager.GetUserId(User);




            var pr = from p in db.Users where p.Id == uid select p;

            ViewBag.existsProfile = pr.Count();
            

            var search = "";



            // pt a cauta profilul, se va cauta dupa nume

            if (Convert.ToString(HttpContext.Request.Query["search"]) == null) return View();
              

                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

                List<string> idProfile = db.Users.Where(p => p.FirstName.Contains(search)).Select(pr => pr.FirstName).ToList();

                


                var  profile = db.Users.Where(pr => idProfile.Contains(pr.Id));
                //----------------------------------------------------------------------------------------------



            

            var nr = profile.Count();
            var currentPage = Convert.ToInt32(Convert.ToString(HttpContext.Request.Query["page"]));
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perpage;
            }


            var paginatedProfiles = profile.Skip(offset).Take(this._perpage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            ViewBag.total = nr;
            ViewBag.lastpage = Math.Ceiling((float)nr / (float)this._perpage);
            ViewBag.Profiles = paginatedProfiles;
            ViewBag.searchstring = search;
            return View();

        }




        [Authorize(Roles = "User,Admin")]
        public ActionResult Show(int id)
        {
            ApplicationUser profile = db.Users.Find(id);

            var posts = from p in db.Posts where p.UserId == profile.Id select p;

            ViewBag.Posts = posts;

            ViewBag.UserId = (User.Identity.GetHashCode()).ToString();

          /**  if (profile.DeletedByAdmin == true && profile.UserId == User.Identity.GetHashCode().ToString())
            {
                ViewBag.DeletedByAdmin = false;
                db.SaveChanges();

                TempData["warning"] = "O postare a fost stearsa din cauza continutului care nu respecta politica aplicatiei!";

                ViewBag.Warning = TempData["warning"];
            } **/
          // pt stergerea de cate un admin a unui cont


            string currentUserid = _userManager.GetUserId(User);
            var currentUser = db.Users.Find(currentUserid);
            var currentUserProfile = db.Users.Where(p => p.Id == currentUserid).FirstOrDefault();

            var user = profile;

            if (currentUserProfile.SentFriends.Contains(user))
            {

                ViewBag.sentReq = true;

            }
            if (currentUserProfile.Friends.Contains(user))
            {
                ViewBag.friend = true;
            }
            if (currentUserid == profile.Id)
            {
                ViewBag.sameuser = true;
            }
            if (profile.SentFriends.Contains(currentUser))
            {
                ViewBag.nobutton = true;
            }
            return View();

        }







        [Authorize(Roles = "User,Admin")]
        public ActionResult New()
        {
            if (TempData.ContainsKey("friend"))
            {
                ViewBag.Friend = TempData["friend"];
            }


            ApplicationUser profile= new ApplicationUser();
            string uid = _userManager.GetUserId(User); 


            var profiles = from p in db.Users
                           where p.Id == uid
                           select p;

            ViewBag.NoProfile = profiles.Count();
            if (profiles.Count() == 0)
                return View(profile);
            else
                return RedirectToAction("Index", "Profiles");

        }





        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(ApplicationUser profile)
        {
            profile.Id = _userManager.GetUserId(User);
            try
            {
                if (ModelState.IsValid)
                {
                    db.Users.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost adaugat cu succes!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(profile);
                }
            }
            catch (Exception e)
            {
                return View(profile);
            }
        }




        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            ApplicationUser profile = db.Users.Find(id);
            if (profile.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ViewBag.Profile = profile;
                return View(profile);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                return RedirectToAction("Index");
            }
        }




        [HttpPut]
        [Authorize(Roles = "User,Admin")]
        public async Task<ActionResult> EditAsync(int id, ApplicationUser requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser profile = db.Users.Find(id);
                    if (profile.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                    {
                        if (await TryUpdateModelAsync(profile))
                        {
                            profile = requestProfile;
                            
                            db.SaveChanges();
                            TempData["message"] = "Profilul a fost editat!";
                            return RedirectToAction("Index");
                        }
                        return View(requestProfile);
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui profil care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestProfile);
                }
            }
            catch (Exception e)
            {
                return View(requestProfile);
            }
        }




        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            ApplicationUser profile = db.Users.Find(id);
            if (profile.Id == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Users.Remove(profile);
                db.SaveChanges();
                TempData["message"] = "Profilul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un profil care nu va apartine";
                return RedirectToAction("Index");
            }
        }






        [Authorize(Roles = "User,Admin")]
        public ActionResult SendRequest(int id)
        {
            var profile = db.Users.Find(id);
            var user = User;
            var currentUserId = _userManager.GetUserId(User);
            var prof = db.Users.Where(p => p.Id == currentUserId);
            if (prof.Count() == 0)
            {
                TempData["friend"] = "Creeaza-ti un profil pentru a adauga prieteni!";
                return RedirectToAction("New");
            }
            if (currentUserId == profile.Id)
            {
                return RedirectToAction("Index");
            }

            var currentUserProfile = prof.FirstOrDefault();
            var currentUser = db.Users.Find(currentUserId);

            if (currentUserProfile.SentFriends.Contains(profile) || currentUserProfile.Friends.Contains(profile))
            {
                return RedirectToAction("Index");
            }
            if (profile.SentFriends.Contains(currentUser))
            {
                return RedirectToAction("Index");
            }
            currentUserProfile.SentFriends.Add(profile);
            profile.ReceivedFriends.Add(currentUser);
            db.SaveChanges();
            return RedirectToAction("Show/" + id.ToString());
        }





        [Authorize(Roles = "User,Admin")]
        public ActionResult FriendRequests(int id)
        {
            var profile = db.Users.Find(id);
            if (profile.Id != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            ViewBag.profileId = profile.Id;
            ViewBag.FriendRequests = friendRequests;
            ViewBag.Length = friendRequests.Count();
            return View(profile);
        }





        [Authorize(Roles = "User,Admin")]
        public ActionResult AddFriend(int id, int id2)
        {

            var profile = db.Users.Find(id);
            if (profile.Id != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            for (int j = 0; j < friendRequests.Count(); j++)
            {
                var user = friendRequests[j];
                if (id2 == j)
                {
                    profile.Friends.Add(user);
                    var userId = user.Id;
                    var userProfile = db.Users.Where(p => p.Id == userId).FirstOrDefault();
                    userProfile.Friends.Add(profile);

                    Friend friendship = new Friend();
                    friendship.User1_Id = profile.Id;
                    friendship.User1 = profile;
                    friendship.User2_Id = userId;
                    friendship.User2 = user;
                    db.Friends.Add(friendship);

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile);


                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }









        [Authorize(Roles = "User,Admin")]
        public ActionResult Friends(int id)
        {
            var profile = db.Users.Find(id);
            ViewBag.currentUser = _userManager.GetUserId(User);
            ViewBag.UserId = profile.Id.ToString();
            /* ViewBag.friends = profile.Friends;*/
            var userId = profile.Id;
            var friendships1 = db.Friends.Where(f => f.User1_Id == userId);
            List<ApplicationUser> friends1 = new List<ApplicationUser>();
            foreach (var friendship in friendships1)
            {
                friends1.Add(friendship.User2);
            }
            var friendships2 = db.Friends.Where(f => f.User2_Id == userId);
            List<ApplicationUser> friends2 = new List<ApplicationUser>();
            foreach (var friendship in friendships2)
            {
                friends2.Add(friendship.User1);
            }
            var friends = friends1.Union(friends2);

            List<ApplicationUser> Friends = new List<ApplicationUser>();
            foreach (var friend in friends)
            {
                Friends.Add(friend);
            }
            ViewBag.friends = Friends;
            ViewBag.Length = friends.Count();
            ViewBag.profileId = profile.Id;
            return View(profile);
        }




        [Authorize(Roles = "User,Admin")]
        public ActionResult DeclineFriend(int id, int id2)
        {
            var profile = db.Users.Find(id);
            if (profile.Id != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            for (int j = 0; j < friendRequests.Count(); j++)
            {
                var user = friendRequests[j];
                if (id2 == j)
                {
                    var userId = user.Id;
                    var userProfile = db.Users.Where(p => p.Id == userId).FirstOrDefault();

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile);

                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }


        [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteFriend(int id, int id2)
        {

            var profile = db.Users.Find(id);
            if (profile.Id != _userManager.GetUserId(User))
            {
                return RedirectToAction("Index");
            }
            var userId = profile.Id;
            var friendships1 = db.Friends.Where(f => f.User1_Id == userId);
            List<ApplicationUser> friends1 = new List<ApplicationUser>();
            foreach (var friendship in friendships1)
            {
                friends1.Add(friendship.User2);
            }
            var friendships2 = db.Friends.Where(f => f.User2_Id == userId);
            List<ApplicationUser> friends2 = new List<ApplicationUser>();
            foreach (var friendship in friendships2)
            {
                friends2.Add(friendship.User1);
            }
            var friends = friends1.Union(friends2);
            List<ApplicationUser> Friends = new List<ApplicationUser>();
            foreach (var friend in friends)
            {
                Friends.Add(friend);
            }

            for (int j = 0; j < Friends.Count(); j++)
            {
                var user = Friends[j];
                if (id2 == j)
                {
                    var user2Id = user.Id;
                    var userProfile = db.Users.Where(p => p.Id == user2Id).FirstOrDefault();

                    var friendShips = db.Friends.Where(f => (f.User1_Id == userId && f.User2_Id == user2Id) || (f.User2_Id == userId && f.User1_Id == user2Id));
                    foreach (var friendship in friendShips)
                    {
                        db.Friends.Remove(friendship);
                    }
                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("Friends/" + id.ToString());
        }



        [Authorize(Roles = "User,Admin")]
        public ActionResult JoinedGroups(int id)
        {
            var profile = db.Users.Find(id);
            var user = profile;
            ViewBag.joinedGroups = user.Groups;
            return View(profile);
        }





































    }
}
