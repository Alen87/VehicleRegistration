namespace Project.Model.Common;


public interface IVehicleMake : IBaseModel
{

    string Name { get; set; }
    string Abrv { get; set; }
}