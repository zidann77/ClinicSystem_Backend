using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class UserDTO 
    {
        public int ID { get; set; }
        public int PersonID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? LastSeen { get; set; } 
    }
}
