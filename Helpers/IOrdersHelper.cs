using TSShopping.Common;
using TSShopping.Models;

namespace TSShopping.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);
    }
}