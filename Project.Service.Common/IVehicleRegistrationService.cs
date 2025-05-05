using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Project.Common;
using Project.Common.Paging;
using Project.Model.Common;

namespace Project.Service.Common
{
    public interface IVehicleRegistrationService
    {
        /// <summary>
        /// Dohvaća sve registracije vozila
        /// </summary>
        Task<PagedResult<IVehicleRegistration>> GetAllRegistrations(QueryOptions queryOptions);
        
        /// <summary>
        /// Dohvaća registracije vozila s paginacijom
        /// </summary>
        Task<PagedResult<IVehicleRegistration>> GetPagedRegistrations(QueryOptions queryOptions);
        
        /// <summary>
        /// Dohvaća registraciju vozila prema ID-u
        /// </summary>
        Task<IVehicleRegistration> GetRegistrationById(int id);
        
        /// <summary>
        /// Dohvaća prvu registraciju vozila koja zadovoljava uvjet
        /// </summary>
        Task<IVehicleRegistration> GetFirstRegistrationAsync(Expression<Func<IVehicleRegistration, bool>> predicate);
        
        /// <summary>
        /// Dodaje novu registraciju vozila
        /// </summary>
        Task<IVehicleRegistration> AddRegistration(IVehicleRegistration registration);
        
        /// <summary>
        /// Ažurira postojeću registraciju vozila
        /// </summary>
        Task<IVehicleRegistration> UpdateRegistration(IVehicleRegistration registration);
        
        /// <summary>
        /// Briše registraciju vozila
        /// </summary>
        Task<bool> DeleteRegistration(int id);
        
        /// <summary>
        /// Provjerava postoji li registracija s navedenim registracijskim brojem
        /// </summary>
        Task<bool> RegistrationExistsByNumber(string registrationNumber);
    }
} 