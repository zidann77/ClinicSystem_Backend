using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDataAccess
{
    using Microsoft.Data.SqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ClinicDTO;

    namespace ClinicDataAccess
    {
        public class clsAppointmentDataAccess
        {
            public static int AddAppointment(AppointmentDTO dto)
            {
                using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new SqlCommand("app.Insert_Appointment", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PatientID", dto.PatientID);
                cmd.Parameters.AddWithValue("@Datetime", dto.Datetime);
                cmd.Parameters.AddWithValue("@status", dto.Status);
                cmd.Parameters.AddWithValue("@Notes",
                    (object?)dto.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DoctorID", dto.DoctorID);

                cmd.Parameters.AddWithValue("@MedicalRecordID",
                    (object?)dto.MedicalRecordID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceID",
                    (object?)dto.InvoiceID ?? DBNull.Value);


                SqlParameter outputId = new SqlParameter("@NewID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(outputId);


                con.Open();

                cmd.ExecuteNonQuery();


                return outputId.Value != DBNull.Value ? (int)outputId.Value : -1;
            }



            public static AppointmentDTO? GetAppointmentById(int id)
            {
                using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new SqlCommand("app.Get_AppointmentByID", con);


                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", id);


                con.Open();


                using SqlDataReader reader = cmd.ExecuteReader();


                if (reader.Read())
                {
                    return new AppointmentDTO
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        Datetime = Convert.ToDateTime(reader["Datetime"]),
                        Status = Convert.ToByte(reader["status"]),
                        Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString(),
                        DoctorID = Convert.ToInt32(reader["DoctorID"]),
                        MedicalRecordID = reader["MedicalRecordID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["MedicalRecordID"]),
                        InvoiceID = reader["InvoiceID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["InvoiceID"])
                    };
                }


                return null;
            }




            public static List<AppointmentDTO> GetAllAppointments()
            {
                List<AppointmentDTO> list = new List<AppointmentDTO>();


                using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new SqlCommand("app.GetAll_Appointment", con);


                cmd.CommandType = CommandType.StoredProcedure;


                con.Open();


                using SqlDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    list.Add(new AppointmentDTO
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        Datetime = Convert.ToDateTime(reader["Datetime"]),
                        Status = Convert.ToByte(reader["status"]),
                        Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString(),
                        DoctorID = Convert.ToInt32(reader["DoctorID"]),
                        MedicalRecordID = reader["MedicalRecordID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["MedicalRecordID"]),
                        InvoiceID = reader["InvoiceID"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["InvoiceID"])
                    });
                }


                return list;
            }




            public static bool UpdateAppointment(AppointmentDTO dto)
            {
                using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new SqlCommand("app.Update_Appointment", con);


                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@ID", dto.ID);
                cmd.Parameters.AddWithValue("@PatientID", dto.PatientID);
                cmd.Parameters.AddWithValue("@Datetime", dto.Datetime);
                cmd.Parameters.AddWithValue("@status", dto.Status);
                cmd.Parameters.AddWithValue("@Notes",
                    (object?)dto.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DoctorID", dto.DoctorID);
                cmd.Parameters.AddWithValue("@MedicalRecordID",
                    (object?)dto.MedicalRecordID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InvoiceID",
                    (object?)dto.InvoiceID ?? DBNull.Value);


                con.Open();


                return cmd.ExecuteNonQuery() > 0;
            }




            public static bool DeleteAppointment(int id)
            {
                using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new SqlCommand("app.Delete_Appointment", con);


                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);


                con.Open();


                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
