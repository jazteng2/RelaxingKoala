using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.ViewModels;

namespace RelaxingKoala.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(MySqlDataSource dataSource)
        {
            _userRepository = new UserRepository(dataSource);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.GetByEmail(model.Email);
                if (user != null && user.Password == model.Password)
                {
                    TempData["UserId"] = user.Id.ToString();
                    TempData["UserRole"] = user.GetType().Name;
                    return RedirectToAction("Index", "Home", user);
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            TempData.Remove("UserId");
            TempData.Remove("UserRole");
            return RedirectToAction("Login", "Account");
        }
    }
}