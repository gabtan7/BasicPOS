using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string emailSender, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
