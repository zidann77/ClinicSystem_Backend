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
        // Inner Class DTO
       
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        private readonly string _encryptionKey;

        public clsPerson()
        {
            _encryptionKey = clsSecuritySettings.GetEncryptionKey() ?? string.Empty;
            this.ID = -1;
            Mode = enMode.AddNew;
        }

        // Constructor يحول من الـ Inner DTO إلى البزنس أوبجكت
        private clsPerson(PeopleDTO dto)
        {
            _encryptionKey = clsSecuritySettings.GetEncryptionKey() ?? string.Empty;
            this.ID = dto.ID;
            this.FirstName = dto.FirstName;
            this.SecondName = dto.SecondName;
            this.LastName = dto.LastName;

            // فك التشفير
            this.Phone = !string.IsNullOrEmpty(dto.Phone)
                ? clsAesEncryptionService.Decrypt(dto.Phone, _encryptionKey)
                : string.Empty;

            this.Email = !string.IsNullOrEmpty(dto.Email)
                ? clsAesEncryptionService.Decrypt(dto.Email, _encryptionKey)
                : string.Empty;

            Mode = enMode.Update;
        }

        public static clsPerson? Find(int ID)
        {
            // نستخدم الـ DTO الموجود في الـ Data Access لجلب البيانات أولاً
            var dataDto = clsPeopleDataAccess.GetPersonById(ID);

            if (dataDto != null)
            {
                // تحويل من DTO الداتا أكسس إلى Inner DTO البزنس
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
                Phone = clsAesEncryptionService.Encrypt(this.Phone, _encryptionKey),
                Email = clsAesEncryptionService.Encrypt(this.Email, _encryptionKey)
            };

            this.ID = clsPeopleDataAccess.AddPerson(dto);
            return (this.ID != -1);
        }

        private bool _UpdatePerson()
        {
            var dto = new PeopleDTO
            {
                ID = this.ID,
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                LastName = this.LastName,
                Phone = clsAesEncryptionService.Encrypt(this.Phone, _encryptionKey),
                Email = clsAesEncryptionService.Encrypt(this.Email, _encryptionKey)
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

        public static List<PeopleDTO> GetAllPeople() => clsPeopleDataAccess.GetAllPeople();
    }
}