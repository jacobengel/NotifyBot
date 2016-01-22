using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifyBot.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Recipients { get; set; }
    }
}