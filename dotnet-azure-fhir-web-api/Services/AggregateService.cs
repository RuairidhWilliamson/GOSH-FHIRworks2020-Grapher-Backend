using System;
using HDR_UK_Web_Application.IServices;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HDR_UK_Web_Application.Services
{
    public class AggregateService : IAggregateService
    {
        private static readonly string requestOption = "/Patient/";
        private readonly IResourceFetchService _resource;
        private readonly ILoggerManager _logger;

        public AggregateService(IResourceFetchService resource, ILoggerManager logger)
        {
            _resource = resource;
            _logger = logger;
        }

        private static int GetYears(TimeSpan t)
        {
            return (new DateTime(1, 1, 1) + t).Year - 1;
        }

        private async Task<IEnumerable<JToken>> GetAllPatients()
        {
            List<JObject> allPatients = await _resource.GetAllPages(requestOption);
            return allPatients
                .SelectMany(page => page["entry"], (page, entry) => entry);
        }

        public async Task<List<JObject>> GetAggregateAge(int groupSize)
        {
            _logger.LogInfo("Class: AggregateService, Method: GetAggregateAge");
            return (await GetAllPatients())
                .Select(entry => entry["resource"]["birthDate"])
                .Where(el => el != null)
                .Select(el => DateTime.Now - DateTime.Parse(el.Value<string>()))
                .Select(GetYears)
                .GroupBy(i => groupSize * (i / groupSize))
                .Select(i => JObject.FromObject(new {key = i.Key, freq = i.Count()}))
                .OrderBy(i => i["key"])
                .ToList();
        }

        public async Task<List<JObject>> GetAggregateGender()
        {
            _logger.LogInfo("Class: AggregateService, Method: GetAggregateGender");
            return (await GetAllPatients())
                .Select(entry => entry["resource"]["gender"])
                .Where(el => el != null)
                .GroupBy(i => i)
                .Select(i => JObject.FromObject(new {key = i.Key, freq = i.Count()}))
                .ToList();
        }

        public async Task<List<JObject>> GetAggregatePatientCustom(string path)
        {
            _logger.LogInfo("Class: AggregateService, Method: GetAggregateGender");
                return (await GetAllPatients())
                    .Select(entry => GetPath(entry, path))
                    .Where(el => el != null)
                    .GroupBy(i => i)
                    .Select(i => JObject.FromObject(new {key = i.Key, freq = i.Count()}))
                    .OrderBy(i => i["key"])
                    .ToList();
            
        }

        private static JToken GetPath(JToken entry, string path)
        {
            try
            {
                return path.Split(".").Aggregate(entry, GetIndex);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        private static JToken GetIndex(JToken token, string index)
        {
            return int.TryParse(index, out int result) ? token[result] : token[index];
        }
    }
}
