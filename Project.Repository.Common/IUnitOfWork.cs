using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Model.Common;

namespace Project.Repository.Common;

public interface IUnitOfWork : IDisposable
{
    IVehicleMakeRepository VehicleMakeRepository { get; }
    IVehicleModelRepository VehicleModelRepository { get; }
    IVehicleEngineTypeRepository VehicleEngineTypeRepository { get; }
    IVehicleOwnerRepository VehicleOwnerRepository { get; }
    IVehicleRegistrationRepository VehicleRegistrationRepository { get; }

    Task<int> SaveChangesAsync();
}
