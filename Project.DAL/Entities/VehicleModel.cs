using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;


public class VehicleModel
{

    public int Id { get; set; }
    
    [Required(ErrorMessage = "Naziv modela je obavezan.")]
    [MaxLength(50, ErrorMessage = "Naziv modela može imati najviše 50 znakova.")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(10, ErrorMessage = "Skraćenica može imati najviše 10 znakova.")]
    public string Abrv { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ID proizvođača je obavezan.")]
    public int MakeId { get; set; }

    public virtual VehicleMake Make { get; set; } = null!;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija registracija za ovaj model
    /// </summary>
    public virtual ICollection<VehicleRegistration> Registrations { get; set; } = new List<VehicleRegistration>();

    /// <summary>
    /// Navigacijsko svojstvo - veza više-na-više s tipovima motora kroz spojnu tablicu
    /// </summary>
    public virtual ICollection<VehicleModelEngineType> ModelEngineTypes { get; set; } = new List<VehicleModelEngineType>();
}