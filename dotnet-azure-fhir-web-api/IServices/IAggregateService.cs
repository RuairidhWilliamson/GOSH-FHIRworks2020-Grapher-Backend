using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HDR_UK_Web_Application.IServices
{
    public interface IAggregateService
    {
        Task<List<JObject>> GetAggregateAge(int groupSize);

        Task<List<JObject>> GetAggregateGender();

        Task<List<JObject>> GetAggregatePatientCustom(string path);
    }
}
