using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Model.Common;
using Project.Common;
using Project.Common.Paging;

namespace Project.Service.Common
{
    public interface IVehicleModelService
    {
        Task<IEnumerable<IVehicleModel>> GetAllModels();
        Task<PagedResult<IVehicleModel>> GetPagedModels(QueryOptions queryOptions);
        Task<IVehicleModel> GetModelById(int id);
        Task<IVehicleModel> GetFirstModelAsync(Expression<Func<IVehicleModel, bool>> predicate);
        Task<IVehicleModel> AddModel(IVehicleModel model);
        Task<IVehicleModel> UpdateModel(IVehicleModel model);
        Task<bool> DeleteModel(int id);
        Task<bool> ModelExistsByName(string name);
    }
} 