using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class PatientDTO
    {
        public int ID { get; set; }

        public int PersonID { get; set; }

        public byte Status { get; set; }

        public byte? Age { get; set; } 

        public string? Notes { get; set; } 
    }
}