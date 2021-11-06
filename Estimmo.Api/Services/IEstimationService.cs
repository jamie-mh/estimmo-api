using Estimmo.Api.Entities;
using System.Threading.Tasks;

namespace Estimmo.Api.Services
{
    public interface IEstimationService
    {
        public Task<Estimate> GetEstimateAsync(EstimateRequest request);
    }
}