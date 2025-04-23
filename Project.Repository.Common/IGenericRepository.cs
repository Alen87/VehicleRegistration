using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Project.Model.Common;
using Project.Common;
using Project.Common.Paging;

namespace Project.Repository.Common
{
    
  
   

    public interface IGenericRepository<T> where T : IBaseModel
    {

        Task<IEnumerable<T>> GetAllAsync();
        Task<PagedResult<T>> GetPagedAsync(QueryOptions queryOptions);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
