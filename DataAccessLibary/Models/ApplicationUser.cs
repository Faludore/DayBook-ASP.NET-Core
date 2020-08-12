using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibary.Models
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }

        
        public string InviteCode { get; set; }
        
        public string DnTDelete { get; set; }

        public ICollection<Record> records { get; set; }
        public ApplicationUser()
        {
            records = new List<Record>();
        }
    }
}
