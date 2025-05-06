using Ninject;
using Project.Repository;
using Project.Repository.Common;
using Project.Service;
using Project.Service.Common;
using AutoMapper;

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
               
            });

            kernel.Bind<IMapper>().ToConstant(mapperConfiguration.CreateMapper()).InSingletonScope();
        }
    }
} 