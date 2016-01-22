using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotifyBot.Controllers
{
    using NotifyBot.Models;

    public class NotifyController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostNotification(NotifyRequest request)
        {
            HttpResponseMessage responseMessage;
            try
            {
                var text = request.Item.message.message.Substring(8);

                var message = new NotifyResponse
                {
                    color = "green",
                    message = "It's going to be sunny tomorrow! (" + text + ")",
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
    }
}
