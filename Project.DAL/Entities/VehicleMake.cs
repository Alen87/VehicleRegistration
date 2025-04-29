using System.ComponentModel.DataAnnotations;

namespace Project.DAL.Entities;

/// <summary>
/// Predstavlja proizvođača vozila (npr. BMW, Ford, Volkswagen)
/// </summary>
public class VehicleMake
{

    public int Id { get; set; }

    [Required]
    [MaxLength(50,ErrorMessage = "Naziv proizvođača može imati najviše 50 znakova.")]
    public string Name { get; set; } = string.Empty;
    [MaxLength(10,ErrorMessage = "Skraćenica može imati najviše 10 znakova.")]
    public string Abrv { get; set; } = string.Empty;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija modela za ovog proizvođača
    /// </summary>
    public virtual ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
}