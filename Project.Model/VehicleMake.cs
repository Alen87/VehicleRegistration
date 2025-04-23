using Project.Model.Common;

namespace Project.Model;

/// <summary>
/// Model koji predstavlja proizvođača vozila 
/// </summary>
public class VehicleMake : IVehicleMake
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;
}