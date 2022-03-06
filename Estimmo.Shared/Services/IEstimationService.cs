using Estimmo.Shared.Entities;

namespace Estimmo.Shared.Services
{
    public interface IEstimationService
    {
        public Task<Estimate> GetEstimateAsync(EstimateRequest request);
    }
}
