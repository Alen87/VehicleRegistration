namespace Project.Model.Common;


public interface IVehicleModel : IBaseModel
{

    string Name { get; set; }
    string Abrv { get; set; }
    int MakeId { get; set; }
    string MakeName { get; set; }
}