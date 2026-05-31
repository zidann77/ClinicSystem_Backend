using ClinicDataAccess;
using ClinicDTO;
using SecurityLayer;
using System;
using System.Collections.Generic;

namespace ClinicBusinessLogic
{
    public class clsUser : clsPerson
    {
        public new enum enMode { AddNew = 0, Update = 1 };
        public new enMode Mode = enMode.AddNew;

        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime? LastSeen { get; set; }

        private readonly string _encryptionKey;

        public clsUser()
        {
            _encryptionKey = clsSecuritySettings.GetEncryptionKey() ?? string.Empty;
            this.UserID = -1;
            this.PersonID = -1;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Active = true;
            this.LastSeen = null;
            Mode = enMode.AddNew;
        }

        private clsUser(UserDTO dto)
        {
            _encryptionKey = clsSecuritySettings.GetEncryptionKey() ?? string.Empty;
            this.UserID = dto.ID;
            this.PersonID = dto.PersonID;
            this.UserName = dto.UserName;
            this.Active = dto.Active;
            this.LastSeen = dto.LastSeen;
            this.Password = string.Empty;
            Mode = enMode.Update;
        }

        private clsUser(UserFullDTO dto)
        {
            _encryptionKey = clsSecuritySettings.GetEncryptionKey() ?? string.Empty;
            this.UserID = dto.ID;
            this.PersonID = dto.PersonID;
            this.UserName = dto.UserName;
            this.Active = dto.Active;
            this.LastSeen = dto.LastSeen;
            this.Password = string.Empty;
            this.FirstName = dto.FirstName;
            this.SecondName = dto.SecondName;
            this.LastName = dto.LastName;
            this.Email = string.IsNullOrEmpty(dto.Email) ? null : clsAesEncryptionService.Decrypt(dto.Email, _encryptionKey);
            this.Phone = clsAesEncryptionService.Decrypt(dto.Phone, _encryptionKey);
            this.Mode = enMode.Update;
        }

        public new static clsUser? Find(int ID)
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

        public new bool Save()
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

        public static List<clsUser> GetAllUsers()
        {
            List<UserDTO> Records = clsUserDataAccess.GetAllUsers();
            List<clsUser> users = new List<clsUser>();

            foreach (UserDTO Record in Records)
            {
                users.Add(new clsUser(Record));
            }
            return users;
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
        public static List<clsUser> GetAllUsers_FullDAata()
        {
            List<UserFullDTO> Records = clsUserDataAccess.GetAllUsersView();
            List<clsUser> users = new List<clsUser>();

            foreach (UserFullDTO Record in Records)
            {
                users.Add(new clsUser(Record));
            }
            return users;
        }

        // login

        public static clsUser? Login(string userName, string password)
        {
            UserDTO? User = clsUserDataAccess.LogInUser(userName);


            if (User != null)
            {
                if (clsPasswordHasher.VerifyPassword(password, User.Password))
                {
                   
                    clsUser user = new clsUser(User);
                    user.LastSeen = DateTime.Now;
                    user.Save();
                    return user;
                }

            }
                return null;

        }


        public static bool IsUserNameExists(string username)
            => clsUserDataAccess.IsUserNameExists(username);

        public UserDTO ToUserDTO()
        {
            return new UserDTO
            {
                ID = this.UserID,
                PersonID = this.PersonID,
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
                PersonID = this.PersonID,
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