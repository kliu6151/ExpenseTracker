using Expense_Tracker.Models;
using Expense_Tracker.Models.Identity;
using Expense_Tracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddRazorPages();

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
*/

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    options.SignIn.RequireConfirmedAccount = true; // Include the SignIn.RequireConfirmedAccount setting

    // Other Identity configurations...
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Configure Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Expense_Tracker.Cookie";
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


builder.Services.AddControllers(options =>
{
    // Configure Newtonsoft.Json settings
    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

    options.ReturnHttpNotAcceptable = true;

})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});



builder.Services.AddScoped<IUserService, UserService>();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF1cWmhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEZiW31XcXBRTmNfVUZ2Vg==");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
