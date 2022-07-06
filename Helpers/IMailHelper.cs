using TSShopping.Common;

namespace TSShopping.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string subject, string body, MemoryStream attachment = null);        
    }
}