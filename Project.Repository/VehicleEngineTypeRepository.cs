using AutoMapper;
using Project.DAL;
using Entities = Project.DAL.Entities;
using Project.Model.Common;
using Project.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project.Common;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Project.Repository;

public class VehicleEngineTypeRepository : GenericRepository<IVehicleEngineType, Entities.VehicleEngineType>, IVehicleEngineTypeRepository
{
    public VehicleEngineTypeRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    
    protected override async Task<IVehicleEngineType?> FindModelAsync(Expression<Func<IVehicleEngineType, bool>> predicate)
    {
        
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
      
        if (TryExtractPropertyValueFromPredicate(predicate, "Type", out string type))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Type.ToLower() == type.ToLower());
            return entity != null ? MapEntityToModel(entity) : default;
        }


        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .FirstOrDefaultAsync(predicate);
    }
    
    protected override async Task<bool> ExistsModelAsync(Expression<Func<IVehicleEngineType, bool>> predicate)
    {
      
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        
        
        if (TryExtractPropertyValueFromPredicate(predicate, "Type", out string type))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.Type.ToLower() == type.ToLower());
        }


        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .AnyAsync(predicate);
    }
    
    protected override IQueryable<Entities.VehicleEngineType> ApplyFiltering(IQueryable<Entities.VehicleEngineType> query, QueryOptions options)
    {
        if (options.Filtering != null && !string.IsNullOrWhiteSpace(options.Filtering.SearchField))
        {
            string searchQuery = options.Filtering.SearchText?.ToLower() ?? string.Empty;
            query = query.Where(e =>
                e.Type.ToLower().Contains(searchQuery) ||
                e.Abrv.ToLower().Contains(searchQuery));
        }

        return query;
    }

    protected override IQueryable<Entities.VehicleEngineType> ApplySorting(IQueryable<Entities.VehicleEngineType> query, QueryOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Sorting.SortBy))
        {
            bool isAscending = options.Sorting.SortAscending;

            switch (options.Sorting.SortBy.ToLower())
            {
                case "type":
                    query = isAscending
                        ? query.OrderBy(e => e.Type)
                        : query.OrderByDescending(e => e.Type);
                    break;
                case "abrv":
                    query = isAscending
                        ? query.OrderBy(e => e.Abrv)
                        : query.OrderByDescending(e => e.Abrv);
                    break;
                default:
                    query = isAscending
                        ? query.OrderBy(e => e.Id)
                        : query.OrderByDescending(e => e.Id);
                    break;
            }
        }

        return query;
    }
}