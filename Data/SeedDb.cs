using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSShopping.Data.Entities;

namespace TSShopping.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;

        public SeedDb(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCategoriesAsync();
            await CheckCountriesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if(!_context.Countries.Any())
            {
                _context.Countries.Add(new Country{
                    Name="Colombia",
                    States=new List<State>()
                    {
                        new State {
                            Name="Amazonas",
                            Cities=new List<City>()
                            {
                                new City{Name="El Encanto"},
                                new City{Name="La Chorrera"},
                                new City{Name="La Pedrera"},
                                new City{Name="La Victoria"},
                                new City{Name="Leticia"},
                                new City{Name="Mirití-Paraná"},
                                new City{Name="Puerto Alegria"},
                                new City{Name="Puerto Arica"},
                                new City{Name="Puerto Nariño"},
                                new City{Name="Puerto Santander"},
                                new City{Name="Tarapacá"}
                            }
                        },
                        new State{
                             Name="Antioquia",
                             Cities=new List<City>(){
                                new City{Name="Medellin"},
                                new City{Name="Bello"},
                                new City{Name="Itagui"},
                                new City{Name="envigado"},
                                new City{Name="Apartado"}
                             }
                        },
                        new State{
                            Name="Arauca",
                            Cities=new List<City>(){
                                new City{Name="Arauca"},
                                new City{Name="Arauquita"},
                                new City{Name="Cravo Norte"},
                                new City{Name="Fortul"},
                                new City{Name="Puerto Rondon"},
                                new City{Name="Saravena"},
                                new City{Name="Tame"}
                            }
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if(!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Tecnología" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Nutrición" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Apple" });

                await _context.SaveChangesAsync();
            }
        }
    }
}