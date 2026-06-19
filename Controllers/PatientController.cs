using ClinicBusinessLogic;
using ClinicDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendClinicProject.GlobalClasses;
using System.Collections.Generic;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        [HttpGet("AllPatients", Name = "GetAllPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PatientDTO>> GetAllPatients()
        {
            try
            {
                var patients = clsPatient.GetAllPatients();

                if (patients == null || patients.Count == 0)
                {
                    return NotFound("No patients found.");
                }

                return Ok(patients);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving all patients.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("AllPatientsView", Name = "GetAllPatientView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PatientFullDTO>> GetAllPatientsView()
        {
            try
            {
                var patientsView = clsPatient.GetAllPatientsView();

                if (patientsView == null || patientsView.Count == 0)
                {
                    return NotFound("No detailed patient data found.");
                }

                return Ok(patientsView);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving detailed patient data.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("{id}", Name = "GetPatientByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PatientDTO> GetPatientById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid Patient ID.");
                }

                var patient = clsPatient.Find(id);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                // نُرجع الـ DTO الخاص بكائن البزنس مباشرة للفرونت إند
                return Ok(patient.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving patient with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddNew", Name = "AddNewPatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PatientDTO> AddPatient(PatientDTO newPatientDTO)
        {
            try
            {
                if (newPatientDTO == null || newPatientDTO.Status == 0)
                {
                    return BadRequest("Patient data is null.");
                }


                clsPatient patient = new clsPatient
                {
                    PersonINFO = clsPerson.Find(newPatientDTO.PersonID) ?? new clsPerson(),
                    Status = newPatientDTO.Status,
                    Age = newPatientDTO.Age,
                    Notes = newPatientDTO.Notes ?? string.Empty
                };

                if (patient.Save())
                {

                    newPatientDTO.ID = patient.ID;


                    return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, newPatientDTO);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the patient.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }


        }


        [HttpPut("{id}", Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdatePatient(int id, PatientDTO updatedPatientDTO)
        {
            try
            {
                if (updatedPatientDTO == null || id != updatedPatientDTO.ID)
                {
                    return BadRequest("Patient ID mismatch or data is corrupt.");
                }

                var patient = clsPatient.Find(id);

                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }


                patient.Status = updatedPatientDTO.Status;
                patient.Age = updatedPatientDTO.Age;
                patient.Notes = updatedPatientDTO.Notes ?? string.Empty;

                if (patient.Save())
                {
                    return Ok("Patient updated successfully."); // أو NoContent() حسب رغبتك
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the patient.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating patient with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the patient.");
            }
        }

        [HttpDelete("{id}", Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeletePatient(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid Patient ID.");
                }

                var patient = clsPatient.Find(id);
                if (patient == null)
                {
                    return NotFound($"Patient with ID {id} not found.");
                }

                if (clsPatient.Delete(id))
                {
                    return Ok($"Patient with ID {id} has been deleted.");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the patient.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting patient with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");

            }
        }
    }
}