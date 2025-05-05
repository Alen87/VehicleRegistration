using AutoMapper;
using Project.DAL;
using Entities = Project.DAL.Entities;
using Project.Model;
using Project.Model.Common;
using Project.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Project.Common;
using Project.Common.Paging;
using System.Text.RegularExpressions;

namespace Project.Repository;


public class VehicleModelRepository : GenericRepository<IVehicleModel, Entities.VehicleModel>, IVehicleModelRepository
{
    public VehicleModelRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override async Task<IVehicleModel?> FindModelAsync(Expression<Func<IVehicleModel, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "Name", out string name))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "MakeId", out int makeId))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.MakeId == makeId);
            return entity != null ? MapEntityToModel(entity) : default;
        }

        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .FirstOrDefaultAsync(predicate);
    }
    
    protected override async Task<bool> ExistsModelAsync(Expression<Func<IVehicleModel, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "Name", out string name))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.Name.ToLower() == name.ToLower());
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "MakeId", out int makeId))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.MakeId == makeId);
        }

         return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .AnyAsync(predicate);
    
    }

    public async Task<IEnumerable<IVehicleModel>> GetByMakeIdAsync(int makeId, QueryOptions queryOptions)
    {
        var entities = await _dbSet.Where(m => m.MakeId == makeId).ToListAsync();
        return entities.Select(MapEntityToModel);
    }

    
    public async Task<PagedResult<IVehicleModel>> GetPagedByMakeIdAsync(int makeId, QueryOptions queryOptions)
    {
        IQueryable<Entities.VehicleModel> query = _dbSet.Where(m => m.MakeId == makeId);

        query = ApplyFiltering(query, queryOptions);

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(queryOptions.Sorting.SortBy))
        {
            query = ApplySorting(query, queryOptions);
        }

        query = query.Skip((queryOptions.Paging.PageNumber - 1) * queryOptions.Paging.PageSize)
                     .Take(queryOptions.Paging.PageSize);

        var items = await query.ToListAsync();
        var mappedItems = items.Select(MapEntityToModel).ToList();

        return new PagedResult<IVehicleModel>(
            mappedItems,
            totalCount,
            queryOptions.Paging.PageNumber,
            queryOptions.Paging.PageSize);
    }

   
    protected override IQueryable<Entities.VehicleModel> ApplyFiltering(IQueryable<Entities.VehicleModel> query, QueryOptions options)
    {
        if (options.Filtering != null && !string.IsNullOrWhiteSpace(options.Filtering.SearchText))
        {
            string searchQuery = options.Filtering.SearchText.ToLower();
            query = query.Where(m =>
                m.Name.ToLower().Contains(searchQuery) ||
                m.Abrv.ToLower().Contains(searchQuery));
        }

        if (options.Filtering != null && options.Filtering.MakeId.HasValue)
        {
            query = query.Where(m => m.MakeId == options.Filtering.MakeId.Value);
        }

        return query;
    }

       protected override IQueryable<Entities.VehicleModel> ApplySorting(IQueryable<Entities.VehicleModel> query, QueryOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Sorting.SortBy))
        {
            bool isAscending = options.Sorting.SortAscending;

            switch (options.Sorting.SortBy.ToLower())
            {
                case "name":
                    query = isAscending
                        ? query.OrderBy(m => m.Name)
                        : query.OrderByDescending(m => m.Name);
                    break;
                case "abrv":
                    query = isAscending
                        ? query.OrderBy(m => m.Abrv)
                        : query.OrderByDescending(m => m.Abrv);
                    break;
                default:
                    query = isAscending
                        ? query.OrderBy(m => m.Id)
                        : query.OrderByDescending(m => m.Id);
                    break;
            }
        }

        return query;
    }
}