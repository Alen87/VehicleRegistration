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

public class VehicleOwnerRepository : GenericRepository<IVehicleOwner, Entities.VehicleOwner>, IVehicleOwnerRepository
{
    public VehicleOwnerRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    
    protected override async Task<IVehicleOwner?> FindModelAsync(Expression<Func<IVehicleOwner, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "FirstName", out string firstName))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.FirstName.ToLower() == firstName.ToLower());
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "LastName", out string lastName))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.LastName.ToLower() == lastName.ToLower());
            return entity != null ? MapEntityToModel(entity) : default;
        }

         return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .FirstOrDefaultAsync(predicate);
    }
    
    protected override async Task<bool> ExistsModelAsync(Expression<Func<IVehicleOwner, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "FirstName", out string firstName))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.FirstName.ToLower() == firstName.ToLower());
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "LastName", out string lastName))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.LastName.ToLower() == lastName.ToLower());
        }

       return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .AnyAsync(predicate);
    }
    
    protected override IQueryable<Entities.VehicleOwner> ApplyFiltering(IQueryable<Entities.VehicleOwner> query, QueryOptions options)
    {
        if (options.Filtering != null && !string.IsNullOrWhiteSpace(options.Filtering.SearchText))
        {
            string searchQuery = options.Filtering.SearchText.ToLower();
            query = query.Where(o =>
                o.FirstName.ToLower().Contains(searchQuery) ||
                o.LastName.ToLower().Contains(searchQuery));
        }

        return query;
    }

    protected override IQueryable<Entities.VehicleOwner> ApplySorting(IQueryable<Entities.VehicleOwner> query, QueryOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Sorting.SortBy))
        {
            bool isAscending = options.Sorting.SortAscending;

            switch (options.Sorting.SortBy.ToLower())
            {
                case "firstname":
                    query = isAscending
                        ? query.OrderBy(o => o.FirstName)
                        : query.OrderByDescending(o => o.FirstName);
                    break;
                case "lastname":
                    query = isAscending
                        ? query.OrderBy(o => o.LastName)
                        : query.OrderByDescending(o => o.LastName);
                    break;
                case "dateofbirth":
                case "dob":
                    query = isAscending
                        ? query.OrderBy(o => o.DOB)
                        : query.OrderByDescending(o => o.DOB);
                    break;
                default:
                    query = isAscending
                        ? query.OrderBy(o => o.Id)
                        : query.OrderByDescending(o => o.Id);
                    break;
            }
        }

        return query;
    }
} 