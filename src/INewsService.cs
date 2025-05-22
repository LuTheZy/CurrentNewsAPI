using System.Threading.Tasks;
using NewsAggregatorLambda.Models;

namespace NewsAggregatorLambda
{
    public interface INewsService
    {
        Task<NewsResponse> GetTopHeadlinesAsync();
    }
}