using System.Text.Json.Serialization;
using Funifest.Infrastructure.Data;
using Funifest.Application.Services.Interfaces;
using Funifest.Application.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // to masz lub doda³eœ dla innych rzeczy,
        // mo¿emy tu dok³adaæ ustawienia JSON
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISkydiverService, SkydiverService>();
builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IParachuteService, ParachuteService>();
builder.Services.AddScoped<IExitPlanService, ExitPlanService>();

//builder.Services.AddDbContext<FunifestContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<FunifestContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Funifest.Infrastructure")
    ));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// W£¥CZ CORS
app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
