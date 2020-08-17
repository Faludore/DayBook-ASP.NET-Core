using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAngularIdentity.Models
{
    public class Settings
    {
        private readonly IConfiguration configuration;

        public Settings(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public int WorkersCount => configuration.GetValue<int>("WorkersCount");    
        public string InstanceName => configuration.GetValue<string>("name");
      
    }
}
