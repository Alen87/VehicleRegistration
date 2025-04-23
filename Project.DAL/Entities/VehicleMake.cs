namespace Project.DAL.Entities;

/// <summary>
/// Predstavlja proizvođača vozila (npr. BMW, Ford, Volkswagen)
/// </summary>
public class VehicleMake
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;

    /// <summary>
    /// Navigacijsko svojstvo - kolekcija modela za ovog proizvođača
    /// </summary>
    public virtual ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
}