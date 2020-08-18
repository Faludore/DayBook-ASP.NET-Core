using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAngularIdentity.Settings
{
    public class Config
    {
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPost { get; set; }
    }
}
