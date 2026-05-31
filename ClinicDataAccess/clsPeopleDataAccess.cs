using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicDTO;

namespace ClinicDataAccess
{
    public class clsPeopleDataAccess
    {
       
        // INSERT
        public static int AddPerson(PeopleDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("ppl.Insert_People", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
            cmd.Parameters.AddWithValue("@SecondName", dto.SecondName);
            cmd.Parameters.AddWithValue("@LastName", dto.LastName);
            cmd.Parameters.AddWithValue("@Phone", dto.Phone);
            cmd.Parameters.AddWithValue("@Email", dto.Email);

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return outputId.Value != DBNull.Value ? (int)outputId.Value : -1;
        }

        // GET ALL
        public static List<PeopleDTO> GetAllPeople()
        {
            List<PeopleDTO> list = new List<PeopleDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("ppl.GetALL_People", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PeopleDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    FirstName = reader["FirstName"].ToString() ?? string.Empty,
                    SecondName = reader["SecondName"].ToString() ?? string.Empty,
                    LastName = reader["LastName"].ToString() ?? string.Empty,
                    Phone = reader["Phone"].ToString() ?? string.Empty,
                    Email = reader["Email"].ToString() ?? string.Empty
                });
            }

            return list;
        }

        // GET BY ID
        public static PeopleDTO? GetPersonById(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("ppl.GetByID_People", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new PeopleDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    FirstName = reader["FirstName"].ToString() ?? string.Empty,
                    SecondName = reader["SecondName"].ToString() ?? string.Empty,
                    LastName = reader["LastName"].ToString() ?? string.Empty,
                    Phone = reader["Phone"].ToString() ?? string.Empty,
                    Email = reader["Email"].ToString() ?? string.Empty
                };
            }
           
                return null;
        }

        // UPDATE
        public static bool UpdatePerson(PeopleDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("ppl.Update_People", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", dto.ID);
            cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
            cmd.Parameters.AddWithValue("@SecondName", dto.SecondName);
            cmd.Parameters.AddWithValue("@LastName", dto.LastName);
            cmd.Parameters.AddWithValue("@Phone", dto.Phone);
            cmd.Parameters.AddWithValue("@Email", dto.Email);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        // DELETE
        public static bool DeletePerson(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("ppl.Delete_People", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }
    }
    }
