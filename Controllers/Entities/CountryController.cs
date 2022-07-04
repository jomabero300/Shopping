using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Models;

namespace TSShopping.Controllers.Entities
{
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Country
        public async Task<IActionResult> Index()
        {
              return View(await _context.Countries.Include(x=>x.States).ToListAsync());
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

        // GET: Country/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));                    
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(country);
        }

        // GET: Country/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Country/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); 
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un país con el mismo nombre.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(country);
        }

        // GET: Country/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

            Country country = await _context.Countries.Include(x=>x.States)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Countries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Countries'  is null.");
            }

            Country country = await _context.Countries.FindAsync(id);
            
            if (country != null)
            {
                _context.Countries.Remove(country);
            }
                        
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddState(int? id)
        {
            if (id == null || _context.Countries == null)
            {
                return NotFound();
            }

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

                _context.Add(state);
                
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),new {id=model.CountryId});                    
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un departamento/estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);
        }

        // GET: Country/EditState/5
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
        public async Task<IActionResult> EditState(int id, StateViewModel state)
        {
            if (id != state.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    State model=new State()
                    {
                        Id=state.Id,
                        Name=state.Name,

                    };
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new{id=state.CountryId});
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un Departamento/estado con el mismo nombre en este país.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(state);
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
            if (id == null || _context.States == null)
            {
                return NotFound();
            }

            State state = await _context.States.Include(s=>s.Country)
                        .FirstOrDefaultAsync(s=>s.Id==id);

            if (state == null)
            {
                return NotFound();
            }

            return View(state);
        }

        // POST: Country/DeleteState/5
        [HttpPost, ActionName("DeleteState")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStateConfirmed(int id)
        {
            if (_context.States == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Countries'  is null.");
            }

            State state = await _context.States.Include(s=>s.Country).FirstOrDefaultAsync(s=>s.Id==id);
            
            if (state != null)
            {
                _context.States.Remove(state);
            }
                        
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details),new {id=state.Country.Id});
        }

        // GET: Country/AddCity/5
        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null || _context.States == null)
            {
                return NotFound();
            }

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
                City city=new City()
                {
                    Name=model.Name,
                    State=await _context.States.FirstOrDefaultAsync(s=>s.Id==model.StateId)
                };

                _context.Add(city);
                
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState),new {id = model.StateId});                    
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe una cidad con el mismo nombre en este Departamento/Estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(model);
        }

        // GET: Country/EditCity/5
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
        public async Task<IActionResult> EditCity(int id, CityViewModel city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                City model=new City()
                {
                    Id=city.Id,
                    Name=city.Name,
                };
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(DetailsState), new{id=city.StateId});
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplica"))
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un ciudad con el mismo nombre en este Departamento/estado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, dbUpdateException.InnerException.Message);
                    }
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return View(city);
        }
        
        // GET: Country/DetailsCity/5
        public async Task<IActionResult> DetailsCity(int? id)
        {
            if (id == null || _context.States == null)
            {
                return NotFound();
            }

            City model = await _context.Cities
                .Include(c=>c.State)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Country/DeleteCity/5
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null || _context.Cities == null)
            {
                return NotFound();
            }

            City city = await _context.Cities.Include(x=>x.State)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Country/DeleteCity/5
        [HttpPost, ActionName("DeleteCity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCityConfirmed(int id)
        {
            if (_context.Cities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Countries'  is null.");
            }

            City city = await _context.Cities.Include(c=>c.State).FirstOrDefaultAsync(c=>c.Id== id);
            
            if (city != null)
            {
                _context.Cities.Remove(city);
            }
                        
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index),new {id=city.State.Id});
        }

    }
}
