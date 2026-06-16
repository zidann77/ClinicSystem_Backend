using System;
using System.Collections.Generic;
using ClinicDataAccess;
using ClinicDTO;

namespace ClinicBusinessLogic
{
    public class clsPatient 
    {
      
        public   enum enMode { AddNew = 0, Update = 1 }
        public  enMode Mode = enMode.AddNew;
        public clsPerson PersonINFO { get; set; } = new clsPerson();
        public int ID { get; set; }
        public byte Status { get; set; }
        public byte? Age { get; set; }
        public string Notes { get; set; } = string.Empty;

        // property 
        public PatientDTO DTO
        {
            get
            {
                return new PatientDTO
                {
                    ID = this.ID,
                    PersonID =PersonINFO.PersonID,
                    Status = this.Status,
                    Age = this.Age,
                    Notes = this.Notes
                };
            }
        }

        public PatientFullDTO FullDTO
        {
            get
            {
                return new PatientFullDTO
                {
                    FirstName = this.PersonINFO.FirstName,
                    SecondName = this.PersonINFO.SecondName,
                    LastName = this.PersonINFO.LastName,
                    Phone = this.PersonINFO.Phone,
                    Email = this.PersonINFO.Email ?? string.Empty,
                    Status = this.Status,
                    Age = this.Age,
                    Notes = this.Notes
                };
            }
        }


        public clsPatient()
        {
            this.ID = -1;
            //   this.PersonID = -1;
            this. PersonINFO = new clsPerson();  
            this.Status = 0;
            this.Age = null;
            this.Notes = string.Empty;

            Mode = enMode.AddNew;
        }

        
        private clsPatient(PatientDTO DTO)
        {
            this.ID = DTO.ID;
            //    this.PersonID = DTO.PersonID;
            this.PersonINFO = clsPerson.Find(DTO.PersonID)?? new clsPerson() ;
            this.Status = DTO.Status;
            this.Age = DTO.Age;
            this.Notes = DTO.Notes ?? string.Empty;

            Mode = enMode.Update;
        }

        
        public static clsPatient? Find(int id)
        {
            PatientDTO? DTO = clsPatientDataAccess.GetPatientByID(id);

            if (DTO != null)
            {
                return new clsPatient(DTO); 
            }
            return null;
        }

       
        private bool _Add()
        {
            this.ID = clsPatientDataAccess.AddPatient(this.DTO);
            return (this.ID != -1);
        }

        private bool _Update()
        {
            return clsPatientDataAccess.UpdatePatient(this.DTO);
        }

        public  bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    PersonINFO.Mode = clsPerson.enMode.AddNew;
                    if (!PersonINFO.Save())
                    return false;
                    
                    if (_Add())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    PersonINFO.Mode = clsPerson.enMode.Update;
                    if (!PersonINFO.Save())
                        return false;
                    return _Update();
            }
            return false;
        }

       
        public static bool Delete(int id)
        {
            return clsPatientDataAccess.DeletePatient(id);
        }

       
        public static List<PatientDTO> GetAllPatients()
        {
            return clsPatientDataAccess.GetAllPatients();
        }

      
        public static List<PatientFullDTO> GetAllPatientsView()
        {
            return clsPatientDataAccess.GetAllPatientsView();
        }
    }
}