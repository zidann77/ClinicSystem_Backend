using ClinicDataAccess;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicDTO;

namespace ClinicBusinessLogic
{
    public class clsPerson
    {
       
       
        public  enum enMode { AddNew = 0, Update = 1 };
        public  enMode Mode = enMode.AddNew;

        public int PersonID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ? Email { get; set; } = string.Empty;


        public clsPerson()
        {
            this.PersonID = -1;
            Mode = enMode.AddNew;
        }

       
        private clsPerson(PeopleDTO dto)
        {
            this.PersonID = dto.ID;
            this.FirstName = dto.FirstName;
            this.SecondName = dto.SecondName;
            this.LastName = dto.LastName;

           
            this.Phone = !string.IsNullOrEmpty(dto.Phone)
                ? dto.Phone
                : string.Empty;

            this.Email = !string.IsNullOrEmpty(dto.Email)
                ? dto.Email
                : string.Empty;

            Mode = enMode.Update;
        }

        public static clsPerson? Find(int ID)
        {
            
            var dataDto = clsPeopleDataAccess.GetPersonById(ID);

            if (dataDto != null)
            {
               
                return new clsPerson(new PeopleDTO
                {
                    ID = dataDto.ID,
                    FirstName = dataDto.FirstName,
                    SecondName = dataDto.SecondName,
                    LastName = dataDto.LastName,
                    Phone = dataDto.Phone,
                    Email = dataDto.Email
                });
            }
            return null;
        }

        private bool _AddNewPerson()
        {
           
            var dto = new PeopleDTO
            {
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                LastName = this.LastName,
                Phone = this.Phone,
                Email = string.IsNullOrEmpty(this.Email) ? string.Empty: this.Email
            };

            this.PersonID = clsPeopleDataAccess.AddPerson(dto);
            return (this.PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            var dto = new PeopleDTO
            {
                ID = this.PersonID,
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                LastName = this.LastName,
                Phone = this.Phone,
                Email = string.IsNullOrEmpty(this.Email) ? string.Empty : this.Email
            };

            return clsPeopleDataAccess.UpdatePerson(dto);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson()) { Mode = enMode.Update; return true; }
                    return false;
                case enMode.Update:
                    return _UpdatePerson();
            }
            return false;
        }

        public static bool DeletePerson(int ID) => clsPeopleDataAccess.DeletePerson(ID);

        public static List<PeopleDTO> GetAllPeople()
        {
           return clsPeopleDataAccess.GetAllPeople();   
           
        }

        public PeopleDTO ToDTO() => new PeopleDTO
        {
            ID = this.PersonID,
            FirstName = this.FirstName,
            SecondName = this.SecondName,
            LastName = this.LastName,
            Phone = this.Phone,
            Email = string.IsNullOrEmpty(this.Email) ? string.Empty : this.Email
        };
    }
}