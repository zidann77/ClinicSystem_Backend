using ClinicDTO;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{
    public class clsRoleDataAccess
    {
        public static int AddRole(RoleDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("rol.Insert_Role", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RoleName", dto.RoleName);

            con.Open();
            object result = cmd.ExecuteScalar();

            return (result != null && int.TryParse(result.ToString(), out int insertedId)) ? insertedId : -1;
        }

        public static RoleDTO? GetRoleById(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("rol.Get_RoleByID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new RoleDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    RoleName = Convert.ToString(reader["RoleName"]) ?? string.Empty
                };
            }

            return null;
        }

        public static List<RoleDTO> GetAllRoles()
        {
            List<RoleDTO> list = new List<RoleDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("rol.GetAll_Roles", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new RoleDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    RoleName = Convert.ToString(reader["RoleName"]) ?? string.Empty
                });
            }

            return list;
        }

        public static bool UpdateRole(RoleDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("rol.Update_Role", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", dto.ID);
            cmd.Parameters.AddWithValue("@RoleName", dto.RoleName);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool DeleteRole(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("rol.Delete_Role", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
