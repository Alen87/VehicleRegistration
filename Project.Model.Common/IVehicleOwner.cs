namespace Project.Model.Common;


public interface IVehicleOwner : IBaseModel
{

    string FirstName { get; set; }
    string LastName { get; set; }
    DateTime DOB { get; set; }


}