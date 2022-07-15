using TSShopping.Common;

namespace TSShopping.Helpers
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }
}