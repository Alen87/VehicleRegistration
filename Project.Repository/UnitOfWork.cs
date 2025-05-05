using AutoMapper;
using Project.DAL;
using Project.Repository.Common;

namespace Project.Repository;

/// <summary>
/// Implementacija Unit of Work obrasca koja koordinira rad više repozitorija
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly VehicleDbContext _context;
    private readonly IMapper _mapper;
    private bool _disposed;

    private IVehicleMakeRepository? _vehicleMakeRepository;
    private IVehicleModelRepository? _vehicleModelRepository;
    private IVehicleEngineTypeRepository? _vehicleEngineTypeRepository;
    private IVehicleOwnerRepository? _vehicleOwnerRepository;
    private IVehicleRegistrationRepository? _vehicleRegistrationRepository;

   
    public UnitOfWork(VehicleDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _disposed = false;
    }

    public IVehicleMakeRepository VehicleMakeRepository
    {
        get
        {
            _vehicleMakeRepository ??= new VehicleMakeRepository(_context, _mapper);
            return _vehicleMakeRepository;
        }
    }

    public IVehicleModelRepository VehicleModelRepository
    {
        get
        {
            _vehicleModelRepository ??= new VehicleModelRepository(_context, _mapper);
            return _vehicleModelRepository;
        }
    }

    
    public IVehicleEngineTypeRepository VehicleEngineTypeRepository
    {
        get
        {
            _vehicleEngineTypeRepository ??= new VehicleEngineTypeRepository(_context, _mapper);
            return _vehicleEngineTypeRepository;
        }
    }

    public IVehicleOwnerRepository VehicleOwnerRepository
    {
        get
        {
            _vehicleOwnerRepository ??= new VehicleOwnerRepository(_context, _mapper);
            return _vehicleOwnerRepository;
        }
    }

    public IVehicleRegistrationRepository VehicleRegistrationRepository
    {
        get
        {
            _vehicleRegistrationRepository ??= new VehicleRegistrationRepository(_context, _mapper);
            return _vehicleRegistrationRepository;
        }
    }

  
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

 
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

   
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
