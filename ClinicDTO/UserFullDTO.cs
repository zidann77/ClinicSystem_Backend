using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class UserFullDTO
    {
        public int ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PersonID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
