using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Model.Common;
using Project.Common;
using Project.Common.Paging;

namespace Project.Service.Common
{
    public interface IVehicleOwnerService
    {
        Task<IEnumerable<IVehicleOwner>> GetAllOwners();
        Task<PagedResult<IVehicleOwner>> GetPagedOwners(QueryOptions queryOptions);
        Task<IVehicleOwner> GetOwnerById(int id);
        Task<IVehicleOwner> GetFirstOwnerAsync(Expression<Func<IVehicleOwner, bool>> predicate);
        Task<IVehicleOwner> AddOwner(IVehicleOwner owner);
        Task<IVehicleOwner> UpdateOwner(IVehicleOwner owner);
        Task<bool> DeleteOwner(int id);
        Task<bool> OwnerExistsByFirstNameAndLastName(string firstName, string lastName);
    }
} 