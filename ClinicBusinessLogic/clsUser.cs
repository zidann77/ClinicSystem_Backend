using ClinicDataAccess;
using ClinicDTO;
using SecurityLayer;
using System;
using System.Collections.Generic;

namespace ClinicBusinessLogic
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public int PersonID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? LastSeen { get; set; }

        public clsUser()
        {
            this.ID = -1;
            this.PersonID = -1;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Active = true;
            this.LastSeen = null;
            Mode = enMode.AddNew;
        }

        private clsUser(UserDTO dto)
        {
            this.ID = dto.ID;
            this.PersonID = dto.PersonID;
            this.UserName = dto.UserName;
            this.Active = dto.Active;
            this.LastSeen = dto.LastSeen;
            this.Password = string.Empty;
            Mode = enMode.Update;
        }

        public static clsUser Find(int ID)
        {
            var dto = clsUserDataAccess.GetUserByID(ID);
            return (dto != null) ? new clsUser(dto) : null;
        }

        private bool _AddNewUser()
        {
            var dto = new UserDTO
            {
                PersonID = this.PersonID,
                UserName = this.UserName,
                Password = clsPasswordHasher.HashPassword(this.Password),
                Active = this.Active,
                LastSeen = this.LastSeen
            };

            this.ID = clsUserDataAccess.AddUser(dto);
            return (this.ID != -1);
        }

        private bool _UpdateUser()
        {
            var dto = new UserDTO
            {
                ID = this.ID,
                PersonID = this.PersonID,
                UserName = this.UserName,
                Active = this.Active,
                LastSeen = this.LastSeen
            };

            return clsUserDataAccess.UpdateUserBasicInfo(dto);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        public static bool DeleteUser(int ID)
            => clsUserDataAccess.DeleteUser(ID);

        public static List<UserDTO> GetAllUsers()
            => clsUserDataAccess.GetAllUsers();

        public static List<UserFullDTO> GetAllUsersView()
            => clsUserDataAccess.GetAllUsersView();

        public static clsUser Login(string userName, string password)
        {
            var users = clsUserDataAccess.GetAllUsers();
            var userDto = users.Find(u => u.UserName == userName);

            if (userDto != null)
            {
                string storedHash = clsUserDataAccess.GetHashedPassword(userName, userDto.ID);
                if (clsPasswordHasher.VerifyPassword(password, storedHash))
                {
                    return Find(userDto.ID);
                }
            }
            return null;
        }
    }
}