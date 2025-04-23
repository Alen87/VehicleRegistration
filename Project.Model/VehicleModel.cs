using Project.Model.Common;

namespace Project.Model;

/// <summary>
/// Model koji predstavlja model vozila
/// </summary>
public class VehicleModel : IVehicleModel
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;
    public int MakeId { get; set; }
    public string MakeName { get; set; } = string.Empty;
}