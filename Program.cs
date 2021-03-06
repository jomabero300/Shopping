using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TSShopping.Data;
using TSShopping.Data.Entities;
using TSShopping.Helpers;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//TODO: make strongest password
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    cfg.SignIn.RequireConfirmedEmail = true;
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    cfg.Lockout.MaxFailedAccessAttempts = 3;
    cfg.Lockout.AllowedForNewUsers = true;
})
.AddDefaultTokenProviders()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});

/*** Ciclo de vide de las inyecciones ***/

//Se Utiliza (AddTransient) una sola vez
builder.Services.AddTransient<SeedDb>();
builder.Services.AddFlashMessage();
//Se utiliza (AddScoped) cada vez que lo necesita y lo destrue cuando lo termine
builder.Services.AddScoped<IUserHelper,UserHelper>();
builder.Services.AddScoped<ICombosHelper,CombosHelper>();
builder.Services.AddScoped<IImageHelper,ImageHelper>();
builder.Services.AddScoped<IMailHelper,MailHelper>();
builder.Services.AddScoped<IOrdersHelper,OrdersHelper>();
builder.Services.AddScoped<IApiService,ApiService>();
//Las (AddSingleton) inyecta y nunca se destruyen
//builder.Services.AddSingleton<SeedDb>();

builder.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();

var app = builder.Build();

SeedData();

void SeedData()
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope scope = scopedFactory.CreateScope())
    {
        SeedDb service = scope.ServiceProvider.GetService<SeedDb>();
        service.SeedAsync().Wait();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
