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
    public class VehicleRegistrationService : IVehicleRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleRegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<IVehicleRegistration>> GetAllRegistrations(QueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
            }

            
            queryOptions.Paging = new PagingOptions { PageNumber = 1, PageSize = int.MaxValue };

            return await _unitOfWork.VehicleRegistrationRepository.GetPagedAsync(queryOptions);
        }

        public async Task<PagedResult<IVehicleRegistration>> GetPagedRegistrations(QueryOptions queryOptions)
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
                queryOptions.Sorting = new SortOptions { SortBy = "RegistrationNumber", SortAscending = true };
            }

            return await _unitOfWork.VehicleRegistrationRepository.GetPagedAsync(queryOptions);
        }

        public async Task<IVehicleRegistration> GetRegistrationById(int id)
        {
            var registration = await _unitOfWork.VehicleRegistrationRepository.GetByIdAsync(id);
            if (registration == null)
                throw new KeyNotFoundException($"Registracija vozila s ID-om {id} nije pronađena.");

            return registration;
        }

        public async Task<IVehicleRegistration> GetFirstRegistrationAsync(Expression<Func<IVehicleRegistration, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Ne može biti null");

            var registration = await _unitOfWork.VehicleRegistrationRepository.GetFirstAsync(predicate);
            if (registration == null)
                throw new KeyNotFoundException("Registracija vozila nije pronađena.");
            
            return registration;
        }

        public async Task<IVehicleRegistration> AddRegistration(IVehicleRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration), "Registracija vozila ne može biti null");

            if (string.IsNullOrWhiteSpace(registration.RegistrationNumber))
                throw new ArgumentException("Registracijski broj je obavezan", nameof(registration));

           
            bool exists = await RegistrationExistsByNumber(registration.RegistrationNumber);
            if (exists)
                throw new InvalidOperationException($"Registracija s brojem '{registration.RegistrationNumber}' već postoji");

            var model = await _unitOfWork.VehicleModelRepository.GetByIdAsync(registration.ModelId);
            if (model == null)
                throw new ArgumentException($"Model vozila s ID-om {registration.ModelId} ne postoji", nameof(registration));

           
            var owner = await _unitOfWork.VehicleOwnerRepository.GetByIdAsync(registration.OwnerId);
            if (owner == null)
                throw new ArgumentException($"Vlasnik s ID-om {registration.OwnerId} ne postoji", nameof(registration));

            registration.ModelName = model.Name;
            registration.OwnerName = $"{owner.FirstName} {owner.LastName}";

            var result = await _unitOfWork.VehicleRegistrationRepository.AddAsync(registration);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<IVehicleRegistration> UpdateRegistration(IVehicleRegistration registration)
        {
            if (registration == null)
                throw new ArgumentNullException(nameof(registration), "Registracija vozila ne može biti null");

            if (registration.Id <= 0)
                throw new ArgumentException("ID registracije vozila nije valjan", nameof(registration));

            if (string.IsNullOrWhiteSpace(registration.RegistrationNumber))
                throw new ArgumentException("Registracijski broj je obavezan", nameof(registration));

            
            var existingWithSameNumber = await _unitOfWork.VehicleRegistrationRepository.GetFirstAsync(
                r => r.RegistrationNumber.ToLower() == registration.RegistrationNumber.ToLower() && r.Id != registration.Id);
                
            if (existingWithSameNumber != null)
                throw new InvalidOperationException($"Registracija s brojem '{registration.RegistrationNumber}' već postoji");

            
            var existingRegistration = await _unitOfWork.VehicleRegistrationRepository.GetByIdAsync(registration.Id);
            if (existingRegistration == null)
                throw new InvalidOperationException($"Registracija vozila s ID-om {registration.Id} ne postoji");

            var model = await _unitOfWork.VehicleModelRepository.GetByIdAsync(registration.ModelId);
            if (model == null)
                throw new ArgumentException($"Model vozila s ID-om {registration.ModelId} ne postoji", nameof(registration));

            
            var owner = await _unitOfWork.VehicleOwnerRepository.GetByIdAsync(registration.OwnerId);
            if (owner == null)
                throw new ArgumentException($"Vlasnik s ID-om {registration.OwnerId} ne postoji", nameof(registration));

            registration.ModelName = model.Name;
            registration.OwnerName = $"{owner.FirstName} {owner.LastName}";

            var result = await _unitOfWork.VehicleRegistrationRepository.UpdateAsync(registration);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteRegistration(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID registracije vozila nije valjan", nameof(id));

            var result = await _unitOfWork.VehicleRegistrationRepository.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return result;
        }

        public async Task<bool> RegistrationExistsByNumber(string registrationNumber)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
                return false;

            return await _unitOfWork.VehicleRegistrationRepository.ExistsAsync(r => r.RegistrationNumber.ToLower() == registrationNumber.ToLower());
        }
    }
} 