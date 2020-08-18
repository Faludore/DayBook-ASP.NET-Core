using DataAccessLibary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAngularIdentity.Services.Sender
{
    public interface IEmailSender
    {
        Task Send(Email email);
    }
}
