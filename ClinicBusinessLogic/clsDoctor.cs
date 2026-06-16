using System;
using System.Collections.Generic;
using System.Data;
using ClinicDataAccess;
using ClinicDTO;

namespace ClinicBusinessLogic
{
    public class clsDoctor 
    {
        public  enum enMode { AddNew = 0, Update = 1 }
        public  enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool Available { get; set; }

        public clsPerson PersonINFO { get; set; } = new clsPerson();


        public DoctorDTO DTO
        {
            get
            {
                return new DoctorDTO
                {
                    ID = this.ID,
                    PersonID = this.PersonINFO.PersonID,
                    Specialization = this.Specialization,
                    Notes = this.Notes,
                    Available = this.Available
                };
            }
        }

        public DoctorFullDTO FullDTO
        {
            get
            {
                return new DoctorFullDTO
                {
                    ID = this.ID,
                    PersonID = this.PersonINFO.PersonID,
                    FirstName = this.PersonINFO.FirstName,
                    SecondName = this.PersonINFO.SecondName,
                    LastName = this.PersonINFO.LastName,
                    Phone = this.PersonINFO.Phone,
                    Email = this.PersonINFO.Email ?? string.Empty,
                    Specialization = this.Specialization,
                    Notes = this.Notes,
                    Available = this.Available
                };
            }
        }



        public clsDoctor()
        {
            this.ID = -1;
          this.PersonINFO = new clsPerson();
            this.Specialization = string.Empty;
            this.Notes = string.Empty;
            this.Available = false;

            Mode = enMode.AddNew;
        }

       
        private clsDoctor(DoctorDTO DTO)
        {
            this.ID = DTO.ID;
          this.PersonINFO = clsPerson.Find(DTO.PersonID) ?? new clsPerson();
            this.Specialization = DTO.Specialization ?? string.Empty;
            this.Notes = DTO.Notes ?? string.Empty;
            this.Available = DTO.Available;

            Mode = enMode.Update;
        }

        
        public  static clsDoctor? Find(int id)
        {
            DoctorDTO? DTO = clsDoctorDataAccess.GetDoctorById(id);

            if (DTO != null)
            {
                return new clsDoctor(DTO);
            }
            return null;
        }

      
        private bool _Add()
        {
            this.ID = clsDoctorDataAccess.AddDoctor(this.DTO);
            return (this.ID != -1);
        }

       
        private bool _Update()
        {
            return clsDoctorDataAccess.UpdateDoctor(this.DTO);
        }

       
        public  bool Save()
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
            return clsDoctorDataAccess.DeleteDoctor(id);
        }

      
        public static List<DoctorFullDTO> GetAllDoctorsView()
        {
            return clsDoctorDataAccess.GetAllDoctorsView();
        }

        public static List<DoctorDTO> GetAllDoctors()
        {
            return clsDoctorDataAccess.GetAllDoctors();
        }

    }
}