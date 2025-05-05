using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Sorting;
using Project.Model.Common;
using Project.Repository.Common;
using Project.Service.Common;

namespace Project.Service
{
    public class VehicleEngineTypeService : IVehicleEngineTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleEngineTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<IVehicleEngineType>> GetAllEngineTypes(QueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
            }

           
            queryOptions.Paging = new PagingOptions { PageNumber = 1, PageSize = int.MaxValue };

            return await _unitOfWork.VehicleEngineTypeRepository.GetPagedAsync(queryOptions);
        }

        public async Task<PagedResult<IVehicleEngineType>> GetPagedEngineTypes(QueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
            }

            if (queryOptions.Paging == null)
            {
                queryOptions.Paging = new PagingOptions { PageNumber = 1, PageSize = 10 };
            }
            else
            {
                if (queryOptions.Paging.PageNumber < 1)
                    queryOptions.Paging.PageNumber = 1;

                if (queryOptions.Paging.PageSize < 1)
                    queryOptions.Paging.PageSize = 10;
            }

            if (queryOptions.Sorting == null)
            {
                queryOptions.Sorting = new SortOptions { SortBy = "Type", SortAscending = true };
            }

            return await _unitOfWork.VehicleEngineTypeRepository.GetPagedAsync(queryOptions);
        }

        public async Task<IVehicleEngineType> GetEngineTypeById(int id)
        {
            var engineType = await _unitOfWork.VehicleEngineTypeRepository.GetByIdAsync(id);
            if (engineType == null)
                throw new KeyNotFoundException($"Tip motora s ID-om {id} nije pronađen.");

            return engineType;
        }

        public async Task<IVehicleEngineType> GetFirstEngineTypeAsync(Expression<Func<IVehicleEngineType, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Ne može biti null");

            var engineType = await _unitOfWork.VehicleEngineTypeRepository.GetFirstAsync(predicate);
            if (engineType == null)
                throw new KeyNotFoundException("Tip motora nije pronađen.");
            
            return engineType;
        }

        public async Task<bool> EngineTypeExistsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _unitOfWork.VehicleEngineTypeRepository.ExistsAsync(et => et.Type.ToLower() == name.ToLower());
        }
    }
} 