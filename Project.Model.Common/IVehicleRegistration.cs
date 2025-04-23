namespace Project.Model.Common;


public interface IVehicleRegistration : IBaseModel
{

    string RegistrationNumber { get; set; }


    int ModelId { get; set; }
    string ModelName { get; set; }
    int OwnerId { get; set; }
    string OwnerName { get; set; }
    int ModelEngineTypeId { get; set; }
}