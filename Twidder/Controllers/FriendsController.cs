using Microsoft.AspNetCore.Mvc;

namespace Twidder.Controllers
{
    public class FriendsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
