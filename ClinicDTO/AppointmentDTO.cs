using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class AppointmentDTO
    {
        public int ID { get; set; }

        public int PatientID { get; set; }

        public DateTime Datetime { get; set; }

        public byte Status { get; set; }

        public string? Notes { get; set; }

        public int DoctorID { get; set; }

        public int? MedicalRecordID { get; set; }

        public int? InvoiceID { get; set; }
    }
}
