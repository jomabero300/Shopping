using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using TSShopping.Common;

namespace TSShopping.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _conf;

        public MailHelper(IConfiguration configuration)
        {
            _conf = configuration;
        }
        public Response SendMail(string to, string subject, string body, MemoryStream attachment = null)
        {
            string from = _conf["Mail:From"];
            string smtp = _conf["Mail:Smtp"];
            int port = int.Parse(_conf["Mail:Port"]);
            string password = _conf["Mail:Password"];
            string name = _conf["Mail:Name"];

            try
            {
                // Smtp client
                var client = new SmtpClient
                {
                    Port = port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = smtp,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(from, password)
                };
                using (client)
                {
                    var mail = new MailMessage
                    {
                        To = { new MailAddress(to) },
                        From = new MailAddress(from, name),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    if (attachment != null)
                    {
                        mail.Attachments.Add(new Attachment(attachment, "Araucactiva.pdf", MediaTypeNames.Application.Pdf));
                    }

                    client.Send(mail);
                    mail.Dispose();
                }

                return new Response { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}