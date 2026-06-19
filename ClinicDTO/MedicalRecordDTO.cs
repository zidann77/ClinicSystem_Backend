using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class MedicalRecordDTO
    {
        public int ID { get; set; }

       
        public int AppointmentID { get; set; }

       
        public string Diagnosis { get; set; } = string.Empty;

        
        public string? Prescription { get; set; }

       
        public string? Notes { get; set; }
    }
}
