using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Mail;
using NotifyBot.Models;
using NotifyBot.Utility;

namespace NotifyBot.Controllers
{
    using Microsoft.Azure.Documents;

    using Newtonsoft.Json;

    using ServiceStack;

    public class NotifyController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostNotification(NotifyRequest request)
        {
            HttpResponseMessage responseMessage;
            var commandHandler = new CommandHandler();
            try
            {
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
                var result = "";

                if (commandResult)
                {
                    switch (command)
                    {
                        case Command.Add:
                            result = commandHandler.Add(message);
                            break;
                        case Command.Update:
                            result = commandHandler.Update(message);
                            break;
                    }
                }
                else
                {
                    var senderName = request.Item.message.from.name;
                    var senderMention = request.Item.message.from.mention_name;
                    result = commandHandler.Email(senderName, senderMention, commandString, message);
                }

                if (result == null)
                {
                    throw new Exception("invalid command");
                }


                // Notify Hipchat
                var responseBody = new NotifyResponse
                {
                    color = "green",
                    message = "It's going to be sunny tomorrow! (" + result + " )",
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateResponse(HttpStatusCode.OK, responseBody);
            }
            catch (Exception ex)
            {
                commandHandler.Dispose();
                var responseBody = new NotifyResponse
                {
                    color = "green",
                    message = ex.Message,
                    message_format = "text",
                    notify = "false"
                };
                responseMessage = this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(responseBody));
            }
            return responseMessage;
        }

        
    }
}
