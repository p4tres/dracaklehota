using DrDWebAPP.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Home/Profile";
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<DrDContext>(options => options.UseSqlServer("Data Source=.\\sqlexpress;Initial Catalog=DrDDatabase;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();


app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();


app.MapDefaultControllerRoute();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.Run();
