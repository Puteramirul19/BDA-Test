using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;

namespace BDA.Entities
{
    public class EmailTemplates : Entity<string>
    {
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailTemplates()
        {
            Body = "";
            Description = "";
            Subject = "";
        }

        public MailMessage Parse(IDictionary<string, string> replacements)
        {
            var sbSubject = new StringBuilder(Subject ?? "");
            var sbBody = new StringBuilder(Body ?? "");

            foreach (var rep in replacements)
            {
                var placeholder = "{{" + rep.Key + "}}";
                sbSubject.Replace(placeholder, rep.Value);
                sbBody.Replace(placeholder, rep.Value);
            }

            return new MailMessage
            {
                Subject = sbSubject.ToString(),
                Body = sbBody.ToString(),
                IsBodyHtml = true,
            };
        }

    }
}
