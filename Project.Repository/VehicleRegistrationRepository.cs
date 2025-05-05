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

public class VehicleRegistrationRepository : GenericRepository<IVehicleRegistration, Entities.VehicleRegistration>, IVehicleRegistrationRepository
{
    public VehicleRegistrationRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    
    protected override async Task<IVehicleRegistration?> FindModelAsync(Expression<Func<IVehicleRegistration, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            var entity = await _dbSet.FindAsync(id);
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "RegistrationNumber", out string registrationNumber))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.RegistrationNumber.ToLower() == registrationNumber.ToLower());
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "ModelId", out int modelId))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ModelId == modelId);
            return entity != null ? MapEntityToModel(entity) : default;
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "OwnerId", out int ownerId))
        {
            var entity = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.OwnerId == ownerId);
            return entity != null ? MapEntityToModel(entity) : default;
        }

        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .FirstOrDefaultAsync(predicate);
    }
    
    protected override async Task<bool> ExistsModelAsync(Expression<Func<IVehicleRegistration, bool>> predicate)
    {
        if (TryExtractIdFromPredicate(predicate, out int id))
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
        
        if (TryExtractPropertyValueFromPredicate(predicate, "RegistrationNumber", out string registrationNumber))
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(e => e.RegistrationNumber.ToLower() == registrationNumber.ToLower());
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "ModelId", out int modelId))
        {
            return await _dbSet.AnyAsync(e => e.ModelId == modelId);
        }
        
        if (TryExtractNumericPropertyValueFromPredicate(predicate, "OwnerId", out int ownerId))
        {
            return await _dbSet.AnyAsync(e => e.OwnerId == ownerId);
        }
        
        return await _dbSet.AsNoTracking()
            .Select(e => MapEntityToModel(e))
            .AnyAsync(predicate);
    }
    
    protected override IQueryable<Entities.VehicleRegistration> ApplyFiltering(IQueryable<Entities.VehicleRegistration> query, QueryOptions options)
    {
        if (options.Filtering != null && !string.IsNullOrWhiteSpace(options.Filtering.SearchText))
        {
            string searchQuery = options.Filtering.SearchText.ToLower();
            query = query.Where(r =>
                r.RegistrationNumber.ToLower().Contains(searchQuery));
        }

        return query;
    }

    protected override IQueryable<Entities.VehicleRegistration> ApplySorting(IQueryable<Entities.VehicleRegistration> query, QueryOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Sorting.SortBy))
        {
            bool isAscending = options.Sorting.SortAscending;

            switch (options.Sorting.SortBy.ToLower())
            {
                case "registrationnumber":
                    query = isAscending
                        ? query.OrderBy(r => r.RegistrationNumber)
                        : query.OrderByDescending(r => r.RegistrationNumber);
                    break;
                case "modelid":
                    query = isAscending
                        ? query.OrderBy(r => r.ModelId)
                        : query.OrderByDescending(r => r.ModelId);
                    break;
                case "ownerid":
                    query = isAscending
                        ? query.OrderBy(r => r.OwnerId)
                        : query.OrderByDescending(r => r.OwnerId);
                    break;
                default:
                    query = isAscending
                        ? query.OrderBy(r => r.Id)
                        : query.OrderByDescending(r => r.Id);
                    break;
            }
        }

        return query;
    }
} 