using AutoMapper;
using Project.DAL;
using Entities = Project.DAL.Entities;
using Project.Model.Common;
using Project.Repository.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project.Common;

namespace Project.Repository;

public class VehicleEngineTypeRepository : GenericRepository<IVehicleEngineType, Entities.VehicleEngineType>, IVehicleEngineTypeRepository
{
    public VehicleEngineTypeRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
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