using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifyBot.Utility
{
    using System.Net;
    using System.Net.Mail;

    using Microsoft.Azure.Documents;

    using NotifyBot.Models;

    public enum Command
    {
        Add,
        Update
    }
    public class CommandHandler
    {
        private static DocumentDbRepository dataRepository;
        public CommandHandler()
        {
            dataRepository = new DocumentDbRepository();
            dataRepository.Setup();
        }
        public string Add(string message)
        {
            var parsedMessage = Parser.SplitOnFirstWord(message);
            var notification = new Notification { Id = parsedMessage.Item1, Type = "email", Recipients = parsedMessage.Item2 };
            var documentTask = dataRepository.CreateDocumentAsync(notification.Id, notification);
            documentTask.Wait();
            if (documentTask.Result != null)
            {
                return "Added Successfully";
            }
            throw new Exception("That notification alias already exists");
        }

        public string Update(string message)
        {
            throw new Exception("That notification alias doesn't exists");
        }

        public string Email(string senderName, string senderMention, string documentId, string message)
        {
            var document = dataRepository.GetDocument(documentId);

            if (document == null)
            {
                throw new Exception("That notification alias doesn't exists");
            }
            //Send emails
            var to = Newtonsoft.Json.JsonConvert.DeserializeObject<Notification>(document.ToString()).Recipients;
            var subject = senderName + " AKA " + senderMention + " has notified you!";
            var body = message;

            this.sendEmail(to, subject, body);
            return "Email sent successfully";

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

        public void Dispose()
        {
            dataRepository.Dispose();
        }
    }
}