using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;

/// <summary>
/// Spojna tablica koja implementira vezu više-na-više između modela vozila i tipova motora
/// </summary>
public class VehicleModelEngineType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "ID modela vozila je obavezan.")]
    public int ModelId { get; set; }
    
    [Required(ErrorMessage = "ID tipa motora je obavezan.")]
    public int EngineTypeId { get; set; }

    public virtual VehicleModel Model { get; set; } = null!;
    public virtual VehicleEngineType EngineType { get; set; } = null!;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovu kombinaciju modela i tipa motora
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();
}