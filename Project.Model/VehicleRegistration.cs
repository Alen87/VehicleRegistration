using Project.Model.Common;

namespace Project.Model;

/// <summary>
/// Model koji predstavlja registraciju vozila
/// </summary>
public class VehicleRegistration : IVehicleRegistration
{

    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int ModelEngineTypeId { get; set; }
}