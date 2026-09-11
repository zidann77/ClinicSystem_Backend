using BackendClinicProject.GlobalClasses;
using ClinicBusinessLogic;
using ClinicDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        [Authorize(Roles = "Admin, User, Receptionist, Doctor")]
        [HttpGet("AllDoctors",Name ="GetAllDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<DoctorDTO>> GetAllDoctors()
        {
            try
            {
                var doctors = clsDoctor.GetAllDoctors();
                if (doctors == null || doctors.Count == 0)
                {
                    return NotFound("No doctors found.");
                }
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving all doctors.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin , User , Receptionist, Doctor")]
        [HttpGet("AllDoctorsView", Name = "GetAllDoctorsView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<DoctorFullDTO>> GetAllDoctorsView()
        {
            try
            {
                var doctorsView = clsDoctor.GetAllDoctorsView();
                if (doctorsView == null || doctorsView.Count == 0)
                {
                    return NotFound("No detailed doctor data found.");
                }
                return Ok(doctorsView);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving detailed doctor data.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize (Roles ="Admin,User,Receptionist,Doctor")]
        [HttpGet("{id}", Name = "GetDoctorById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<DoctorDTO> GetDoctorById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than zero.");
            }
            try
            {
                var doctor = clsDoctor.Find(id);
                if (doctor == null)
                {
                    return NotFound($"No doctor found with ID {id}.");
                }
                return Ok(doctor.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving doctor with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteDoctor(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than zero.");
            }
            try
            {
                bool isDeleted = clsDoctor.Delete(id);
                if (!isDeleted)
                {
                    return NotFound($"No doctor found with ID {id}.");
                }
                return Ok($"Doctor with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting doctor with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost ("AddNew", Name = "AddDoctor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<DoctorDTO> AddDoctor(DoctorDTO newDoctorDTO)
        {
            if (newDoctorDTO == null || newDoctorDTO.PersonID <= 0)
            {
                return BadRequest("Invalid doctor data.");
            }
            try
            {
                clsDoctor doctor = new clsDoctor
                {
                    Mode = clsDoctor.enMode.AddNew,
                    PersonINFO = new clsPerson { PersonID = newDoctorDTO.PersonID },
                    Specialization = newDoctorDTO.Specialization,
                    Notes = newDoctorDTO.Notes,
                    Available = newDoctorDTO.Available
                };
                bool isSaved = doctor.Save();
                if (!isSaved)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the doctor.");
                }
                return CreatedAtAction("GetDoctorById", new { id = doctor.ID }, doctor.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new doctor.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin,User,Doctor")]
        [HttpPut("{id}", Name = "UpdateDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateDoctor(int id, DoctorDTO updatedDoctorDTO)
        {
            if (id <= 0 || updatedDoctorDTO == null || id != updatedDoctorDTO.ID)
            {
                return BadRequest("Invalid ID or doctor data.");
            }
            try
            {
                clsDoctor? existingDoctor = clsDoctor.Find(id);
                if (existingDoctor == null)
                {
                    return NotFound($"No doctor found with ID {id}.");
                }
                existingDoctor.PersonINFO = new clsPerson { PersonID = updatedDoctorDTO.PersonID };
                existingDoctor.Specialization = updatedDoctorDTO.Specialization;
                existingDoctor.Notes = updatedDoctorDTO.Notes;
                existingDoctor.Available = updatedDoctorDTO.Available;
                bool isSaved = existingDoctor.Save();
                if (!isSaved)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the doctor.");
                }
                return Ok(existingDoctor.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating doctor with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
