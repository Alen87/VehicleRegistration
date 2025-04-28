using System;

namespace Project.Model.Common;


public interface IVehicleOwner : IBaseModel
{

    string FirstName { get; set; }
    string LastName { get; set; }
    DateTime DateOfBirth { get; set; }


}