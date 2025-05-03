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
using System.Linq.Expressions;

namespace Project.Service
{
    public class VehicleOwnerService : IVehicleOwnerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleOwnerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<IVehicleOwner>> GetAllOwners(QueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
            }

         
            queryOptions.Paging = new PagingOptions { PageNumber = 1, PageSize = int.MaxValue };

            return await _unitOfWork.VehicleOwnerRepository.GetPagedAsync(queryOptions);
        }

        public async Task<PagedResult<IVehicleOwner>> GetPagedOwners(QueryOptions queryOptions)
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
                queryOptions.Sorting = new SortOptions { SortBy = "LastName", SortAscending = true };
            }

            return await _unitOfWork.VehicleOwnerRepository.GetPagedAsync(queryOptions);
        }

        public async Task<IVehicleOwner> GetOwnerById(int id)
        {
            var owner = await _unitOfWork.VehicleOwnerRepository.GetByIdAsync(id);
            if (owner == null)
                throw new KeyNotFoundException($"Vlasnik vozila s ID-om {id} nije pronađen.");
            
            return owner;
        }

        public async Task<IVehicleOwner> AddOwner(IVehicleOwner owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner), "Vlasnik vozila ne može biti null");

            if (string.IsNullOrWhiteSpace(owner.FirstName))
                throw new ArgumentException("Ime vlasnika vozila je obavezno", nameof(owner));

            if (string.IsNullOrWhiteSpace(owner.LastName))
                throw new ArgumentException("Prezime vlasnika vozila je obavezno", nameof(owner));

            if (owner.DateOfBirth == default)
                throw new ArgumentException("Datum rođenja vlasnika vozila je obavezan", nameof(owner));

            bool exists = await OwnerExistsByFirstNameAndLastName(owner.FirstName, owner.LastName);
            if (exists)
                throw new InvalidOperationException($"Vlasnik vozila s imenom '{owner.FirstName}' i prezimenom '{owner.LastName}' već postoji");

            var result = await _unitOfWork.VehicleOwnerRepository.AddAsync(owner);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<IVehicleOwner> UpdateOwner(IVehicleOwner owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner), "Vlasnik vozila ne može biti null");

            if (owner.Id <= 0)
                throw new ArgumentException("ID vlasnika vozila nije valjan", nameof(owner));

            if (string.IsNullOrWhiteSpace(owner.FirstName))
                throw new ArgumentException("Ime vlasnika vozila je obavezno", nameof(owner));

            if (string.IsNullOrWhiteSpace(owner.LastName))
                throw new ArgumentException("Prezime vlasnika vozila je obavezno", nameof(owner));

            if (owner.DateOfBirth == default)
                throw new ArgumentException("Datum rođenja vlasnika vozila je obavezan", nameof(owner));

            var existingOwner = await _unitOfWork.VehicleOwnerRepository.GetByIdAsync(owner.Id);
            if (existingOwner == null)
                throw new InvalidOperationException($"Vlasnik vozila s ID-om {owner.Id} ne postoji");

            var result = await _unitOfWork.VehicleOwnerRepository.UpdateAsync(owner);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteOwner(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID vlasnika vozila nije valjan", nameof(id));

            var result = await _unitOfWork.VehicleOwnerRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> OwnerExistsByFirstNameAndLastName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                return false;

            return await _unitOfWork.VehicleOwnerRepository.ExistsAsync(o => 
                o.FirstName.ToLower() == firstName.ToLower() && 
                o.LastName.ToLower() == lastName.ToLower());
        }

        public async Task<IVehicleOwner> GetFirstOwnerAsync(Expression<Func<IVehicleOwner, bool>> predicate)
        {
            var owner = await _unitOfWork.VehicleOwnerRepository.GetFirstAsync(predicate);
            if (owner == null)
                throw new KeyNotFoundException("Vlasnik vozila nije pronađen.");
            
            return owner;
        }
    }
} 