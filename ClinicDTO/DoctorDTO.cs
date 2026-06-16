using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class DoctorDTO
    {
        public int ID { get; set; }
        public int PersonID { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool Available { get; set; }
    }
}
