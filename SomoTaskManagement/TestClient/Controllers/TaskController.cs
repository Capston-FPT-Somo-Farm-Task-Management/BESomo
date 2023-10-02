using Microsoft.AspNetCore.Mvc;

namespace TestClient.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
