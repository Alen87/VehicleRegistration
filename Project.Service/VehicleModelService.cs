using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Sorting;
using Project.Model.Common;
using Project.Repository.Common;
using Project.Service.Common;

namespace Project.Service
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly IVehicleModelRepository _vehicleModelRepository;

        public VehicleModelService(IVehicleModelRepository vehicleModelRepository)
        {
            _vehicleModelRepository = vehicleModelRepository;
        }

        public async Task<IEnumerable<IVehicleModel>> GetAllModels()
        {
            return await _vehicleModelRepository.GetAllAsync();
        }

        public async Task<PagedResult<IVehicleModel>> GetPagedModels(QueryOptions queryOptions)
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
                queryOptions.Sorting = new SortOptions { SortBy = "Name", SortAscending = true };
            }

            return await _vehicleModelRepository.GetPagedAsync(queryOptions);
        }

        public async Task<IVehicleModel> GetModelById(int id)
        {
            var model = await _vehicleModelRepository.GetByIdAsync(id);
            if (model == null)
                throw new KeyNotFoundException($"Model vozila s ID-om {id} nije pronađen.");
            
            return model;
        }

        public async Task<IVehicleModel> AddModel(IVehicleModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model vozila ne može biti null");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Ime modela vozila je obavezno", nameof(model));

            if (model.MakeId <= 0)
                throw new ArgumentException("ID proizvođača vozila je obavezan", nameof(model));

            bool exists = await ModelExistsByName(model.Name);
            if (exists)
                throw new InvalidOperationException($"Model vozila s imenom '{model.Name}' već postoji");

            return await _vehicleModelRepository.AddAsync(model);
        }

        public async Task<IVehicleModel> UpdateModel(IVehicleModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model vozila ne može biti null");

            if (model.Id <= 0)
                throw new ArgumentException("ID modela vozila nije valjan", nameof(model));

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Ime modela vozila je obavezno", nameof(model));

            if (model.MakeId <= 0)
                throw new ArgumentException("ID proizvođača vozila je obavezan", nameof(model));

            var existingModel = await _vehicleModelRepository.GetByIdAsync(model.Id);
            if (existingModel == null)
                throw new InvalidOperationException($"Model vozila s ID-om {model.Id} ne postoji");

            return await _vehicleModelRepository.UpdateAsync(model);
        }

        public async Task<bool> DeleteModel(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID modela vozila nije valjan", nameof(id));

            return await _vehicleModelRepository.DeleteAsync(id);
        }

        public async Task<bool> ModelExistsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _vehicleModelRepository.ExistsAsync(m => m.Name.ToLower() == name.ToLower());
        }
    }
} 