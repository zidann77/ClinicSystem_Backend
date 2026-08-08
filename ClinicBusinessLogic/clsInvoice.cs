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

        public enum enPaymentMethod { NotSet = 0, Cash = 1, CreditCard = 2, Insurance = 3 }

        public enum enInvoiceStatus { Pending = 1, Paid = 2, Cancelled = 3, Overdue = 4 }

        public enPaymentMethod PaymentMethod
        {
            get
            {
                return (enPaymentMethod)Method;
            }
            set
            {
                Method = (byte)value;
            }
        }

        public enInvoiceStatus InvoiceStatus
        {
            get
            {
                return (enInvoiceStatus)Status;
            }
            set
            {
                Status = (byte)value;
            }
        }

        public int ID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public byte Method { get; set; }
        public byte Status { get; set; }

     
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

       
        public clsInvoice()
        {
            this.ID = -1;
            this.Date = DateTime.Now;
            this.Amount = 0.00m;
            this.Method = (byte)enPaymentMethod.NotSet;
            this.Status = (byte)enInvoiceStatus.Pending;

            Mode = enMode.AddNew;
        }

      
        private clsInvoice(InvoiceDTO DTO)
        {
            this.ID = DTO.ID;
            this.Date = DTO.Date;
            this.Amount = DTO.Amount;
            this.Method = DTO.Method;
            this.Status = DTO.Status;

            Mode = enMode.Update;
        }

        public static clsInvoice? Find(int id)
        {
            InvoiceDTO? DTO = clsInvoiceDataAccess.GetInvoiceById(id);

            if (DTO != null)
            {
                return new clsInvoice(DTO);
            }
            return null;
        }

      
        private bool _Add()
        {
            this.ID = clsInvoiceDataAccess.AddInvoice(this.DTO);
            return (this.ID != -1);
        }

      
        private bool _Update()
        {
            return clsInvoiceDataAccess.UpdateInvoice(this.DTO);
        }

       
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

      
        public static bool Delete(int id)
        {
            return clsInvoiceDataAccess.DeleteInvoice(id);
        }

       
        public static List<InvoiceDTO> GetAllInvoices()
        {
            return clsInvoiceDataAccess.GetAllInvoices();
        }
    }
}