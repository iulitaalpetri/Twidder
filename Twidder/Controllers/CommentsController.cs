using Microsoft.AspNetCore.Mvc;

namespace Twidder.Controllers
{
    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
            
        }
    }
}
