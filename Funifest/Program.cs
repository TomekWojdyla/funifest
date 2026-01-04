using System.Text.Json.Serialization;
using Funifest.Infrastructure.Data;
using Funifest.Infrastructure.Seeder;
using Funifest.Application.Services.Interfaces;
using Funifest.Application.Services.Implementations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// CORS
// --------------------
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

// --------------------
// Controllers + JSON
// --------------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Enumy jako stringi w JSON (np. "Instructor" zamiast 4)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// Services (Application)
// --------------------
builder.Services.AddScoped<ISkydiverService, SkydiverService>();
builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IParachuteService, ParachuteService>();
builder.Services.AddScoped<IExitPlanService, ExitPlanService>();

// --------------------
// DbContext (SQLite)
// --------------------
builder.Services.AddDbContext<FunifestContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Funifest.Infrastructure")
    ));

// --------------------
// Build app
// --------------------
var app = builder.Build();

// --------------------
// MIGRATIONS + SEED
// --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FunifestContext>();

    // Upewniamy siê, ¿e schemat jest aktualny
    await db.Database.MigrateAsync();

    // Seed danych (tylko jeœli baza pusta)
    await FunifestDbSeeder.SeedAsync(db);
}

// --------------------
// Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
