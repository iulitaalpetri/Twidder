﻿using Microsoft.AspNetCore.Mvc;
using Twidder.Data;
using Twidder.Models;

namespace Twidder.Controllers
{

   
    public class GroupsController : Controller
    {

        private readonly ApplicationDbContext db;

        public GroupsController(ApplicationDbContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
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

        [HttpPost]
        public ActionResult New(Group grp)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(grp);
                db.SaveChanges();
                TempData["message"] = "Grupul a fost adaugat";
                return RedirectToAction("Index");
            }

            else
            {
                return View(grp);
            }
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

        
    }
}
