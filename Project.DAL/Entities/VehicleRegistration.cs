using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;


public class VehicleRegistration
{

    public int Id { get; set; }
    
    [Required(ErrorMessage = "Registracija je obavezna.")]
    [MaxLength(20, ErrorMessage = "Registracijski broj može imati najviše 20 znakova.")]
    public string RegistrationNumber { get; set; } = string.Empty;


    [Required(ErrorMessage = "ID modela vozila je obavezan.")]
    public int ModelId { get; set; }
    
    [Required(ErrorMessage = "ID vlasnika je obavezan.")]
    public int OwnerId { get; set; }
    
    [Required(ErrorMessage = "ID kombinacije modela i tipa motora je obavezan.")]
    public int ModelEngineTypeId { get; set; }


    public virtual VehicleModel Model { get; set; } = null!;
    public virtual VehicleOwner Owner { get; set; } = null!;
    public virtual VehicleModelEngineType ModelEngineType { get; set; } = null!;
}