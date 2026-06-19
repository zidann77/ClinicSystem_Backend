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
    public class clsInvoiceDataAccess
    {
        // INSERT
        public static int AddInvoice(InvoiceDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("inv.Insert_Invoice", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Date", dto.Date);
            cmd.Parameters.AddWithValue("@Amount", dto.Amount);
            cmd.Parameters.AddWithValue("@Method", dto.Method);
            cmd.Parameters.AddWithValue("@Status", dto.Status);

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
        public static InvoiceDTO? GetInvoiceById(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("inv.Get_InvoiceByID", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new InvoiceDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Method = Convert.ToByte(reader["Method"]),
                    Status = Convert.ToByte(reader["Status"])
                };
            }

            return null;
        }

        // GET ALL
        public static List<InvoiceDTO> GetAllInvoices()
        {
            List<InvoiceDTO> list = new List<InvoiceDTO>();

            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("inv.GetAll_Invoices", con);

            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new InvoiceDTO
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Method = Convert.ToByte(reader["Method"]),
                    Status = Convert.ToByte(reader["Status"])
                });
            }

            return list;
        }

        // UPDATE
        public static bool UpdateInvoice(InvoiceDTO dto)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("inv.Update_Invoice", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", dto.ID);
            cmd.Parameters.AddWithValue("@Date", dto.Date);
            cmd.Parameters.AddWithValue("@Amount", dto.Amount);
            cmd.Parameters.AddWithValue("@Method", dto.Method);
            cmd.Parameters.AddWithValue("@Status", dto.Status);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }

        // DELETE
        public static bool DeleteInvoice(int id)
        {
            using SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString);
            using SqlCommand cmd = new SqlCommand("inv.Delete_Invoice", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", id);

            con.Open();

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}