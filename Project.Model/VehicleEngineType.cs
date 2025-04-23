using Project.Model.Common;

namespace Project.Model;

/// <summary>
/// Model koji predstavlja tip motora vozila 
/// </summary>
public class VehicleEngineType : IVehicleEngineType
{

    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Abrv { get; set; } = string.Empty;
}