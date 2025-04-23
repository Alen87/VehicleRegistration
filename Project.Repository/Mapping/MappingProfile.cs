using AutoMapper;
using Project.Model;
using Entities = Project.DAL.Entities;

namespace Project.Repository.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapiranje za VehicleMake
        CreateMap<Entities.VehicleMake, Model.VehicleMake>().ReverseMap();

        // Mapiranje za VehicleModel
        CreateMap<Entities.VehicleModel, Model.VehicleModel>().ReverseMap();

        // Mapiranje za VehicleEngineType
        CreateMap<Entities.VehicleEngineType, Model.VehicleEngineType>().ReverseMap();

        // Mapiranje za VehicleOwner
        CreateMap<Entities.VehicleOwner, Model.VehicleOwner>().ReverseMap();

        // Mapiranje za VehicleRegistration
        CreateMap<Entities.VehicleRegistration, Model.VehicleRegistration>().ReverseMap();
    }
}