using Microsoft.AspNetCore.Mvc;

namespace Web_Book.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddNews()
        {
            return View();
        } 
    }
}
