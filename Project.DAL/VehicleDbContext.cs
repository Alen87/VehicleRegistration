using Microsoft.EntityFrameworkCore;
using Project.DAL.Entities;


namespace Project.DAL;

/// <summary>
/// DbContext klasa koja upravlja pristupom bazi podataka
/// </summary>
public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
    {
    }

    public DbSet<VehicleMake> Makes { get; set; } = null!;
    public DbSet<VehicleModel> Models { get; set; } = null!;
    public DbSet<VehicleEngineType> EngineTypes { get; set; } = null!;
    public DbSet<VehicleOwner> Owners { get; set; } = null!;
    public DbSet<VehicleModelEngineType> ModelEngineTypes { get; set; } = null!;
    public DbSet<VehicleRegistration> Registrations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // VehicleMake - VehicleModel (1:N)
        modelBuilder.Entity<VehicleMake>()
            .HasMany(m => m.Models)
            .WithOne(m => m.Make)
            .HasForeignKey(m => m.MakeId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleModel - VehicleModelEngineType (1:N)
        modelBuilder.Entity<VehicleModel>()
            .HasMany(m => m.ModelEngineTypes)
            .WithOne(met => met.Model)
            .HasForeignKey(met => met.ModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleEngineType - VehicleModelEngineType (1:N)
        modelBuilder.Entity<VehicleEngineType>()
            .HasMany(et => et.ModelEngineTypes)
            .WithOne(met => met.EngineType)
            .HasForeignKey(met => met.EngineTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleOwner - VehicleRegistration (1:N)
        modelBuilder.Entity<VehicleOwner>()
            .HasMany(o => o.Registrations)
            .WithOne(r => r.Owner)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // VehicleModel - VehicleRegistration (1:N)
        modelBuilder.Entity<VehicleModel>()
            .HasMany(m => m.Registrations)
            .WithOne(r => r.Model)
            .HasForeignKey(r => r.ModelId)
            .OnDelete(DeleteBehavior.NoAction);

        // VehicleModelEngineType - VehicleRegistration (1:N)
        modelBuilder.Entity<VehicleModelEngineType>()
            .HasMany(met => met.Registrations)
            .WithOne(r => r.ModelEngineType)
            .HasForeignKey(r => r.ModelEngineTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
