using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using RelaxingKoala.Data;
using RelaxingKoala.Models.Users;
using RelaxingKoala.Models.ViewModels;
using Newtonsoft.Json;

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
                User user = _userRepository.GetByEmail(model.Email);
                if (user != null && user.Password == model.Password)
                {
                    user.Password = string.Empty;
                    TempData["User"] = JsonConvert.SerializeObject(user);
                    TempData["UserRole"] = user.Role;
                    return RedirectToAction("Index", "Home", TempData);
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

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Customer
                {
                    Id = Guid.NewGuid(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = UserRole.Customer // Assuming customers are being registered
                };
                _userRepository.Insert(user);
                return RedirectToAction("Login");
            }
            return View(model);
        }
    }
}
