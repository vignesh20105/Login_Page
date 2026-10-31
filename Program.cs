
using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Register SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register password hasher
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

var app = builder.Build();

app.MapControllers();

app.Run();
