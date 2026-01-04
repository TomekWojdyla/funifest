using Funifest.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Funifest.Infrastructure.Data;

public class FunifestContext : DbContext
{
    public FunifestContext(DbContextOptions<FunifestContext> options) : base(options)
    {
    }

    public DbSet<Skydiver> Skydivers => Set<Skydiver>();
    public DbSet<Passenger> Passengers => Set<Passenger>();
    public DbSet<Parachute> Parachutes => Set<Parachute>();
    public DbSet<ExitPlan> ExitPlans => Set<ExitPlan>();
    public DbSet<ExitSlot> ExitSlots => Set<ExitSlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Skydiver -> Parachute (opcjonalnie: Student może nie mieć przypisanego)
        modelBuilder.Entity<Skydiver>()
            .HasOne<Parachute>()
            .WithMany()
            .HasForeignKey(s => s.ParachuteId)
            .OnDelete(DeleteBehavior.SetNull);

        // ExitPlan 1 - * ExitSlot
        modelBuilder.Entity<ExitSlot>()
            .HasOne(s => s.ExitPlan)
            .WithMany(p => p.Slots)
            .HasForeignKey(s => s.ExitPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // ExitSlot -> Parachute (wiele slotów może używać tego samego spadochronu)
        modelBuilder.Entity<ExitSlot>()
            .HasOne(s => s.Parachute)
            .WithMany()
            .HasForeignKey(s => s.ParachuteId);
    }
}
