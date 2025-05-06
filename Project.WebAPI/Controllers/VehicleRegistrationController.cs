using Microsoft.AspNetCore.Mvc;
using Project.Model.Common;
using Project.Service.Common;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleRegistrationController : ControllerBase
    {
        private readonly IVehicleRegistrationService _vehicleRegistrationService;

        public VehicleRegistrationController(IVehicleRegistrationService vehicleRegistrationService)
        {
            _vehicleRegistrationService = vehicleRegistrationService;
        }

        // GET: api/VehicleRegistration
        [HttpGet]
        public async Task<ActionResult<PagedResult<IVehicleRegistration>>> GetAll([FromQuery] QueryOptions queryOptions)
        {
            var registrations = await _vehicleRegistrationService.GetAllRegistrations(queryOptions);
            return Ok(registrations);
        }

        // GET: api/VehicleRegistration/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IVehicleRegistration>>> GetPaged([FromQuery] QueryOptions queryOptions)
        {
            var pagedRegistrations = await _vehicleRegistrationService.GetPagedRegistrations(queryOptions);
            return Ok(pagedRegistrations);
        }

        // GET: api/VehicleRegistration/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IVehicleRegistration>> GetById(int id)
        {
            var registration = await _vehicleRegistrationService.GetRegistrationById(id);
            if (registration == null)
            {
                return NotFound();
            }
            return Ok(registration);
        }

        // POST: api/VehicleRegistration
        [HttpPost]
        public async Task<ActionResult<IVehicleRegistration>> Create([FromBody] IVehicleRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Provjera postoji li već registracija s istim brojem
            if (await _vehicleRegistrationService.RegistrationExistsByNumber(registration.RegistrationNumber))
            {
                return Conflict("Registration with this number already exists.");
            }

            var createdRegistration = await _vehicleRegistrationService.AddRegistration(registration);
            return CreatedAtAction(nameof(GetById), new { id = createdRegistration.Id }, createdRegistration);
        }

        // PUT: api/VehicleRegistration/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IVehicleRegistration registration)
        {
            if (id != registration.Id)
            {
                return BadRequest("ID in URL does not match ID in the registration.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRegistration = await _vehicleRegistrationService.GetRegistrationById(id);
            if (existingRegistration == null)
            {
                return NotFound($"Registration with ID {id} not found.");
            }

            var updatedRegistration = await _vehicleRegistrationService.UpdateRegistration(registration);
            return Ok(updatedRegistration);
        }

        // DELETE: api/VehicleRegistration/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vehicleRegistrationService.DeleteRegistration(id);
            if (!success)
            {
                return NotFound($"Registration with ID {id} not found.");
            }

            return NoContent();
        }

        // GET: api/VehicleRegistration/byOwner/{ownerId}
        [HttpGet("byOwner/{ownerId}")]
        public async Task<ActionResult<PagedResult<IVehicleRegistration>>> GetByOwnerId(int ownerId, [FromQuery] QueryOptions queryOptions)
        {
            if (queryOptions.Filtering == null)
            {
                queryOptions.Filtering = new FilteringOptions { Filters = new Dictionary<string, string>() };
            }
            queryOptions.Filtering.Filters["OwnerId"] = ownerId.ToString();
            
            var registrations = await _vehicleRegistrationService.GetPagedRegistrations(queryOptions);
            return Ok(registrations);
        }

        // GET: api/VehicleRegistration/byModel/{modelId}
        [HttpGet("byModel/{modelId}")]
        public async Task<ActionResult<PagedResult<IVehicleRegistration>>> GetByModelId(int modelId, [FromQuery] QueryOptions queryOptions)
        {
            if (queryOptions.Filtering == null)
            {
                queryOptions.Filtering = new FilteringOptions { Filters = new Dictionary<string, string>() };
            }
            queryOptions.Filtering.Filters["ModelId"] = modelId.ToString();
            
            var registrations = await _vehicleRegistrationService.GetPagedRegistrations(queryOptions);
            return Ok(registrations);
        }
    }
} 