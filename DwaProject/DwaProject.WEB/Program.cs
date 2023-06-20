using DwaProject.BL.DALModels;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<RwaMoviesContext>(options =>
{
	options.UseSqlServer("Name=ConnectionStrings:MovieConnStr");
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services
				.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


//DI for repositories
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/StatusCode/500");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

//for error 500
app.UseExceptionHandler("/StatusCode/500");
app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
