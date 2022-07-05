using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Enum;
using TSShopping.Helpers;
using TSShopping.Models;

namespace TSShopping.Controllers.Entities
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ApplicationDbContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(IUserHelper userHelper,ApplicationDbContext context,ICombosHelper combosHelper, IImageHelper imageHelper)
        {
            _userHelper = userHelper;
            _context = context;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
        }
        // GET: Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password,RememberMe")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult NotAuthorized()
        {
            return View();
        }
        public async Task<IActionResult> Register()
        {
            AddUserViewModel model = new AddUserViewModel()
            {
                Id = Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.User,
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile,"images/users");
                }

                model.ImageId = imageId;
                User user = await _userHelper.AddUserAsync(model);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty,"Este correo ya está siendo usado");

                //     _flashMessage.Danger("Este correo ya está siendo usado.");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

                // string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                // string tokenLink = Url.Action("ConfirmEmail", "Account", new
                // {
                //     userid = user.Id,
                //     token = myToken
                // }, protocol: HttpContext.Request.Scheme);

                // Response response = _mailHelper.SendMail(
                //     $"{model.FirstName} {model.LastName}",
                //     model.Username,
                //     "Shopping - Confirmación de Email",
                //     $"<h1>Shopping - Confirmación de Email</h1>" +
                //         $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                //         $"<hr/><br/><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                // if (response.IsSuccess)
                // {
                //     _flashMessage.Info("Usuario registrado. Para poder ingresar al sistema, siga las instrucciones que han sido enviadas a su correo.");
                //     return RedirectToAction(nameof(Login));
                // }

                // ModelState.AddModelError(string.Empty, response.Message);


                LoginViewModel log=new LoginViewModel()
                {
                    Password=model.Password,
                    Username=model.Username,
                    RememberMe=false
                };

                var result2=await _userHelper.LoginAsync(log);
                if(result2.Succeeded)
                {
                    return RedirectToAction("Index","Home");
                }

            }

            model.Countries = await _combosHelper.GetComboCountriesAsync();

            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);

            return View(model);
        }
        public JsonResult GetStates(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.States)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.States.OrderBy(d => d.Name));
        }
        public JsonResult GetCities(int stateId)
        {
            State state = _context.States
                .Include(s => s.Cities)
                .FirstOrDefault(s => s.Id == stateId);
            if (state == null)
            {
                return null;
            }

            return Json(state.Cities.OrderBy(c => c.Name));
        }
        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new()
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                Cities = await _combosHelper.GetComboCitiesAsync(user.City.State.Id),
                CityId = user.City.Id,
                Countries = await _combosHelper.GetComboCountriesAsync(),
                CountryId = user.City.State.Country.Id,
                States = await _combosHelper.GetComboStatesAsync(user.City.State.Country.Id),
                StateId = user.City.State.Id,
                Id = user.Id,
                Document = user.Document
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;

                if (model.ImageFile != null)
                {
                    imageId = await _imageHelper.UploadImageAsync(model.ImageFile,"images/users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.ImageId = imageId;
                user.City = await _context.Cities.FindAsync(model.CityId);
                user.Document = model.Document;

                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            model.Countries = await _combosHelper.GetComboCountriesAsync();
            model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
            model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.OldPassword != model.NewPassword)
                {
                    User user = await _userHelper.GetUserAsync(User.Identity.Name);
                    if (user != null)
                    {
                        IdentityResult result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("ChangeUser");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "User no found.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Debes ingresar una contraseña diferente.");
                }
            }

            return View(model);
        }

    }
}