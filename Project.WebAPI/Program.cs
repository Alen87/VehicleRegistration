using Microsoft.EntityFrameworkCore;
using Ninject;
using Project.DAL;
using Project.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Konfiguracija Ninject-a
var kernel = new StandardKernel();
NinjectConfig.RegisterServices(kernel);

// Dodavanje DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VehicleDbContext>(options =>
    options.UseSqlServer(connectionString));

// Konfiguracija CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
