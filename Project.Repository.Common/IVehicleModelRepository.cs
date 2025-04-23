using Project.Common.Paging;
using Project.Common;
using Project.Model.Common;


namespace Project.Repository.Common;


public interface IVehicleModelRepository : IGenericRepository<IVehicleModel>
{
   
    Task<IEnumerable<IVehicleModel>> GetByMakeIdAsync(int makeId, QueryOptions queryOptions);

  
    Task<PagedResult<IVehicleModel>> GetPagedByMakeIdAsync(int makeId, QueryOptions queryOptions);
}