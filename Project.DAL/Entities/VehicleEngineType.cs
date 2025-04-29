using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;


public class VehicleEngineType
{

    public int Id { get; set; }
    
    [Required(ErrorMessage = "Tip motora je obavezan.")]
    [MaxLength(50, ErrorMessage = "Tip motora može imati najviše 50 znakova.")]
    public string Type { get; set; } = string.Empty;
    
    [MaxLength(10, ErrorMessage = "Skraćenica može imati najviše 10 znakova.")]
    public string Abrv { get; set; } = string.Empty;

    /// <summary>
    /// Navigacijsko svojstvo - veza više-na-više s modelima vozila kroz spojnu tablicu
    /// </summary>
    public virtual ICollection<VehicleModelEngineType> ModelEngineTypes { get; set; } = new List<VehicleModelEngineType>();
}