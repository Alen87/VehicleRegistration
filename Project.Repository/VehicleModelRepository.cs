using AutoMapper;
using Project.DAL;
using Entities = Project.DAL.Entities;
using Project.Model;
using Project.Model.Common;
using Project.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project.Common;
using Project.Common.Paging;

namespace Project.Repository;


public class VehicleModelRepository : GenericRepository<IVehicleModel, Entities.VehicleModel>, IVehicleModelRepository
{
    public VehicleModelRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
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