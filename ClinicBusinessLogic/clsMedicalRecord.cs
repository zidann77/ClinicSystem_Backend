using System;
using System.Collections.Generic;
using ClinicDataAccess;
using ClinicDTO;

namespace ClinicBusinessLogic
{
    public class clsMedicalRecord
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int AppointmentID { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string? Prescription { get; set; }
        public string? Notes { get; set; }

       
        public MedicalRecordDTO DTO
        {
            get
            {
                return new MedicalRecordDTO
                {
                    ID = this.ID,
                    AppointmentID = this.AppointmentID,
                    Diagnosis = this.Diagnosis,
                    Prescription = this.Prescription,
                    Notes = this.Notes
                };
            }
        }

        
        public clsMedicalRecord()
        {
            this.ID = -1;
            this.AppointmentID = -1;
            this.Diagnosis = string.Empty;
            this.Prescription = null;
            this.Notes = null;

            Mode = enMode.AddNew;
        }

      
        private clsMedicalRecord(MedicalRecordDTO DTO)
        {
            this.ID = DTO.ID;
            this.AppointmentID = DTO.AppointmentID;
            this.Diagnosis = DTO.Diagnosis;
            this.Prescription = DTO.Prescription;
            this.Notes = DTO.Notes;

            Mode = enMode.Update;
        }

       
        public static clsMedicalRecord? Find(int id)
        {
            MedicalRecordDTO? DTO = clsMedicalRecordDataAccess.GetMedicalRecordById(id);

            if (DTO != null)
            {
                return new clsMedicalRecord(DTO);
            }
            return null;
        }

      
        private bool _Add()
        {
            this.ID = clsMedicalRecordDataAccess.AddMedicalRecord(this.DTO);
            return (this.ID != -1);
        }

       
        private bool _Update()
        {
            return clsMedicalRecordDataAccess.UpdateMedicalRecord(this.DTO);
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
            return clsMedicalRecordDataAccess.DeleteMedicalRecord(id);
        }

    
        public static List<MedicalRecordDTO> GetAllMedicalRecords()
        {
            return clsMedicalRecordDataAccess.GetAllMedicalRecords();
        }
    }
}