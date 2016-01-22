﻿using System;
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
                
                var tempString = Parser.SplitOnFirstWord(request.Item.message.message).Item2;
                if (string.IsNullOrEmpty(tempString))
                {
                    throw new Exception("no email distribution alias provided");
                }

                // parse message
                var parsedMessage = Parser.SplitOnFirstWord(tempString);
                
                //get command
                var commandString = parsedMessage.Item1;
                Command command;
                var commandResult = Enum.TryParse(commandString, true, out command);
                
                //get message
                var message = parsedMessage.Item2;
                
                var commandHandler = new CommandHandler();
                if (commandResult)
                {

                    switch (command)
                    {
                        case Command.Add:
                            commandHandler.Add(message);
                            break;
                        case Command.Update:
                            commandHandler.Update();
                            break;
                        default:
                            commandHandler.Email();
                            break;
                    }
                }




                //Send emails
                var to = "dominickaleardi@quickenloans.com";
                var subject = senderName + " AKA " + senderMention + " has notified you!";
                var body = message;

                this.sendEmail(to, subject, body);

                // Notify Hipchat
                var responseBody = new NotifyResponse
                {
                    color = "green",
                    message = "It's going to be sunny tomorrow! (" + message + " )",
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateResponse(HttpStatusCode.OK, responseBody);
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
