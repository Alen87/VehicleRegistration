using Microsoft.AspNetCore.Mvc;
using Project.Model.Common;
using Project.Service.Common;
using Project.Common;
using Project.Common.Paging;
using Project.Common.Filtering;
using System.Threading.Tasks;

namespace Project.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleEngineTypeController : ControllerBase
    {
        private readonly IVehicleEngineTypeService _vehicleEngineTypeService;

        public VehicleEngineTypeController(IVehicleEngineTypeService vehicleEngineTypeService)
        {
            _vehicleEngineTypeService = vehicleEngineTypeService;
        }

        // GET: api/VehicleEngineType
        [HttpGet]
        public async Task<ActionResult<PagedResult<IVehicleEngineType>>> GetAll([FromQuery] QueryOptions queryOptions)
        {
            var engineTypes = await _vehicleEngineTypeService.GetAllEngineTypes(queryOptions);
            return Ok(engineTypes);
        }

        // GET: api/VehicleEngineType/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<IVehicleEngineType>>> GetPaged([FromQuery] QueryOptions queryOptions)
        {
            var pagedEngineTypes = await _vehicleEngineTypeService.GetPagedEngineTypes(queryOptions);
            return Ok(pagedEngineTypes);
        }

        // GET: api/VehicleEngineType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IVehicleEngineType>> GetById(int id)
        {
            var engineType = await _vehicleEngineTypeService.GetEngineTypeById(id);
            if (engineType == null)
            {
                return NotFound();
            }
            return Ok(engineType);
        }

        // GET: api/VehicleEngineType/exists/{name}
        [HttpGet("exists/{name}")]
        public async Task<ActionResult<bool>> Exists(string name)
        {
            return await _vehicleEngineTypeService.EngineTypeExistsByName(name);
        }
    }
} 