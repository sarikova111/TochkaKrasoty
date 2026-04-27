using Microsoft.EntityFrameworkCore;
using TochkaKrasoty.Data;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddDbContext<SalonDbContext>(options =>
    options.UseSqlite("Data Source=salon.db"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SalonDbContext>();

    if (!context.Users.Any(u => u.Email == "admin@salon.ru"))
    {
        context.Users.Add(new TochkaKrasoty.Models.User
        {
            FullName = "Администратор",
            Email = "admin@salon.ru",
            Password = "12345",
            Role = "Admin",
            Phone = ""
        });

        context.SaveChanges();
    }
}

app.Run();