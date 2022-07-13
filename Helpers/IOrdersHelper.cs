using TSShopping.Common;
using TSShopping.Models;

namespace TSShopping.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> CancelOrderAsync(int id);
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);
    }
}