using ClinicDataAccess;
using ClinicDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBusinessLogic
{
    public class clsRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode;

        public clsRole() { 
            RoleID = -1;
            RoleName = string.Empty;
            Mode = enMode.AddNew;
        }

        private clsRole(RoleDTO dto)
        {
            RoleID = dto.ID;
            RoleName = dto.RoleName;
            Mode = enMode.Update;   
        }

        public RoleDTO DTO
        {
            get
            {
                return new RoleDTO
                {
                    ID = this.RoleID,
                    RoleName = this.RoleName
                };
            }
        }

        public static clsRole? Find(int id)
        {
            RoleDTO? dto = ClinicDataAccess.clsRoleDataAccess.GetRoleById(id);
            if (dto != null)
            {
                return new clsRole(dto);
            }
            return null;
        }

        public static List<clsRole> GetAllRoles()
        {
            List<RoleDTO> dtos = ClinicDataAccess.clsRoleDataAccess.GetAllRoles();
            List<clsRole> roles = new List<clsRole>();
            foreach (var dto in dtos)
            {
                roles.Add(new clsRole(dto));
            }
            return roles;
        }

        public static int AddRole(string roleName)
        {
            RoleDTO dto = new RoleDTO
            {
                RoleName = roleName
            };
            return ClinicDataAccess.clsRoleDataAccess.AddRole(dto);
        }

        public static bool UpdateRole(int roleId, string roleName)
        {
            RoleDTO dto = new RoleDTO
            {
                ID = roleId,
                RoleName = roleName
            };
            return ClinicDataAccess.clsRoleDataAccess.UpdateRole(dto);
        }

        public static bool DeleteRole(int roleId)
        {
            return ClinicDataAccess.clsRoleDataAccess.DeleteRole(roleId);
        }
       
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    this.RoleID = clsRoleDataAccess.AddRole(this.DTO);
                    if (this.RoleID != -1)
                    {
                        Mode = enMode.Update; // State changed to update for subsequent saves
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return clsRoleDataAccess.UpdateRole(this.DTO);
            }
            return false;
        }
    }
}
