using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TSShopping.Common;
using TSShopping.Data.Entities;
using TSShopping.Enum;
using TSShopping.Helpers;

namespace TSShopping.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IApiService _apiService;

        public SeedDb(ApplicationDbContext context, IUserHelper userHelper,IImageHelper imageHelper, IApiService apiService)
        {
            _context = context;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _apiService = apiService;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCategoriesAsync();
            await CheckProductsAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Manuel", "Bello", "jomabero300@yopmail.com", "313 367 0740", "Calle Luna Calle Sol","" ,UserType.Admin);
            await CheckUserAsync("2020", "Marcos", "Suarez", "marcos301@yopmail.com", "318 210 8658", "Calle 20 nO 32-63", "", UserType.User);
            await CheckUserAsync("2020", "Ledys", "Bedoya", "ledys@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "LedysBedoya.jpeg", UserType.User);
            await CheckUserAsync("3030", "Brad", "Pitt", "brad@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Brad.jpg", UserType.User);
            await CheckUserAsync("4040", "Angelina", "Jolie", "angelina@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Angelina.jpg", UserType.User);

        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "adidas_barracuda.png" });
                await AddProductAsync("Adidas Superstar", 250000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "Adidas_superstar.png" });
                await AddProductAsync("AirPods", 1300000M, 12F, new List<string>() { "Tecnolog??a", "Apple" }, new List<string>() { "airpos.png", "airpos2.png" });
                await AddProductAsync("Audifonos Bose", 870000M, 12F, new List<string>() { "Tecnolog??a" }, new List<string>() { "audifonos_bose.png" });
                await AddProductAsync("Bicicleta Ribble", 12000000M, 6F, new List<string>() { "Deportes" }, new List<string>() { "bicicleta_ribble.png" });
                await AddProductAsync("Camisa Cuadros", 56000M, 24F, new List<string>() { "Ropa" }, new List<string>() { "camisa_cuadros.png" });
                await AddProductAsync("Casco Bicicleta", 820000M, 12F, new List<string>() { "Deportes" }, new List<string>() { "casco_bicicleta.png", "casco.png" });
                await AddProductAsync("iPad", 2300000M, 6F, new List<string>() { "Tecnolog??a", "Apple" }, new List<string>() { "ipad.png" });
                await AddProductAsync("iPhone 13", 5200000M, 6F, new List<string>() { "Tecnolog??a", "Apple" }, new List<string>() { "iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png" });
                await AddProductAsync("Mac Book Pro", 12100000M, 6F, new List<string>() { "Tecnolog??a", "Apple" }, new List<string>() { "mac_book_pro.png" });
                await AddProductAsync("Mancuernas", 370000M, 12F, new List<string>() { "Deportes" }, new List<string>() { "mancuernas.png" });
                await AddProductAsync("Mascarilla Cara", 26000M, 100F, new List<string>() { "Belleza" }, new List<string>() { "mascarilla_cara.png" });
                await AddProductAsync("New Balance 530", 180000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "newbalance530.png" });
                await AddProductAsync("New Balance 565", 179000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "newbalance565.png" });
                await AddProductAsync("Nike Air", 233000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "nike_air.png" });
                await AddProductAsync("Nike Zoom", 249900M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "nike_zoom.png" });
                await AddProductAsync("Buso Adidas Mujer", 134000M, 12F, new List<string>() { "Ropa", "Deportes" }, new List<string>() { "buso_adidas.png" });
                await AddProductAsync("Suplemento Boots Original", 15600M, 12F, new List<string>() { "Nutrici??n" }, new List<string>() { "Boost_Original.png" });
                await AddProductAsync("Whey Protein", 252000M, 12F, new List<string>() { "Nutrici??n" }, new List<string>() { "whey_protein.png" });
                await AddProductAsync("Arnes Mascota", 25000M, 12F, new List<string>() { "Mascotas" }, new List<string>() { "arnes_mascota.png" });
                await AddProductAsync("Cama Mascota", 99000M, 12F, new List<string>() { "Mascotas" }, new List<string>() { "cama_mascota.png" });
                await AddProductAsync("Teclado Gamer", 67000M, 12F, new List<string>() { "Gamer", "Tecnolog??a" }, new List<string>() { "teclado_gamer.png" });
                await AddProductAsync("Silla Gamer", 980000M, 12F, new List<string>() { "Gamer", "Tecnolog??a" }, new List<string>() { "silla_gamer.png" });
                await AddProductAsync("Mouse Gamer", 132000M, 12F, new List<string>() { "Gamer", "Tecnolog??a" }, new List<string>() { "mouse_gamer.png" });
                await _context.SaveChangesAsync();
            }

        }

        private async Task<User> CheckUserAsync(    
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string image,
            UserType userType)
        {
            User user=await _userHelper.GetUserAsync(email);
            if(user==null)
            {
                Guid imageId = Guid.Empty;
                if(image != "")
                {
                    string filepath = $"{Environment.CurrentDirectory}\\wwwroot\\images\\imagesTemp\\users\\{image}";

                    FileStream stream = System.IO.File.OpenRead(filepath);

                    IFormFileTemp model=new IFormFileTemp {ImagenIFF=new FormFile(stream,0,stream.Length, null, Path.GetFileName(stream.Name))};
                    
                    imageId = await _imageHelper.UploadImageAsync(model.ImagenIFF, "images/users");

                    //imageId=await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");
                }
                user=new User(){
                    UserName=email,
                    Document=document,
                    FirstName=firstName,
                    LastName=lastName,
                    Email=email,
                    PhoneNumber=phone,
                    Address=address,
                    ImageId = imageId,
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
            if (!_context.Countries.Any())
            {
                Response responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");
                if (responseCountries.IsSuccess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result;
                    foreach (CountryResponse countryResponse in countries)
                    {
                        if (countryResponse.Name == "Argentina" || 
                            countryResponse.Name == "Bolivia" || 
                            countryResponse.Name == "Brasil" || 
                            countryResponse.Name == "Chile" || 
                            countryResponse.Name == "Colombia" ||
                            countryResponse.Name == "Ecuador" || 
                            countryResponse.Name == "Guayana Francesa" || 
                            countryResponse.Name == "Guayanas" || 
                            countryResponse.Name == "Paraguay" || 
                            countryResponse.Name == "Peru" || 
                            countryResponse.Name == "Surinan" || 
                            countryResponse.Name == "Uruguay" || 
                            countryResponse.Name == "Venezuela")
                        {
                            Country country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == countryResponse.Name);
                            if (country == null)
                            {
                                country = new() { Name = countryResponse.Name, States = new List<State>() };    
                                Response responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");
                                if (responseStates.IsSuccess)
                                {
                                    List<StateResponse> states = (List<StateResponse>)responseStates.Result;
                                    foreach (StateResponse stateResponse in states)
                                    {
                                        State state = country.States.FirstOrDefault(s => s.Name == stateResponse.Name);
                                        if (state == null)
                                        {
                                            state = new() { Name = stateResponse.Name, Cities = new List<City>() };
                                            Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");
                                            if (responseCities.IsSuccess)
                                            {
                                                List<CityResponse> cities = (List<CityResponse>)responseCities.Result;
                                                foreach (CityResponse cityResponse in cities)
                                                {
                                                    if (cityResponse.Name == "Mosfellsb??r" || cityResponse.Name == "????uli??a")
                                                    {
                                                        continue;
                                                    }
                                                    City city = state.Cities.FirstOrDefault(c => c.Name == cityResponse.Name);
                                                    if (city == null)
                                                    {
                                                        state.Cities.Add(new City() { Name = cityResponse.Name });
                                                    }
                                                }
                                            }
                                            if (state.CityNumber > 0)
                                            {
                                                country.States.Add(state);
                                            }
                                        }
                                    }
                                }
                                if (country.CitiesNumber > 0)
                                {
                                    _context.Countries.Add(country);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }



                // _context.Countries.Add(new Country
                // {
                //     Name = "Colombia",
                //     States = new List<State>()
                //     {
                //         new State()
                //         {
                //             Name = "Antioquia",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Medell??n" },
                //                 new City() { Name = "Itag????" },
                //                 new City() { Name = "Envigado" },
                //                 new City() { Name = "Bello" },
                //                 new City() { Name = "Sabaneta" },
                //                 new City() { Name = "La Ceja" },
                //                 new City() { Name = "La Union" },
                //                 new City() { Name = "La Estrella" },
                //                 new City() { Name = "Copacabana" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "Bogot??",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Usaquen" },
                //                 new City() { Name = "Champinero" },
                //                 new City() { Name = "Santa fe" },
                //                 new City() { Name = "Usme" },
                //                 new City() { Name = "Bosa" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "Valle",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Cal??" },
                //                 new City() { Name = "Jumbo" },
                //                 new City() { Name = "Jamund??" },
                //                 new City() { Name = "Chipichape" },
                //                 new City() { Name = "Buenaventura" },
                //                 new City() { Name = "Cartago" },
                //                 new City() { Name = "Buga" },
                //                 new City() { Name = "Palmira" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "Santander",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Bucaramanga" },
                //                 new City() { Name = "M??laga" },
                //                 new City() { Name = "Barrancabermeja" },
                //                 new City() { Name = "Rionegro" },
                //                 new City() { Name = "Barichara" },
                //                 new City() { Name = "Zapatoca" },
                //             }
                //         },
                //     }
                // });
                // _context.Countries.Add(new Country
                // {
                //     Name = "Estados Unidos",
                //     States = new List<State>()
                //     {
                //         new State()
                //         {
                //             Name = "Florida",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Orlando" },
                //                 new City() { Name = "Miami" },
                //                 new City() { Name = "Tampa" },
                //                 new City() { Name = "Fort Lauderdale" },
                //                 new City() { Name = "Key West" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "Texas",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Houston" },
                //                 new City() { Name = "San Antonio" },
                //                 new City() { Name = "Dallas" },
                //                 new City() { Name = "Austin" },
                //                 new City() { Name = "El Paso" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "California",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Los Angeles" },
                //                 new City() { Name = "San Francisco" },
                //                 new City() { Name = "San Diego" },
                //                 new City() { Name = "San Bruno" },
                //                 new City() { Name = "Sacramento" },
                //                 new City() { Name = "Fresno" },
                //             }
                //         },
                //     }
                // });
                // _context.Countries.Add(new Country
                // {
                //     Name = "Ecuador",
                //     States = new List<State>()
                //     {
                //         new State()
                //         {
                //             Name = "Pichincha",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Quito" },
                //             }
                //         },
                //         new State()
                //         {
                //             Name = "Esmeraldas",
                //             Cities = new List<City>() {
                //                 new City() { Name = "Esmeraldas" },
                //             }
                //         },
                //     }
                // });
            }
            
            // await _context.SaveChangesAsync();
        }

        private async Task CheckCategoriesAsync()
        {
            if(!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Tecnolog??a" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Nutrici??n" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Apple" });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }            

            foreach (string image in images)
            {
                string filepath = $"{Environment.CurrentDirectory}\\wwwroot\\images\\imagesTemp\\products\\{image}";

                FileStream stream = System.IO.File.OpenRead(filepath);

                IFormFileTemp model=new IFormFileTemp {ImagenIFF=new FormFile(stream,0,stream.Length, null, Path.GetFileName(stream.Name))};
                
                Guid imageId = await _imageHelper.UploadImageAsync(model.ImagenIFF, "images/products");
                
                prodcut.ProductImages.Add(new ProductImage { ImageId = imageId });
            }

            _context.Products.Add(prodcut);
        }


    }

    public class IFormFileTemp
    {
        public IFormFile ImagenIFF { get; set; }
    }
}