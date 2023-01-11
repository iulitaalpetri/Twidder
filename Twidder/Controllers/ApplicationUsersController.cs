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


        //public ActionResult Index()
        //{
        //    // daca user ul nu are profil, trebuie sa apara butonul cu adauga profil
        //    string uid = _userManager.GetUserId(User);




        //    var pr = from p in db.Users where p.Id == uid select p;

        //    ViewBag.existsProfile = pr.Count();


        //    var search = "";



        //    // pt a cauta profilul, se va cauta dupa nume

        //    if (Convert.ToString(HttpContext.Request.Query["search"]) == null) return View();
        //    search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
        //    List<string> idProfile = db.Users.Where(p => p.FirstName.Contains(search)).Select(pr => pr.FirstName).ToList();
        //    var  profile = db.Users.Where(pr => idProfile.Contains(pr.Id));
        //    //----------------------------------------------------------------------------------------------





        //    var nr = profile.Count();
        //    var currentPage = Convert.ToInt32(Convert.ToString(HttpContext.Request.Query["page"]));
        //    var offset = 0;

        //    if (!currentPage.Equals(0))
        //    {
        //        offset = (currentPage - 1) * this._perpage;
        //    }


        //    var paginatedProfiles = profile.Skip(offset).Take(this._perpage);

        //    if (TempData.ContainsKey("message"))
        //    {
        //        ViewBag.Message = TempData["message"];
        //    }

        //    ViewBag.total = nr;
        //    ViewBag.lastpage = Math.Ceiling((float)nr / (float)this._perpage);
        //    ViewBag.Profiles = paginatedProfiles;
        //    ViewBag.searchstring = search;
        //    return View();

        //}

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

            //ViewBag.sentReq = currentUser.SentFriends.Contains(targetUser);
            //ViewBag.friend = currentUser.Friends.Contains(targetUser);
            //ViewBag.nobutton = targetUser.SentFriends.Contains(currentUser);
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
                return RedirectToAction("Index", "Users");

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

        

    }
}
