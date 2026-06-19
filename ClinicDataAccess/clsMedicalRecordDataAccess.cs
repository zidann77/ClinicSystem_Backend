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
    public class clsMedicalRecordDataAccess
    {
        // INSERT
        public static int AddMedicalRecord(MedicalRecordDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("med.Insert_MedicalRecord", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@AppointmentID", dto.AppointmentID);
            cmd.Parameters.AddWithValue("@Diagnosis", dto.Diagnosis);
            cmd.Parameters.AddWithValue("@Prescription", (object)dto.Prescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return outputId.Value != DBNull.Value ? (int)outputId.Value : -1;
        }

        // GET BY ID
        public static MedicalRecordDTO? GetMedicalRecordById(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("med.Get_MedicalRecordByID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new MedicalRecordDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    AppointmentID = Convert.ToInt32(reader["AppointmentID"]),
                    Diagnosis = reader["Diagnosis"].ToString() ?? string.Empty,
                    Prescription = reader["Prescription"] != DBNull.Value ? reader["Prescription"].ToString() : null,
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null
                };
            }

            return null;
        }

        // GET ALL
        public static List<MedicalRecordDTO> GetAllMedicalRecords()
        {
            List<MedicalRecordDTO> list = new List<MedicalRecordDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("med.GetAll_MedicalRecords", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new MedicalRecordDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    AppointmentID = Convert.ToInt32(reader["AppointmentID"]),
                    Diagnosis = reader["Diagnosis"].ToString() ?? string.Empty,
                    Prescription = reader["Prescription"] != DBNull.Value ? reader["Prescription"].ToString() : null,
                    Notes = reader["Notes"] != DBNull.Value ? reader["Notes"].ToString() : null
                });
            }

            return list;
        }

        // UPDATE
        public static bool UpdateMedicalRecord(MedicalRecordDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("med.Update_MedicalRecord", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", dto.ID);
            cmd.Parameters.AddWithValue("@AppointmentID", dto.AppointmentID);
            cmd.Parameters.AddWithValue("@Diagnosis", dto.Diagnosis);
            cmd.Parameters.AddWithValue("@Prescription", (object)dto.Prescription ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        // DELETE
        public static bool DeleteMedicalRecord(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("med.Delete_MedicalRecord", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}