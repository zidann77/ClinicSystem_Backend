using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicDTO
{
    public class InvoiceDTO
    {
        // Primary Key - Cannot be null
        public int ID { get; set; }

        // Date of invoice - Cannot be null
        public DateTime Date { get; set; }

        // Financial amount - Cannot be null (decimal(8,2) maps to decimal)
        public decimal Amount { get; set; }

        // Payment method ID/Enum - Cannot be null (tinyint maps to byte)
        public byte Method { get; set; }

        // Invoice status ID/Enum - Cannot be null (tinyint maps to byte)
        public byte Status { get; set; }
    }
}
