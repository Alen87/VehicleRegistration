

namespace Project.DAL.Entities;


public class VehicleRegistration
{

    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;


    public int ModelId { get; set; }
    public int OwnerId { get; set; }
    public int ModelEngineTypeId { get; set; }


    public virtual VehicleModel Model { get; set; } = null!;
    public virtual VehicleOwner Owner { get; set; } = null!;
    public virtual VehicleModelEngineType ModelEngineType { get; set; } = null!;
}