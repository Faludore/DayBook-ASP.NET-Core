using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLibary.Models
{
    public class Invite
    {
        
        public int Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string DnT { get; set; }
        public string Status { get; set; }
    }
}
