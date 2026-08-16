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
    public class MedicalRecordsController : ControllerBase
    {
        [Authorize]
        [HttpGet("AllMedicalRecords", Name = "GetAllMedicalRecords")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllMedicalRecords()
        {
            try
            {
                var medicalRecords = clsMedicalRecord.GetAllMedicalRecords();
                if (medicalRecords == null || !medicalRecords.Any())
                {
                    return NotFound("No medical records found.");
                }
                return Ok(medicalRecords);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving medical records.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving medical records.");
            }
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetMedicalRecordById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMedicalRecordById(int id)
        {
            try
            {
                var medicalRecord = clsMedicalRecord.Find(id);
                if (medicalRecord == null)
                {
                    return NotFound($"Medical record with ID {id} not found.");
                }
                return Ok(medicalRecord.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving medical record with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the medical record.");
            }
        }

        [Authorize]
        [HttpPost("AddMedicalRecord", Name = "AddMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            try
            {
                if (medicalRecordDTO == null)
                {
                    return BadRequest("Medical record data is null.");
                }
                var medicalRecord = new clsMedicalRecord
                {
                    AppointmentID = medicalRecordDTO.AppointmentID,
                    Diagnosis = medicalRecordDTO.Diagnosis,
                    Prescription = medicalRecordDTO.Prescription,
                    Notes = medicalRecordDTO.Notes
                };
                bool isAdded = medicalRecord.Save();
                if (!isAdded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add the medical record.");
                }
                return CreatedAtRoute("GetMedicalRecordById", new { id = medicalRecord.ID }, medicalRecord.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new medical record.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the medical record.");
            }
        }

        [Authorize]
        [HttpPut("{id}", Name = "UpdateMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMedicalRecord(int id, [FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            try
            {
                if (medicalRecordDTO == null || id <= 0)
                {
                    return BadRequest("Invalid medical record data.");
                }
                var existingMedicalRecord = clsMedicalRecord.Find(id);
                if (existingMedicalRecord == null)
                {
                    return NotFound($"Medical record with ID {id} not found.");
                }
                existingMedicalRecord.AppointmentID = medicalRecordDTO.AppointmentID;
                existingMedicalRecord.Diagnosis = medicalRecordDTO.Diagnosis;
                existingMedicalRecord.Prescription = medicalRecordDTO.Prescription;
                existingMedicalRecord.Notes = medicalRecordDTO.Notes;
                bool isUpdated = existingMedicalRecord.Save();
                if (!isUpdated)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update the medical record.");
                }
                return Ok(existingMedicalRecord.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating medical record with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the medical record.");
            }
        }

        [Authorize]
        [HttpDelete("{id}", Name = "DeleteMedicalRecord")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteMedicalRecord(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid medical record ID.");
                }
                var existingMedicalRecord = clsMedicalRecord.Find(id);
                if (existingMedicalRecord == null)
                {
                    return NotFound($"Medical record with ID {id} not found.");
                }
                bool isDeleted = clsMedicalRecord.Delete(id);
                if (!isDeleted)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete the medical record.");
                }
                return Ok($"Medical record with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting medical record with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the medical record.");
            }
        }


    }
}
