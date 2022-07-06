using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSShopping.Data.Entities;
using TSShopping.Enum;
using TSShopping.Helpers;

namespace TSShopping.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(ApplicationDbContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCategoriesAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Manuel", "Bello", "jomabero300@yopmail.com", "313 367 0740", "Calle Luna Calle Sol", UserType.Admin);
            await CheckUserAsync("2020", "Marcos", "Suarez", "marcos301@yopmail.com", "318 210 8658", "Calle 20 nO 32-63", UserType.User);
        }

        private async Task<User> CheckUserAsync(    
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            User user=await _userHelper.GetUserAsync(email);
            if(user==null)
            {
                user=new User(){
                    UserName=email,
                    Document=document,
                    FirstName=firstName,
                    LastName=lastName,
                    Email=email,
                    PhoneNumber=phone,
                    Address=address,
                    UserType=userType,
                    City=_context.Cities.FirstOrDefault()
                };

                await _userHelper.AddUserAsync(user,"123456");

                await _userHelper.AddUserToRoleAsync(user,userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
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