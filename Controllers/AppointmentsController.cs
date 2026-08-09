using BackendClinicProject.GlobalClasses;
using ClinicBusinessLogic;
using ClinicDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {

        [HttpGet("AllAppointments", Name = "GetAllAppointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<AppointmentDTO>> GetAllAppointments()
        {
            try
            {
                var List = clsAppointment.GetAllAppointments();

                if (List == null || List.Count == 0)
                    return NotFound("No Appointments Found.");

                return Ok(List);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving all appointments.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }




        [HttpGet("{id}", Name = "GetAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AppointmentDTO> GetAppointmentByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");

            try
            {
                var appointment = clsAppointment.Find(id);

                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found.");

                return Ok(appointment.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,
                    $"Error occurred while retrieving appointment with ID {id}.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }




        [HttpDelete("{id}", Name = "DeleteAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteAppointmentByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");

            try
            {
                bool isDeleted = clsAppointment.Delete(id);

                if (!isDeleted)
                    return NotFound($"Appointment with ID {id} not found or could not be deleted.");

                return Ok($"Appointment with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,
                    $"Error occurred while deleting appointment with ID {id}.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }




        [HttpPost("AddNew", Name = "AddAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                return BadRequest("Appointment data is required.");

            try
            {
                clsAppointment appointment = new clsAppointment
                {
                    PatientID = appointmentDTO.PatientID,
                    Datetime = appointmentDTO.Datetime,
                    Status = appointmentDTO.Status,
                    Notes = appointmentDTO.Notes,
                    DoctorID = appointmentDTO.DoctorID,
                    MedicalRecordID = appointmentDTO.MedicalRecordID,
                    InvoiceID = appointmentDTO.InvoiceID
                };


                bool isAdded = appointment.Save();


                if (!isAdded)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "An error occurred while adding the appointment.");


                return CreatedAtAction(
                    "GetAppointmentById",
                    new { id = appointment.ID },
                    appointment.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,
                    "Error occurred while adding a new appointment.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }




        [HttpPut("{id}", Name = "UpdateAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateAppointment(
            int id,
            [FromBody] AppointmentDTO appointmentDTO)
        {
            if (id <= 0 || appointmentDTO == null || id <=0)
                return BadRequest("Invalid ID or appointment data.");


            try
            {
                var existingAppointment = clsAppointment.Find(id);


                if (existingAppointment == null)
                    return NotFound($"Appointment with ID {id} not found.");



                existingAppointment.PatientID = appointmentDTO.PatientID;
                existingAppointment.Datetime = appointmentDTO.Datetime;
                existingAppointment.Status = appointmentDTO.Status;
                existingAppointment.Notes = appointmentDTO.Notes;
                existingAppointment.DoctorID = appointmentDTO.DoctorID;
                existingAppointment.MedicalRecordID = appointmentDTO.MedicalRecordID;
                existingAppointment.InvoiceID = appointmentDTO.InvoiceID;



                bool isUpdated = existingAppointment.Save();



                if (!isUpdated)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "An error occurred while updating the appointment.");


                return Ok(existingAppointment.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex,
                    $"Error occurred while updating appointment with ID {id}.");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred: {ex.Message}");
            }
        }
    }
}