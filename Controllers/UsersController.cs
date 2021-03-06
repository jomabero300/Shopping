using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

namespace TSShopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly ApplicationDbContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public UsersController(IUserHelper userHelper,
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                                    .Include(u=>u.City)
                                    .ThenInclude(c=>c.State)
                                    .ThenInclude(s=>s.Country)
                                    .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new AddUserViewModel()
            {
                Id = Guid.Empty.ToString(),
                Countries = await _combosHelper.GetComboCountriesAsync(),
                States = await _combosHelper.GetComboStatesAsync(0),
                Cities = await _combosHelper.GetComboCitiesAsync(0),
                UserType = UserType.Admin,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
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
                    "Shopping - Confirmaci??n de Email",
                    $"<h1>Shopping - Confirmaci??n de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<hr/><br/><p><a href = \"{tokenLink}\">Confirmar Email</a></p>");

                if (response.IsSuccess)
                {
                    //ViewBag.Message = "Las instrucciones para habilitar el administrador han sido enviadas al correo.";
                    _flashMessage.Danger("Las instrucciones para habilitar el administrador han sido enviadas al correo.");
                    return View(model);
                }
                _flashMessage.Danger(response.Message);                
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
    }
}