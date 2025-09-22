using DrDWebAPP.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + auth + session
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Home/Profile";
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<DrDContext>(options =>
    options.UseSqlServer("Data Source=.\\sqlexpress;Initial Catalog=DrDDatabase;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//* MICHAL
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DrDWebAPP.Data.DrDContext>();
    db.Database.EnsureCreated(); // ak DB ešte neexistuje, vytvorí ju aj s tabu¾kami
}
//*

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

//najprv areas, potom default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Mirror}/{action=Index}/{id?}");

app.MapDefaultControllerRoute();

app.Run();
