using Microsoft.AspNetCore.Mvc;

namespace Twidder.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
