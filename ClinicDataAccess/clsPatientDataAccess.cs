using ClinicDTO;
using Microsoft.Data.SqlClient;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Data;

namespace ClinicDataAccess
{
    public class clsPatientDataAccess
    {
        private readonly static string EncryptionKey = clsSecuritySettings.GetEncryptionKey();
        // 1. Insert_Patient
        public static int AddPatient(PatientDTO patient)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("pat.Insert_Patient", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PersonID", patient.PersonID);
            cmd.Parameters.AddWithValue("@Status", patient.Status);
            cmd.Parameters.AddWithValue("@Age", patient.Age.HasValue? patient.Age.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", patient.Notes );

            SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outputId);

            con.Open();
            cmd.ExecuteNonQuery();

            return (outputId.Value != DBNull.Value) ? (int)outputId.Value : -1;
        }

        // 2. Update_Patient
        public static bool UpdatePatient(PatientDTO patient)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("pat.Update_Patient", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", patient.ID);
            cmd.Parameters.AddWithValue("@Status", patient.Status);
            cmd.Parameters.AddWithValue("@Age", patient.Age.HasValue ? patient.Age.Value: DBNull.Value);
            cmd.Parameters.AddWithValue("@Notes", patient.Notes);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

       
        public static bool DeletePatient(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("pat.Delete_Patient", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

     
        public static List<PatientDTO> GetAllPatients()
        {
            var list = new List<PatientDTO>();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("pat.GetAll_Patients", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PatientDTO
                        {
                            ID = (int)reader["ID"],
                            PersonID = (int)reader["PersonID"],
                            Status = (byte)reader["Status"],
                            Age = reader["Age"] as byte?,
                            Notes = reader["Notes"].ToString() ?? string.Empty
                        });
                    }
                }
            }
            return list;
        }

        // 5. Get_PatientByID
        public static PatientDTO? GetPatientByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("pat.Get_PatientByID", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PatientDTO
                        {
                            ID = (int)reader["ID"],
                            PersonID = (int)reader["PersonID"],
                            Status = (byte)reader["Status"],
                            Age = reader["Age"] as byte?,
                            Notes = reader["Notes"].ToString() ?? string.Empty
                        };
                    }
                }
            }
            return null;
        }


        public static List<PatientFullDTO> GetAllPatientsView()
        {
            var list = new List<PatientFullDTO>();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("pat.GetAll_PatientsView", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PatientFullDTO
                        {
                            FirstName = reader["FirstName"].ToString() ?? string.Empty,
                            SecondName = reader["SecondName"].ToString() ?? string.Empty,
                            LastName = reader["LastName"].ToString() ?? string.Empty,
                            Phone = clsAesEncryptionService.Decrypt(reader["Phone"]?.ToString() ?? string.Empty, EncryptionKey),
                            Email = (reader["Email"] == DBNull.Value || string.IsNullOrWhiteSpace(reader["Email"].ToString())) ? string.Empty : clsAesEncryptionService.Decrypt(reader["Email"].ToString() ?? string.Empty, EncryptionKey),
                            Status = (byte)reader["Status"],
                            Age = reader["Age"] as byte?, // تحويل آمن للقيم التي تقبل NULL
                            Notes = reader["Notes"].ToString() ?? string.Empty
                        });
                    }
                }
            }
            return list;
        }
    }
}

