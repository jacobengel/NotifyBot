using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifyBot.Utility
{
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
        public Document Add(string message)
        {
            var parsedMessage = Parser.SplitOnFirstWord(message);
            var notification = new Notification { Id = parsedMessage.Item1, Type = "email", Recipients = parsedMessage.Item2 };
            var documentTask = dataRepository.CreateDocumentAsync(notification.Id, notification);
            documentTask.Wait();
            if (documentTask.Result != null)
            {
                return documentTask.Result;
            }
            throw new Exception("That notification alias already exists");
        }

        public Document Update(string message)
        {
            throw new Exception("That notification alias doesn't exists");
        }

        public Document Email(string documentId, string message)
        {
            throw new Exception("That notification alias doesn't exists");
        }

        public void Dispose()
        {
            dataRepository.Dispose();
        }
    }
}