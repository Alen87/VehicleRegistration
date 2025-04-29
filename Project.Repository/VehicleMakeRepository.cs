using AutoMapper;
using Project.DAL;
using Project.Model.Common;
using Entities = Project.DAL.Entities;
using Project.Repository.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Project.Common;

namespace Project.Repository;


public class VehicleMakeRepository : GenericRepository<IVehicleMake, Entities.VehicleMake>, IVehicleMakeRepository
{

    public VehicleMakeRepository(VehicleDbContext context, IMapper mapper) : base(context, mapper)
    {
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