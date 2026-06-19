using System;
using System.Collections.Generic;
using ClinicDataAccess;
using ClinicDTO;

namespace ClinicBusinessLogic
{
    public class clsInvoice
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public byte Method { get; set; }
        public byte Status { get; set; }

        // DTO Property mapping internal state out to a data transmission object
        public InvoiceDTO DTO
        {
            get
            {
                return new InvoiceDTO
                {
                    ID = this.ID,
                    Date = this.Date,
                    Amount = this.Amount,
                    Method = this.Method,
                    Status = this.Status
                };
            }
        }

        // Default Constructor (AddNew Mode)
        public clsInvoice()
        {
            this.ID = -1;
            this.Date = DateTime.Now;
            this.Amount = 0.00m;
            this.Method = 0;
            this.Status = 0;

            Mode = enMode.AddNew;
        }

        // Private Constructor (Update Mode)
        private clsInvoice(InvoiceDTO DTO)
        {
            this.ID = DTO.ID;
            this.Date = DTO.Date;
            this.Amount = DTO.Amount;
            this.Method = DTO.Method;
            this.Status = DTO.Status;

            Mode = enMode.Update;
        }

        // FIND INVOICE BY ID
        public static clsInvoice? Find(int id)
        {
            InvoiceDTO? DTO = clsInvoiceDataAccess.GetInvoiceById(id);

            if (DTO != null)
            {
                return new clsInvoice(DTO);
            }
            return null;
        }

        // INTERNAL ADD
        private bool _Add()
        {
            this.ID = clsInvoiceDataAccess.AddInvoice(this.DTO);
            return (this.ID != -1);
        }

        // INTERNAL UPDATE
        private bool _Update()
        {
            return clsInvoiceDataAccess.UpdateInvoice(this.DTO);
        }

        // SAVE METHOD (Saves current state based on entity mode)
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_Add())
                    {
                        Mode = enMode.Update; // State changed to update for subsequent saves
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _Update();
            }
            return false;
        }

        // DELETE INVOICE
        public static bool Delete(int id)
        {
            return clsInvoiceDataAccess.DeleteInvoice(id);
        }

        // GET ALL INVOICES
        public static List<InvoiceDTO> GetAllInvoices()
        {
            return clsInvoiceDataAccess.GetAllInvoices();
        }
    }
}