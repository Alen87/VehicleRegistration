using Microsoft.AspNetCore.Mvc;
using Project.Model.Common;
using Project.Service.Common;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Filtering;

namespace Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleMakeController : ControllerBase
    {
        private readonly IVehicleMakeService _vehicleMakeService;
        private readonly IMapper _mapper;

        public VehicleMakeController(IVehicleMakeService vehicleMakeService, IMapper mapper)
        {
            _vehicleMakeService = vehicleMakeService;
            _mapper = mapper;
        }

        // GET: api/VehicleMake
        [HttpGet]
        public async Task<ActionResult<PagedResult<IVehicleMake>>> GetAll([FromQuery] QueryOptions queryOptions)
        {
            var vehicleMakes = await _vehicleMakeService.GetAllMakes(queryOptions);
            return Ok(vehicleMakes);
        }

        // GET: api/VehicleMake/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IVehicleMake>>> GetPaged([FromQuery] QueryOptions queryOptions)
        {
            var pagedMakes = await _vehicleMakeService.GetPagedMakes(queryOptions);
            return Ok(pagedMakes);
        }

        // GET: api/VehicleMake/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IVehicleMake>> GetById(int id)
        {
            var vehicleMake = await _vehicleMakeService.GetMakeById(id);
            if (vehicleMake == null)
            {
                return NotFound();
            }
            return Ok(vehicleMake);
        }

        // POST: api/VehicleMake
        [HttpPost]
        public async Task<ActionResult<IVehicleMake>> Create([FromBody] IVehicleMake vehicleMake)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Provjera postoji li već proizvođač s istim imenom
            if (await _vehicleMakeService.MakeExistsByName(vehicleMake.Name))
            {
                return Conflict("Vehicle make with this name already exists.");
            }

            var createdVehicleMake = await _vehicleMakeService.AddMake(vehicleMake);
            return CreatedAtAction(nameof(GetById), new { id = createdVehicleMake.Id }, createdVehicleMake);
        }

        // PUT: api/VehicleMake/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IVehicleMake vehicleMake)
        {
            if (id != vehicleMake.Id)
            {
                return BadRequest("ID in URL does not match ID in the make object.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMake = await _vehicleMakeService.GetMakeById(id);
            if (existingMake == null)
            {
                return NotFound($"Vehicle make with ID {id} not found.");
            }

            var updatedMake = await _vehicleMakeService.UpdateMake(vehicleMake);
            return Ok(updatedMake);
        }

        // DELETE: api/VehicleMake/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vehicleMakeService.DeleteMake(id);
            if (!success)
            {
                return NotFound($"Vehicle make with ID {id} not found.");
            }

            return NoContent();
        }
    }
} 