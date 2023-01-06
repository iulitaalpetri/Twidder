using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Twidder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twidder.Models;
using System.Collections.Generic;
using System.Linq;



namespace Twidder.Controllers
{
    public class ProfilesController : Controller
    {


        private ApplicationDbContext db;
        private int _perpage = 100;

        [Authorize(Roles = "User,Admin")]// verifica


        public ActionResult MyProfile()
        {

            int x = User.Identity.GetHashCode();
            string uid = x.ToString();
            var prof = db.Profiles.Where(p => p.UserId == uid);


            ViewBag.myProfile = true;
            if (prof.Count() == 0)
            {
                return RedirectToAction("New", "Profiles");
            }
            else
            {
                int pid = prof.FirstOrDefault().ProfileId;   // Current user -> profile id
                return RedirectToAction("Show/" + pid.ToString());
            }
        }


        
        public ActionResult Index()
        {
            // daca user ul nu are profil, trebuie sa apara butonul cu adauga profil
            int x = User.Identity.GetHashCode();
            string uid = x.ToString();

            


            var pr = from p in db.Profiles where p.UserId == uid select p;

            ViewBag.existsProfile = pr.Count();


            var search = "";
            var profile = db.Profiles.OrderBy(p => p.SignUpDate);


            // pt a cauta profilul, se va cauta dupa nume
            if (Request.Params.Get("search") != null)
            {

                search = Request.Params.Get("search").Trim();

                List<int> idProfile = db.Profiles.Where(p => p.ProfileName.Contains(search)).Select(pr => pr.ProfileId).ToList();

                // ordinea in care primim profilurile 
                profile = db.Profiles.Where(pr => idProfile.Contains(pr.ProfileId)).OrderBy(p => p.SignUpDate);


            }

            var nr = profile.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
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
            Profile profile = db.Profiles.Find(id);

            var posts = from p in db.Posts where p.UserId == profile.UserId select p;

            ViewBag.Posts = posts;

            ViewBag.UserId = (User.Identity.GetHashCode()).ToString();

            if (profile.DeletedByAdmin == true && profile.UserId == User.Identity.GetHashCode().ToString())
            {
                ViewBag.DeletedByAdmin = false;
                db.SaveChanges();

                TempData["warning"] = "O postare a fost staersa din cauza continutului care nu respecta politica aplicatiei!";

                ViewBag.Warning = TempData["warning"];
            }


            string currentUserid = User.Identity.GetHashCode().ToString();
            var currentUser = db.Users.Find(currentUserid);
            var currentUserProfile = db.Profiles.Where(p => p.UserId == currentUserid).FirstOrDefault();

            var user = profile.User;

            if (currentUserProfile.SentFriends.Contains(user))
            {

                ViewBag.sentReq = true;

            }
            if (currentUserProfile.Friends.Contains(user))
            {
                ViewBag.Friend = true;
            }
            if (currentUserid == profile.UserId)
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

            Profile profile = new Profile();
            profile.UserId = User.Identity.GetHashCode().ToString();
            string uid = User.Identity.GetHashCode().ToString();


            var profiles = from p in db.Profiles
                           where p.UserId == uid
                           select p;

            ViewBag.NoProfile = profiles.Count();
            if (profiles.Count() == 0)
                return View(profile);
            else
                return RedirectToAction("Index", "Profiles");

        }






        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(Profile profile)
        {
            profile.SignUpDate = DateTime.Now;
            profile.UserId = User.Identity.GetHashCode().ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
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



        // ---------------------ai ramas la edit -----------------------------------------

        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile.UserId == User.Identity.GetHashCode().ToString() || User.IsInRole("Admin"))
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
        public async Task<ActionResult> EditAsync(int id, Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Profile profile = db.Profiles.Find(id);
                    if (profile.UserId == User.Identity.GetHashCode().ToString() || User.IsInRole("Admin"))
                    {
                        if ( await TryUpdateModelAsync(profile))
                        {
                            profile = requestProfile;
                            /*profile.ProfileName = requestProfile.ProfileName;
                            profile.ProfileDescription = requestProfile.ProfileDescription;
                            profile.SignUpDate = requestProfile.SignUpDate;
                            profile.PrivateProfile = requestProfile.PrivateProfile;*/
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



        //--------------------------------delete------------------------

        [HttpDelete]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile.UserId == User.Identity.GetHashCode().ToString() || User.IsInRole("Admin"))
            {
                db.Profiles.Remove(profile);
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



        //------------------------friend req--------------------------
        [Authorize(Roles = "User,Admin")]
        public ActionResult SendRequest(int id)
        {
            var profile = db.Profiles.Find(id);
            var user = profile.User;
            var currentUserId = User.Identity.GetHashCode().ToString();
            var prof = db.Profiles.Where(p => p.UserId == currentUserId);
            if (prof.Count() == 0)
            {
                TempData["friend"] = "Creeaza-ti un profil pentru a adauga prieteni!";
                return RedirectToAction("New");
            }
            if (currentUserId == profile.UserId)
            {
                return RedirectToAction("Index");
            }

            var currentUserProfile = prof.FirstOrDefault();
            var currentUser = db.Users.Find(currentUserId);

            if (currentUserProfile.SentFriends.Contains(user) || currentUserProfile.Friends.Contains(user))
            {
                return RedirectToAction("Index");
            }
            if (profile.SentFriends.Contains(currentUser))
            {
                return RedirectToAction("Index");
            }
            currentUserProfile.SentFriends.Add(user);
            profile.ReceivedFriends.Add(currentUser);
            db.SaveChanges();
            return RedirectToAction("Show/" + id.ToString());
        }






        [Authorize(Roles = "User,Admin")]
        public ActionResult FriendRequests(int id)
        {
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetHashCode().ToString())
            {
                return RedirectToAction("Index");
            }
            List<ApplicationUser> friendRequests = new List<ApplicationUser>();
            foreach (var user in profile.ReceivedFriends)
            {
                friendRequests.Add(user);
            }
            ViewBag.profileId = profile.ProfileId;
            ViewBag.FriendRequests = friendRequests;
            ViewBag.Length = friendRequests.Count();
            return View(profile);
        }






        [Authorize(Roles = "User,Admin")]
        public ActionResult AddFriend(int id, int id2)
        {
            
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetHashCode().ToString())
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
                    var userProfile = db.Profiles.Where(p => p.UserId == userId).FirstOrDefault();
                    userProfile.Friends.Add(profile.User);

                    Friend friendship = new Friend();
                    friendship.User1_Id = profile.UserId;
                    friendship.User1 = profile.User;
                    friendship.User2_Id = userId;
                    friendship.User2 = user;
                    db.Friends.Add(friendship);

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile.User);


                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }




        [Authorize(Roles = "User,Admin")]
        public ActionResult Friends(int id)
        {
            var profile = db.Profiles.Find(id);
            ViewBag.currentUser = User.Identity.GetHashCode().ToString();
            ViewBag.UserId = profile.UserId.ToString();
            /* ViewBag.friends = profile.Friends;*/
            var userId = profile.UserId;
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
            ViewBag.profileId = profile.ProfileId;
            return View(profile);
        }






        [Authorize(Roles = "User,Admin")]
        public ActionResult DeclineFriend(int id, int id2)
        {
            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetHashCode().ToString())
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
                    var userProfile = db.Profiles.Where(p => p.UserId == userId).FirstOrDefault();

                    profile.ReceivedFriends.Remove(user);
                    userProfile.SentFriends.Remove(profile.User);

                    db.SaveChanges();
                    break;
                }
            }
            return RedirectToAction("FriendRequests/" + id.ToString());
        }





        [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteFriend(int id, int id2)
        {

            var profile = db.Profiles.Find(id);
            if (profile.UserId != User.Identity.GetHashCode().ToString())
            {
                return RedirectToAction("Index");
            }
            var userId = profile.UserId;
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
                    var userProfile = db.Profiles.Where(p => p.UserId == user2Id).FirstOrDefault();

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
            var profile = db.Profiles.Find(id);
            var user = profile.User;
            ViewBag.joinedGroups = user.Groups;
            return View(profile);
        }








    }



}
