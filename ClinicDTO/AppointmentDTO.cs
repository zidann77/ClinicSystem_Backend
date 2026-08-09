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

        private int? _MedicalRecordID;
        private int? _InvoiceID;

        public int? MedicalRecordID
        {
            get => _MedicalRecordID;
            set => _MedicalRecordID = value == 0 ? null : value;
        }

        public int? InvoiceID
        {
            get => _InvoiceID;
            set => _InvoiceID = value == 0 ? null : value;
        }
    }
}
