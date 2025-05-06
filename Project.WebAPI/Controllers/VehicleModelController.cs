using Microsoft.AspNetCore.Mvc;
using Project.Model.Common;
using Project.Service.Common;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleModelController : ControllerBase
    {
        private readonly IVehicleModelService _vehicleModelService;

        public VehicleModelController(IVehicleModelService vehicleModelService)
        {
            _vehicleModelService = vehicleModelService;
        }

        // GET: api/VehicleModel
        [HttpGet]
        public async Task<ActionResult<PagedResult<IVehicleModel>>> GetAll([FromQuery] QueryOptions queryOptions)
        {
            var vehicleModels = await _vehicleModelService.GetAllModels(queryOptions);
            return Ok(vehicleModels);
        }

        // GET: api/VehicleModel/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IVehicleModel>>> GetPaged([FromQuery] QueryOptions queryOptions)
        {
            var pagedModels = await _vehicleModelService.GetPagedModels(queryOptions);
            return Ok(pagedModels);
        }

        // GET: api/VehicleModel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IVehicleModel>> GetById(int id)
        {
            var vehicleModel = await _vehicleModelService.GetModelById(id);
            if (vehicleModel == null)
            {
                return NotFound();
            }
            return Ok(vehicleModel);
        }

        // GET: api/VehicleModel/byMake/{makeId}
        [HttpGet("byMake/{makeId}")]
        public async Task<ActionResult<PagedResult<IVehicleModel>>> GetByMakeId(int makeId, [FromQuery] QueryOptions queryOptions)
        {
            // Dodajemo filter za MakeId
            if (queryOptions.Filtering == null)
            {
                queryOptions.Filtering = new FilteringOptions { Filters = new Dictionary<string, string>() };
            }
            queryOptions.Filtering.Filters["MakeId"] = makeId.ToString();
            
            var models = await _vehicleModelService.GetPagedModels(queryOptions);
            return Ok(models);
        }

        // POST: api/VehicleModel
        [HttpPost]
        public async Task<ActionResult<IVehicleModel>> Create([FromBody] IVehicleModel vehicleModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Provjera postoji li već model s istim imenom
            if (await _vehicleModelService.ModelExistsByName(vehicleModel.Name))
            {
                return Conflict("Vehicle model with this name already exists.");
            }

            var createdModel = await _vehicleModelService.AddModel(vehicleModel);
            return CreatedAtAction(nameof(GetById), new { id = createdModel.Id }, createdModel);
        }

        // PUT: api/VehicleModel/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IVehicleModel vehicleModel)
        {
            if (id != vehicleModel.Id)
            {
                return BadRequest("ID in URL does not match ID in the model.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingModel = await _vehicleModelService.GetModelById(id);
            if (existingModel == null)
            {
                return NotFound($"Vehicle model with ID {id} not found.");
            }

            var updatedModel = await _vehicleModelService.UpdateModel(vehicleModel);
            return Ok(updatedModel);
        }

        // DELETE: api/VehicleModel/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vehicleModelService.DeleteModel(id);
            if (!success)
            {
                return NotFound($"Vehicle model with ID {id} not found.");
            }

            return NoContent();
        }
    }
} 