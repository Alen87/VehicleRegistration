using Ninject;
using Project.Repository;
using Project.Repository.Common;
using Project.Service;
using Project.Service.Common;
using AutoMapper;
using Project.Model;
using Project.DAL.Entities;

namespace Project.WebAPI
{
    public static class NinjectConfig
    {
        public static void RegisterServices(IKernel kernel)
        {
            // Repository 
            kernel.Bind<IVehicleMakeRepository>().To<VehicleMakeRepository>();
            kernel.Bind<IVehicleModelRepository>().To<VehicleModelRepository>();
            kernel.Bind<IVehicleOwnerRepository>().To<VehicleOwnerRepository>();
            kernel.Bind<IVehicleEngineTypeRepository>().To<VehicleEngineTypeRepository>();
            kernel.Bind<IVehicleRegistrationRepository>().To<VehicleRegistrationRepository>();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();

            // Service 
            kernel.Bind<IVehicleMakeService>().To<VehicleMakeService>();
            kernel.Bind<IVehicleModelService>().To<VehicleModelService>();
            kernel.Bind<IVehicleOwnerService>().To<VehicleOwnerService>();
            kernel.Bind<IVehicleEngineTypeService>().To<VehicleEngineTypeService>();
            kernel.Bind<IVehicleRegistrationService>().To<VehicleRegistrationService>();

           
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                // Entity → Model mapiranja
                cfg.CreateMap<DAL.Entities.VehicleMake, Model.VehicleMake>();
                cfg.CreateMap<DAL.Entities.VehicleModel, Model.VehicleModel>()
                   .ForMember(dest => dest.MakeName, opt => opt.MapFrom(src => src.Make.Name));
                cfg.CreateMap<DAL.Entities.VehicleOwner, Model.VehicleOwner>();
                cfg.CreateMap<DAL.Entities.VehicleEngineType, Model.VehicleEngineType>();
                cfg.CreateMap<DAL.Entities.VehicleRegistration, Model.VehicleRegistration>()
                   .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.Name))
                   .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FirstName + " " + src.Owner.LastName));
                
                // Model → Entity mapiranja
                cfg.CreateMap<Model.VehicleMake, DAL.Entities.VehicleMake>();
                cfg.CreateMap<Model.VehicleModel, DAL.Entities.VehicleModel>();
                cfg.CreateMap<Model.VehicleOwner, DAL.Entities.VehicleOwner>();
                cfg.CreateMap<Model.VehicleEngineType, DAL.Entities.VehicleEngineType>();
                cfg.CreateMap<Model.VehicleRegistration, DAL.Entities.VehicleRegistration>();
            });

            kernel.Bind<IMapper>().ToConstant(mapperConfiguration.CreateMapper()).InSingletonScope();
        }
    }
} 