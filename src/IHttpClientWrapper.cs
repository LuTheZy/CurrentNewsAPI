using System.Net.Http;
using System.Threading.Tasks;

namespace CurrentNewsAPI.Services
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
