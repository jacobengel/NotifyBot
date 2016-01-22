using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using NotifyBot.Models;
using NotifyBot.Utility;

namespace NotifyBot.Controllers
{
    public class NotifyController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostNotification(NotifyRequest request)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var senderName = request.Item.message.from.name;
                var senderMention = request.Item.message.from.mention_name;
                var text = request.Item.message.message.Substring(8);

                
                var to = "dominickaleardi@quickenloans.com";
                var subject = senderName + " AKA " + senderMention + " has notified you!";
                var body = text;

                var dataRepo = new DocumentDbRepository();
                dataRepo.Setup();

                this.sendEmail(to, subject, body);


                var message = new NotifyResponse
                {
                    color = "green",
                    message = "It's going to be sunny tomorrow! (" + text + " )",
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateResponse(HttpStatusCode.OK, message);
            }
            catch (Exception ex)
            {
                responseMessage = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return responseMessage;
        }

        private void sendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            const string FromEmail = "hipchatemailbot@gmail.com";
            const string FromPw = "winlikecharliesheen";
            var toEmail = to;
            message.From = new MailAddress(FromEmail);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(FromEmail, FromPw);

                smtpClient.Send(message.From.ToString(), message.To.ToString(),
                                message.Subject, message.Body);
            }
        }
    }
}
