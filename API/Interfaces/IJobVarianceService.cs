using API.Models.InforErp;

namespace API.Interfaces
{
    public interface IJobVarianceService
    {
        Task<JobVariance[]> ConstructJobVarianceReport(string? filter, int rowCount, string bearerToken);
    }
}
