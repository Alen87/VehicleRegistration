using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Model.Common;
using Project.Common;
using Project.Common.Paging;
using System.Linq.Expressions;

namespace Project.Service.Common
{
   
    public interface IVehicleMakeService
    {
       
        Task<IEnumerable<IVehicleMake>> GetAllMakes();
        Task<PagedResult<IVehicleMake>> GetPagedMakes(QueryOptions queryOptions);

        Task<IVehicleMake> GetMakeById(int id);
        Task<IVehicleMake> GetFirstMakeAsync(Expression<Func<IVehicleMake, bool>> predicate);
        Task<IVehicleMake> AddMake(IVehicleMake make);

       
        Task<IVehicleMake> UpdateMake(IVehicleMake make);

       
        Task<bool> DeleteMake(int id);

        
        Task<bool> MakeExistsByName(string name);
    }
}