using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using HDR_UK_Web_Application.IServices;
using Microsoft.Extensions.Caching.Memory;

namespace HDR_UK_Web_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {

        private readonly IAggregateService _service;
        private readonly IMemoryCache _cache;

        public AggregateController(IAggregateService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        // GET: api/Aggregate/Age/<Size of the Groups>
        [HttpGet("Age/{groupSize}")]
        public async Task<List<JObject>> GetAggregateAge(int groupSize)
        {
            return await _cache.GetOrCreateAsync($"AggregateAge{groupSize}", async entry => await _service.GetAggregateAge(groupSize));
        }
        
        // GET: api/Aggregate/Gender
        [HttpGet("Gender")]
        public async Task<List<JObject>> GetAggregateGender()
        {
            return await _cache.GetOrCreateAsync("AggregateGender", async entry => await _service.GetAggregateGender());
        }
        
        // GET: api/Aggregate/Patient/Custom/<Dot Separated JSON Path>
        [HttpGet("Patient/Custom/{path}")]
        public async Task<List<JObject>> GetAggregateCustom(string path)
        {
            return await _cache.GetOrCreateAsync($"AggregatePatientCustom{path}",
                async entry => await _service.GetAggregatePatientCustom(path));
        }
    }
}
