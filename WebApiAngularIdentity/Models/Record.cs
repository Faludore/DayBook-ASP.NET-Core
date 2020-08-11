using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAngularIdentity.Models
{
    public class Record
    {       
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string DnT { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Image { get; set; }

    }
}
