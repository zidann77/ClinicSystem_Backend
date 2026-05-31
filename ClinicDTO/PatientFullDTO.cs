using System;

namespace ClinicDTO
{
    public class PatientFullDTO
    {
        
        public string FirstName { get; set; } = string.Empty;

        public string SecondName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

       
        public byte Status { get; set; }

        public byte? Age { get; set; } 

        public string Notes { get; set; } = string.Empty; 
    }
}