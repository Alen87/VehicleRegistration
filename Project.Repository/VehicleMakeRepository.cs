using AutoMapper;
using Project.DAL;
using Project.Model.Common;
using Entities = Project.DAL.Entities;
using Project.Repository.Common;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Project.Common;
using System.Text.RegularExpressions;

namespace Project.Repository;


public class VehicleMakeRepository : GenericRepository<IVehicleMake, Entities.VehicleMake>, IVehicleMakeRepository
{

    public VehicleMakeRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override async Task<IVehicleMake?> FindModelAsync(Expression<Func<IVehicleMake, bool>> predicate)
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
        
        
        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .FirstOrDefaultAsync(predicate);
    }
    
    protected override async Task<bool> ExistsModelAsync(Expression<Func<IVehicleMake, bool>> predicate)
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
        
       
          return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .AnyAsync(predicate);
    }

    protected override IQueryable<Entities.VehicleMake> ApplyFiltering(IQueryable<Entities.VehicleMake> query, QueryOptions options)
    {
        if (options.Filtering != null && !string.IsNullOrWhiteSpace(options.Filtering.SearchText))
        {
            string searchQuery = options.Filtering.SearchText.ToLower();
            query = query.Where(m =>
                m.Name.ToLower().Contains(searchQuery) ||
                m.Abrv.ToLower().Contains(searchQuery));
        }

        return query;
    }


    protected override IQueryable<Entities.VehicleMake> ApplySorting(IQueryable<Entities.VehicleMake> query, QueryOptions options)
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