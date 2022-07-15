using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Helpers;
using TSShopping.Models;
using Vereyon.Web;
using static TSShopping.Helpers.ModalHelper;

namespace TSShopping.Controllers.Entities
{
    [Authorize(Roles = "Admin")]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFlashMessage _flashMessage;

        public CountryController(ApplicationDbContext context,
                                IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        // GET: Country
        public async Task<IActionResult> Index()
        {
              return View(await _context.Countries
                                .Include(x=>x.States)
                                .ThenInclude(s=>s.Cities)
                                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el país porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Index));
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Country());
            }
            else
            {
                Country country = await _context.Countries.FindAsync(id);
                if (country == null)
                {
                    return NotFound();
                }

                return View(country);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0) //Insert
                    {
                        _context.Add(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado.");
                    }
                    else //Update
                    {
                        _context.Update(country);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado.");
                    }
                    return Json(new 
                    { 
                        isValid = true, 
                        html = ModalHelper.RenderRazorViewToString(
                            this, 
                            "_ViewAll", 
                            _context.Countries
                                .Include(c => c.States)
                                .ThenInclude(s => s.Cities)
                                .ToList()) 
                    });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        _flashMessage.Danger("Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", country) });
        }

        // GET: Country/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            Country model = await _context.Countries
                .Include(c=>c.States)
                .ThenInclude(c=>c.Cities)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        
        [NoDirectAccess]
        public async Task<IActionResult> AddState(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            StateViewModel model=new StateViewModel(){
                CountryId=country.Id
            };



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddState([Bind("Id,Name,CountryId")] StateViewModel model)
        {
            if (ModelState.IsValid)
            {
                State state=new State()
                {
                    Cities=new List<City>(),
                    Country=await _context.Countries.FindAsync(model.CountryId),
                    Name=model.Name
                };

                
                try
                {
                    _context.Add(state);
                    await _context.SaveChangesAsync();
                    Country country = await _context.Countries
                        .Include(c => c.States)
                        .ThenInclude(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    _flashMessage.Info("Registro creado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        _flashMessage.Danger("Ya existe un departamento/estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddState", model) });
        }

        // GET: Country/EditState/5
        [NoDirectAccess]
        public async Task<IActionResult> EditState(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            State state = await _context.States
                                        .Include(x=>x.Country)
                                        .FirstOrDefaultAsync(x=>x.Id==id);
            
            if (state == null)
            {
                return NotFound();
            }

            StateViewModel model=new StateViewModel()
            {
                CountryId=state.Country.Id,
                Id=state.Id,
                Name=state.Name
            }; 

            return View(model);
        }

        // POST: Country/EditState/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditState(int id, StateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    State state=new(){
                        Id=model.Id,
                        Name=model.Name,
                    };

                    _context.Update(state);
                    await _context.SaveChangesAsync();

                    Country country = await _context.Countries
                        .Include(c => c.States)
                        .ThenInclude(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.CountryId);
                    _flashMessage.Confirmation("Registro actualizado");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllStates", country) });                        

                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        _flashMessage.Danger("Ya existe un Departamento/estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditState", model) });
        }

        // GET: Country/DetailsState/5
        public async Task<IActionResult> DetailsState(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            State model = await _context.States
                .Include(s=>s.Country)
                .Include(s=>s.Cities)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Country/DeleteState/5
        public async Task<IActionResult> DeleteState(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            State state = await _context.States
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (state == null)
            {
                return NotFound();
            }

            try
            {
                _context.States.Remove(state);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el estado / departamento porque tiene registros relacionados.");
            }

            return RedirectToAction(nameof(Details), new { Id = state.Country.Id });
        }

        // GET: Country/AddCity/5
        [NoDirectAccess]
        public async Task<IActionResult> AddCity(int id)
        {
            State state = await _context.States.FindAsync(id);

            if (state == null)
            {
                return NotFound();
            }

            CityViewModel model=new CityViewModel(){
                StateId=state.Id
            };

            return View(model);
        }
        
        // POST: Country/AddCity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity([Bind("Id,Name,StateId")] CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                State state = await _context.States.FindAsync(model.StateId);
                City city=new City()
                {
                    Name=model.Name,
                    State=state
                };
                
                try
                {
                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    state = await _context.States
                        .Include(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        _flashMessage.Danger("Ya existe una cidad con el mismo nombre en este Departamento/Estado.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddCity", model) });
        }

        // GET: Country/EditCity/5
        [NoDirectAccess]
        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null || _context.States == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                                    .Include(c=>c.State)
                                    .FirstOrDefaultAsync(c=>c.Id==id);
            
            if (city == null)
            {
                return NotFound();
            }

            CityViewModel model=new CityViewModel()
            {
                StateId=city.State.Id,
                Id=city.Id,
                Name=city.Name
            }; 

            return View(model);
        }

        // POST: Country/EditCity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCity(int id, CityViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                City city=new City()
                {
                    Id=model.Id,
                    Name=model.Name,
                };
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    State state = await _context.States
                        .Include(s => s.Cities)
                        .FirstOrDefaultAsync(c => c.Id == model.StateId);
                    _flashMessage.Confirmation("Registro actualizado.");
                    return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "_ViewAllCities", state) });
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        _flashMessage.Danger("Ya existe un ciudad con el mismo nombre en este Departamento/estado.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                }
            }
            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "EditCity", model) });
        }
        
        // GET: Country/DeleteCity/5
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            City city = await _context.Cities
                            .Include(x=>x.State)
                            .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            try
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            catch
            {
                _flashMessage.Danger("No se puede borrar la ciudad porque tiene registros relacionados.");
            }

            _flashMessage.Info("Registro borrado.");

            return RedirectToAction(nameof(DetailsState), new { Id = city.State.Id });
        }
    }
}
