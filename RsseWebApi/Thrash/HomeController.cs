using Microsoft.AspNetCore.Mvc;

namespace RandomSongSearchEngine.Thrash
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
