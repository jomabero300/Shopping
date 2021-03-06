using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TSShopping.Common;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Enum;
using TSShopping.Helpers;
using TSShopping.Models;
using Vereyon.Web;

namespace TSShopping.Controllers.Entities
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ApplicationDbContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public AccountController(IUserHelper userHelper,
                                 ApplicationDbContext context,
                                 ICombosHelper combosHelper, 
                                 IImageHelper imageHelper,
                                 IMailHelper mailHelper,
                                 IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _context = context;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
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

                if (result.IsLockedOut)
                {
                    _flashMessage.Danger("Ha superado el m??ximo n??mero de intentos, su cuenta est?? bloqueada, intente de nuevo en 5 minutos.");                    
                }
                else if(result.IsNotAllowed)
                {
                    _flashMessage.Danger("El usuario no ha sido habilitado, debes de seguir las instrucciones enviadas al correo para poder habilitarlo.");
                }
                else
                {
                    _flashMessage.Danger("Email o contrase??a incorrectos.");
                }
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
                    _flashMessage.Danger("Este correo ya est?? siendo usado.");
                    model.Countries = await _combosHelper.GetComboCountriesAsync();
                    model.States = await _combosHelper.GetComboStatesAsync(model.CountryId);
                    model.Cities = await _combosHelper.GetComboCitiesAsync(model.StateId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    model.Username,
                    "Shopping - Confirmaci??n de cuenta",
                    $"<h1>Shopping - Confirmaci??n de cuenta</h1>" +
                    $"Para habilitar el usuario por favor hacer click en el siguiente enlace:, " +
                    $"<hr/><br/><p><a href = \"{HtmlEncoder.Default.Encode(tokenLink)}\">Confirmar Email</a></p>");
//HtmlEncoder.Default.Encode()
                if (response.IsSuccess)
                {
                    // ViewBag.Message = "Usuario registrado. Para poder ingresar al sistema, para gabilitar el administrador siga las instrucciones que han sido enviadas a su correo.";
                    // return View(model);
                    _flashMessage.Info("Usuario registrado. Para poder ingresar al sistema, siga las instrucciones que han sido enviadas a su correo.");
                    return RedirectToAction(nameof(Login));
                }

                _flashMessage.Danger(response.Message);


                // LoginViewModel log=new LoginViewModel()
                // {
                //     Password=model.Password,
                //     Username=model.Username,
                //     RememberMe=false
                // };

                // var result2=await _userHelper.LoginAsync(log);
                // if(result2.IsSuccess)
                // {
                //     return RedirectToAction("Index","Home");
                // }

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
                            _flashMessage.Info("Su contrase??a ha sido cambiada con ??xito.");
                            return RedirectToAction("ChangeUser");
                        }
                        else
                        {
                            _flashMessage.Info(result.Errors.FirstOrDefault().Description);
                        }
                    }
                    else
                    {
                        _flashMessage.Info("User no found.");
                    }
                }
                else
                {
                    _flashMessage.Info("Debes ingresar una contrase??a diferente.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    _flashMessage.Danger("El email no corresponde a ning??n usuario registrado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(
                    model.Email, 
                    "Shopping - Recuperaci??n de Contrase??a", 
                    $"<h1>Shopping - Recuperaci??n de Contrase??a</h1>" +
                    $"Para recuperar la contrase??a haga click en el siguiente enlace:" +
                    $"<p><a href = \"{link}\">Reset Password</a></p>");
                // ViewBag.Message = "Las instrucciones para recuperar la contrase??a han sido enviadas a su correo.";
                // return View();
                _flashMessage.Info("Las instrucciones para recuperar la contrase??a han sido enviadas a su correo.");
                return RedirectToAction(nameof(Login));

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _flashMessage.Info("Contrase??a cambiada con ??xito.");
                    return RedirectToAction(nameof(Login));
                    // ViewBag.Message = "Contrase??a cambiada con ??xito.";
                    // return View();
                }
                // ViewBag.Message = "Error cambiando la contrase??a.";
                 _flashMessage.Danger("Error cambiando la contrase??a.");
            }
            else
            {
             _flashMessage.Danger("Usuario no encontrado.");
            }

            //ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }

    }
}