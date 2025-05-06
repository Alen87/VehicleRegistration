using Microsoft.AspNetCore.Mvc;
using Project.Model.Common;
using Project.Service.Common;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleOwnerController : ControllerBase
    {
        private readonly IVehicleOwnerService _vehicleOwnerService;

        public VehicleOwnerController(IVehicleOwnerService vehicleOwnerService)
        {
            _vehicleOwnerService = vehicleOwnerService;
        }

        // GET: api/VehicleOwner
        [HttpGet]
        public async Task<ActionResult<PagedResult<IVehicleOwner>>> GetAll([FromQuery] QueryOptions queryOptions)
        {
            var owners = await _vehicleOwnerService.GetAllOwners(queryOptions);
            return Ok(owners);
        }

        // GET: api/VehicleOwner/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IVehicleOwner>>> GetPaged([FromQuery] QueryOptions queryOptions)
        {
            var pagedOwners = await _vehicleOwnerService.GetPagedOwners(queryOptions);
            return Ok(pagedOwners);
        }

        // GET: api/VehicleOwner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IVehicleOwner>> GetById(int id)
        {
            var owner = await _vehicleOwnerService.GetOwnerById(id);
            if (owner == null)
            {
                return NotFound();
            }
            return Ok(owner);
        }

        // POST: api/VehicleOwner
        [HttpPost]
        public async Task<ActionResult<IVehicleOwner>> Create([FromBody] IVehicleOwner owner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Provjera postoji li već vlasnik s istim imenom i prezimenom
            if (await _vehicleOwnerService.OwnerExistsByFirstNameAndLastName(owner.FirstName, owner.LastName))
            {
                return Conflict("Vehicle owner with this first and last name already exists.");
            }

            var createdOwner = await _vehicleOwnerService.AddOwner(owner);
            return CreatedAtAction(nameof(GetById), new { id = createdOwner.Id }, createdOwner);
        }

        // PUT: api/VehicleOwner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IVehicleOwner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest("ID in URL does not match ID in the owner object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingOwner = await _vehicleOwnerService.GetOwnerById(id);
            if (existingOwner == null)
            {
                return NotFound($"Vehicle owner with ID {id} not found.");
            }

            var updatedOwner = await _vehicleOwnerService.UpdateOwner(owner);
            return Ok(updatedOwner);
        }

        // DELETE: api/VehicleOwner/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vehicleOwnerService.DeleteOwner(id);
            if (!success)
            {
                return NotFound($"Vehicle owner with ID {id} not found.");
            }

            return NoContent();
        }

        // GET: api/VehicleOwner/search
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<IVehicleOwner>>> Search(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] QueryOptions queryOptions)
        {
            if (queryOptions.Filtering == null)
            {
                queryOptions.Filtering = new FilteringOptions { Filters = new Dictionary<string, string>() };
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                queryOptions.Filtering.Filters["FirstName"] = firstName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                queryOptions.Filtering.Filters["LastName"] = lastName;
            }

            var pagedOwners = await _vehicleOwnerService.GetPagedOwners(queryOptions);
            return Ok(pagedOwners);
        }
    }
} 