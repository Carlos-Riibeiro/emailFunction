using SendGrid.Helpers.Mail;
using System.Collections.Generic;

namespace Poc.EmailFunction.HttpTrigger.Models
{
    public class EmailDetail
    {
        public List<EmailAddress> Tos { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
