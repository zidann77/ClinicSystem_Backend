using ClinicDataAccess;
using ClinicDTO;
using SecurityLayer;
using System;
using System.Collections.Generic;

namespace ClinicBusinessLogic
{
    public class clsUser 
    {
        public  enum enMode { AddNew = 0, Update = 1 };
        public  enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? LastSeen { get; set; }

        public clsPerson PersonINFO { get; set; } = new clsPerson();

        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

       
        public clsUser()
        {
            
            this.UserID = -1;
           PersonINFO = new clsPerson();
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Active = true;
            this.LastSeen = null;
            this.PersonINFO = new clsPerson();
            Mode = enMode.AddNew;
        }

        private clsUser(UserDTO dto)
        {
           
            this.UserID = dto.ID;
            this.PersonINFO = clsPerson.Find(dto.PersonID) ?? new clsPerson();
            this.UserName = dto.UserName;
            this.Active = dto.Active;
            this.LastSeen = dto.LastSeen;
            this.Password = string.Empty;
            Mode = enMode.Update;
        }

        private clsUser(UserFullDTO dto)
        {
            
            this.UserID = dto.ID;
            this.PersonINFO = clsPerson.Find(dto.PersonID) ?? new clsPerson();
            this.UserName = dto.UserName;
            this.Active = dto.Active;
            this.LastSeen = dto.LastSeen;
            this.Password = string.Empty;
            this.FirstName = dto.FirstName;
            this.SecondName = dto.SecondName;
            this.LastName = dto.LastName;
            this.Email = string.IsNullOrEmpty(dto.Email) ? string.Empty : dto.Email; // can be empty not null
            this.Phone = string.IsNullOrEmpty(dto.Phone) ? string.Empty : dto.Phone;
            this.Mode = enMode.Update;
        }

        public  static clsUser? Find(int ID)
        {
            var dto = clsUserDataAccess.GetUserByID(ID);
            return (dto != null) ? new clsUser(dto) : null;
        }

        private bool _AddNewUser()
        {
            var dto = new UserDTO
            {
                PersonID = PersonINFO.PersonID,
                UserName = this.UserName,
                Password = this.Password,
                Active = this.Active,
                LastSeen = this.LastSeen
            };

            this.UserID = clsUserDataAccess.AddUser(dto);
            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            var dto = new UserDTO
            {
                ID = this.UserID,
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
                    PersonINFO.Mode = clsPerson.enMode.AddNew;
                    if (!PersonINFO.Save())
                        return false;
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                  
                    if (!PersonINFO.Save())
                        return false;
                    return _UpdateUser();
            }
            return false;
        }

        public static bool DeleteUser(int ID)
            => clsUserDataAccess.DeleteUser(ID);

        public static List<UserDTO> GetAllUsers()
        {
            return clsUserDataAccess.GetAllUsers();

        }

        /// <summary>
        /// METHOD: GetAllUsers_FullData
        /// PURPOSE: Retrieves all users with their full combined profile details from the database.
        /// DESIGN: Maps a list of flat <see cref="UserFullDTO"/> data-containers into fully-behaved <see cref="clsUser"/> business objects.
        /// </summary>
        /// <returns>A <see cref="List{clsUser}"/> containing full user objects, or an empty list if no records exist.</returns>
        /// <remarks>
        /// PERFORMANCE: Utilizes an iterative mapping approach. For massive datasets, consider 
        /// asynchronous streaming or memory optimization patterns.
        /// </remarks>
        public static List<UserFullDTO> GetAllUsers_FullDAata()
        {
            return clsUserDataAccess.GetAllUsersView();
           
        }

        // login

        public static clsUser? Login(string userName, string password)
        {
            UserDTO? User = clsUserDataAccess.LogInUser(userName, password);

            if (User != null)
            {
                return new clsUser(User);
            }
            return null;

        }


        public static bool IsUserNameExists(string username) // prevent duplicate username when adding new user
            => clsUserDataAccess.IsUserNameExists(username);

        public UserDTO ToUserDTO()
        {
            return new UserDTO
            {
                ID = this.UserID,
                PersonID = PersonINFO.PersonID,
                UserName = this.UserName,
                Active = this.Active,
                LastSeen = this.LastSeen
            };

        }

        public UserFullDTO ToUserFullDTO()
        {
            return new UserFullDTO
            {
                ID = this.UserID,
                PersonID = PersonINFO.PersonID,
                UserName = this.UserName,
                Active = this.Active,
                LastSeen = this.LastSeen,
                FirstName = this.FirstName,
                SecondName = this.SecondName,
                LastName = this.LastName,
                Email = this.Email != null ? this.Email : string.Empty,
                Phone = this.Phone
            };

        }
    }
}