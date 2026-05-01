using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using ClinicDTO;

namespace ClinicDataAccess
{
    public class clsUserDataAccess
    {
        public static List<UserDTO> GetAllUsers()
        {
            var list = new List<UserDTO>();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usr.GetAll_Users", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new UserDTO
                            {
                                ID = (int)reader["ID"],
                                PersonID = (int)reader["PersonID"],
                                UserName = reader["UserName"].ToString(),
                                Active = (bool)reader["Active"],
                                LastSeen = reader["LastSeen"] as DateTime?
                            });
                        }
                    }
                }
            }

            return list;
        }
        public static List<UserFullDTO> GetAllUsersView()
        {
            var list = new List<UserFullDTO>();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("usr.GetAll_UsersView", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserFullDTO
                        {
                            ID = (int)reader["ID"],
                            PersonID = (int)reader["PersonID"],
                            UserName = reader["UserName"].ToString(),
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,

                            FirstName = reader["FirstName"].ToString(),
                            SecondName = reader["SecondName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public static string GetHashedPassword(string userName , int ID)
        {
            string passwordHash = string.Empty;

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.GetUserPassword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@Id", ID);
            con.Open();
            object result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
                passwordHash = result.ToString() ?? string.Empty;

            return passwordHash;
        }

        public static int AddUser(UserDTO user , string Password)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.Insert_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", user.PersonID);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@Active", user.Active);

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return (outputId.Value != DBNull.Value) ? (int)outputId.Value : -1;
        }

        public static UserDTO GetUserByID(int id)
        {
          

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("usr.Get_UserByID", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserDTO
                        {
                            ID = (int)reader["ID"],
                            PersonID = (int)reader["PersonID"],
                            UserName = reader["UserName"].ToString(),
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?
                        };
                    }
                }
            }

            return null;
        }
        public static bool UpdateUserBasicInfo(UserDTO user,string Password)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
         

            using SqlCommand cmd = new SqlCommand("usr.Update_User", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", user.ID);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Active", user.Active);
            cmd.Parameters.AddWithValue("@PersonID", user.ID);
            cmd.Parameters.AddWithValue("@Password", Password);
            cmd.Parameters.AddWithValue("@LastSeen", user.LastSeen);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool DeleteUser(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.Delete_User", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}