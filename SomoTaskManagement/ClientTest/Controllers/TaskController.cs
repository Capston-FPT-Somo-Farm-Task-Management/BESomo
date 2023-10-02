using Microsoft.AspNetCore.Mvc;

namespace UITest.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
