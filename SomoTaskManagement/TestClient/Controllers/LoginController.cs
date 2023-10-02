using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using TestClient.Repos;

namespace TestClient.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepo userRepo;

        public LoginController(UserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password)
        {
            var userFromDb = await userRepo.GetUserDetails(username, password);

            if (userFromDb == null)
            {
                ModelState.AddModelError("Login", "Invalid credentials");
                return View();
            }

            HttpContext.Session.SetString("UserName", userFromDb.Id.ToString());

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            return RedirectToAction(nameof(SignIn));
        }
    }
}
