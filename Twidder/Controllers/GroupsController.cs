﻿using Microsoft.AspNetCore.Mvc;

namespace Twidder.Controllers
{
    public class GroupsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
