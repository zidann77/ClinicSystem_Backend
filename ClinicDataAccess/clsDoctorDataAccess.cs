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
    public class clsDoctorDataAccess
    {
        
        public static int AddDoctor(DoctorDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.Insert_Doctor", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", dto.PersonID);
            cmd.Parameters.AddWithValue("@Specialization", (object)dto.Specialization ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Available", dto.Available);

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return outputId.Value != DBNull.Value ? (int)outputId.Value : -1;
        }

        public static DoctorDTO? GetDoctorById(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.Get_DoctorsByID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new DoctorDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    PersonID = Convert.ToInt32(reader["PersonID"]),
                    Specialization = reader["Specialization"] != DBNull.Value ? reader["Specialization"].ToString() ?? string.Empty : string.Empty,
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() ?? string.Empty : string.Empty,
                    Available = Convert.ToBoolean(reader["Available"])
                };
            }

            return null;
        }

     
        public static bool UpdateDoctor(DoctorDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.Update_Doctor", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", dto.ID);
            cmd.Parameters.AddWithValue("@PersonID", dto.PersonID);
            cmd.Parameters.AddWithValue("@Specialization", (object)dto.Specialization ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Available", dto.Available);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool DeleteDoctor(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.Delete_Doctor", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        public static List<DoctorDTO> GetAllDoctors()
        {
            List<DoctorDTO> List = new List<DoctorDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.GetAll_Doctors", con);

            cmd.CommandType = CommandType.StoredProcedure;

            using SqlDataReader reader = cmd.ExecuteReader();
             con.Open();

            while(reader.Read())
            {
                List.Add(new DoctorDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    PersonID = Convert.ToInt32(reader["PersonID"]),
                    Specialization =  reader["Specialization"].ToString() ?? string.Empty ,
                    Notes = reader["Notes"].ToString() ?? string.Empty ,
                    Available = Convert.ToBoolean(reader["Available"])
                });
            }

                return List;
        }

        public static List<DoctorFullDTO> GetAllDoctorsView()
        {
            List<DoctorFullDTO> List = new List<DoctorFullDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("doc.GetAll_DoctorsView", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                List.Add(new DoctorFullDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    PersonID = Convert.ToInt32(reader["PersonID"]),
                    FirstName = reader["FirstName"].ToString() ?? string.Empty,
                    SecondName = reader["SecondName"].ToString() ?? string.Empty,
                    LastName = reader["LastName"].ToString() ?? string.Empty,
                    Phone = reader["Phone"].ToString() ?? string.Empty,
                    Email = reader["Email"].ToString() ?? string.Empty,
                    Specialization = reader["Specialization"].ToString() ?? string.Empty,
                    Notes = reader["Notes"].ToString() ?? string.Empty,
                    Available = Convert.ToBoolean(reader["Available"])
                });
            }

            return List;
        }
    }
}