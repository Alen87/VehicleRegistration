namespace Project.DAL.Entities;

/// <summary>
/// Spojna tablica koja implementira vezu više-na-više između modela vozila i tipova motora
/// </summary>
public class VehicleModelEngineType
{

    public int Id { get; set; }


    public int ModelId { get; set; }
    public int EngineTypeId { get; set; }

    public virtual VehicleModel Model { get; set; } = null!;
    public virtual VehicleEngineType EngineType { get; set; } = null!;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovu kombinaciju modela i tipa motora
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();
}