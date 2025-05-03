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

    public class VehicleMakeService : IVehicleMakeService
    {
        private readonly IUnitOfWork _unitOfWork;

       
        public VehicleMakeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       
        public async Task<PagedResult<IVehicleMake>> GetAllMakes(QueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
            }

           
            queryOptions.Paging = new PagingOptions { PageNumber = 1, PageSize = int.MaxValue };

            return await _unitOfWork.VehicleMakeRepository.GetPagedAsync(queryOptions);
        }

       
        public async Task<PagedResult<IVehicleMake>> GetPagedMakes(QueryOptions queryOptions)
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

            return await _unitOfWork.VehicleMakeRepository.GetPagedAsync(queryOptions);
        }


        public async Task<IVehicleMake> GetMakeById(int id)
        {
            var make = await _unitOfWork.VehicleMakeRepository.GetByIdAsync(id);
            if (make == null)
                throw new KeyNotFoundException($"Proizvođač vozila s ID-om {id} nije pronađen.");

            return make;
        }

        public async Task<IVehicleMake> GetFirstMakeAsync(Expression<Func<IVehicleMake, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), " Ne može biti null");

            var make = await _unitOfWork.VehicleMakeRepository.GetFirstAsync(predicate);
            if (make == null)
                throw new KeyNotFoundException("Proizvođač vozila nije pronađen.");
            
            return make;
        }





        public async Task<IVehicleMake> AddMake(IVehicleMake make)
        {
            if (make == null)
                throw new ArgumentNullException(nameof(make), "Proizvođač vozila ne može biti null");

            if (string.IsNullOrWhiteSpace(make.Name))
                throw new ArgumentException("Ime proizvođača vozila je obavezno", nameof(make));

            
            bool exists = await MakeExistsByName(make.Name);
            if (exists)
                throw new InvalidOperationException($"Proizvođač vozila s imenom '{make.Name}' već postoji");

            var result = await _unitOfWork.VehicleMakeRepository.AddAsync(make);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

       
        public async Task<IVehicleMake> UpdateMake(IVehicleMake make)
        {
            if (make == null)
                throw new ArgumentNullException(nameof(make), "Proizvođač vozila ne može biti null");

            if (make.Id <= 0)
                throw new ArgumentException("ID proizvođača vozila nije valjan", nameof(make));

            if (string.IsNullOrWhiteSpace(make.Name))
                throw new ArgumentException("Ime proizvođača vozila je obavezno", nameof(make));

            
            var existingMake = await _unitOfWork.VehicleMakeRepository.GetByIdAsync(make.Id);
            if (existingMake == null)
                throw new InvalidOperationException($"Proizvođač vozila s ID-om {make.Id} ne postoji");

            var result = await _unitOfWork.VehicleMakeRepository.UpdateAsync(make);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

      
        public async Task<bool> DeleteMake(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID proizvođača vozila nije valjan", nameof(id));

            var result = await _unitOfWork.VehicleMakeRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return result;
        }

       
        public async Task<bool> MakeExistsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            
            return await _unitOfWork.VehicleMakeRepository.ExistsAsync(m => m.Name.ToLower() == name.ToLower());
        }
    }
}