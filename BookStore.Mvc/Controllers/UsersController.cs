using Microsoft.AspNetCore.Mvc;

namespace BookStore.Mvc.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
