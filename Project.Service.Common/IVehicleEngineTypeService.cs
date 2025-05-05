using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Common;
using Project.Common.Paging;
using Project.Model.Common;

namespace Project.Service.Common
{
    public interface IVehicleEngineTypeService
    {
        
        Task<PagedResult<IVehicleEngineType>> GetAllEngineTypes(QueryOptions queryOptions);
        
        Task<PagedResult<IVehicleEngineType>> GetPagedEngineTypes(QueryOptions queryOptions);
        
        Task<IVehicleEngineType> GetEngineTypeById(int id);
        
     
        Task<IVehicleEngineType> GetFirstEngineTypeAsync(Expression<Func<IVehicleEngineType, bool>> predicate);
        
        
        Task<bool> EngineTypeExistsByName(string name);
    }
} 