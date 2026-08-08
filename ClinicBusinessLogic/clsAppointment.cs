using ClinicDataAccess;
using ClinicDataAccess.ClinicDataAccess;
using ClinicDTO;
using System;
using System.Collections.Generic;

namespace ClinicBusinessLogic
{
    public class clsAppointment
    {
        public enum enMode
        {
            AddNew = 0,
            Update = 1
        }

        public enum enAppointmentStatus
        {
            Pending = 1,
            Cancelled = 2,
            Completed = 3
        }


        public enMode Mode = enMode.AddNew;



        public int ID { get; set; }
        public int PatientID { get; set; }
        public DateTime Datetime { get; set; }
        public byte Status { get; set; }
        public string? Notes { get; set; }
        public int DoctorID { get; set; }
        public int? MedicalRecordID { get; set; }
        public int? InvoiceID { get; set; }



       
        public enAppointmentStatus AppointmentStatus
        {
            get
            {
                return (enAppointmentStatus)Status;
            }

            set
            {
                Status = (byte)value;
            }
        }



       
        public AppointmentDTO DTO
        {
            get
            {
                return new AppointmentDTO
                {
                    ID = this.ID,
                    PatientID = this.PatientID,
                    Datetime = this.Datetime,
                    Status = this.Status,
                    Notes = this.Notes,
                    DoctorID = this.DoctorID,
                    MedicalRecordID = this.MedicalRecordID,
                    InvoiceID = this.InvoiceID
                };
            }
        }



       
        public clsAppointment()
        {
            this.ID = -1;
            this.PatientID = -1;
            this.Datetime = DateTime.Now;
            this.Status = (byte)enAppointmentStatus.Pending;
            this.Notes = null;
            this.DoctorID = -1;
            this.MedicalRecordID = null;
            this.InvoiceID = null;

            Mode = enMode.AddNew;
        }




        
        private clsAppointment(AppointmentDTO DTO)
        {
            this.ID = DTO.ID;
            this.PatientID = DTO.PatientID;
            this.Datetime = DTO.Datetime;
            this.Status = DTO.Status;
            this.Notes = DTO.Notes;
            this.DoctorID = DTO.DoctorID;
            this.MedicalRecordID = DTO.MedicalRecordID;
            this.InvoiceID = DTO.InvoiceID;

            Mode = enMode.Update;
        }





       
        public static clsAppointment? Find(int id)
        {
            AppointmentDTO? DTO = clsAppointmentDataAccess.GetAppointmentById(id);

            if (DTO != null)
            {
                return new clsAppointment(DTO);
            }

            return null;
        }



        private bool _Add()
        {
            this.ID = clsAppointmentDataAccess.AddAppointment(this.DTO);

            return (this.ID != -1);
        }


        private bool _Update()
        {
            return clsAppointmentDataAccess.UpdateAppointment(this.DTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:

                    if (_Add())
                    {
                        Mode = enMode.Update;
                        return true;
                    }

                    return false;



                case enMode.Update:

                    return _Update();
            }


            return false;
        }

        public static bool Delete(int id)
        {
            return clsAppointmentDataAccess.DeleteAppointment(id);
        }
        public static List<AppointmentDTO> GetAllAppointments()
        {
            return clsAppointmentDataAccess.GetAllAppointments();
        }
    }
}