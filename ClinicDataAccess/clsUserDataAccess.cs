using ClinicDTO;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClinicDataAccess
{
    public class clsUserDataAccess
    {
        private readonly static string EncryptionKey = clsSecuritySettings.GetEncryptionKey();
        public static List<UserDTO> GetAllUsers()
        {
            var list = new List<UserDTO>();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
                            UserName = reader["UserName"].ToString() ?? string.Empty,
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,
                            Password = string.Empty // لا نرجع الباسورد في القوائم
                        });
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
                            UserName = reader["UserName"].ToString() ?? string.Empty,
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,//Safe Casting أو الـ Safe Type Conversion
                            FirstName = reader["FirstName"].ToString() ?? string.Empty,
                            SecondName = reader["SecondName"].ToString() ?? string.Empty,
                            LastName = reader["LastName"].ToString() ?? string.Empty,
                            Phone = clsAesEncryptionService.Decrypt(reader["Phone"]?.ToString() ?? string.Empty, EncryptionKey),
                            Email = (reader["Email"] == DBNull.Value || string.IsNullOrWhiteSpace(reader["Email"].ToString())) ? string.Empty : clsAesEncryptionService.Decrypt(reader["Email"].ToString() ?? string.Empty, EncryptionKey)
                        });
                    }
                }
            }
            return list;
        }

        
        public static string GetHashedPassword(string userName, int ID)
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

     
        public static int AddUser(UserDTO user)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.Insert_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", user.PersonID);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Password", clsPasswordHasher.HashPassword(user.Password));
            cmd.Parameters.AddWithValue("@Active", user.Active);
            cmd.Parameters.AddWithValue("@LastSeen", user.LastSeen ?? (object)DBNull.Value);

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return (outputId.Value != DBNull.Value) ? (int)outputId.Value : -1;
        }

      
        public static UserDTO? GetUserByID(int id)
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
                            UserName = reader["UserName"].ToString() ?? string.Empty,
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,
                            Password = string.Empty 
                        };
                    }
                }
            }

            return null;
        }

        public static UserDTO? GetUserByUserName(string username)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("usr.GetUserByUserName", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", username);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserDTO
                        {
                            ID = (int)reader["ID"],
                            PersonID = (int)reader["PersonID"],
                            UserName = reader["UserName"].ToString() ?? string.Empty,
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,
                            Password = string.Empty 
                        };
                    }
                }
            }
            return null;
        }

      
        public static bool UpdateUserBasicInfo(UserDTO user)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.Update_User", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", user.ID);
            cmd.Parameters.AddWithValue("@UserName", user.UserName);
            cmd.Parameters.AddWithValue("@Active", user.Active);
            cmd.Parameters.AddWithValue("@LastSeen", user.LastSeen.HasValue ? user.LastSeen.Value: DBNull.Value);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

      public static bool IsUserNameExists(string username)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.IsUserNameExist", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", username );
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

        public static UserDTO? LogInUser(string UserName , string password)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("usr.LoginUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username",UserName);

            con.Open();

            using( SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    if (clsPasswordHasher.VerifyPassword(password, reader["StoredHash"].ToString() ?? string.Empty))
                        return null;
                    else
                    {
                        return new UserDTO
                        {
                            ID = (int)reader["ID"],
                            UserName = reader["UserName"].ToString() ?? string.Empty,
                            Active = (bool)reader["Active"],
                            LastSeen = reader["LastSeen"] as DateTime?,
                            Password = reader["StoredHash"]?.ToString() ?? string.Empty
                        };
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}