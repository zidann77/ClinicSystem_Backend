using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicDTO;
using ClinicBusinessLogic;
using BackendClinicProject.GlobalClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackendClinicProject
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        [HttpGet("AllPeople", Name = "GetAllPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PeopleDTO>> GetAllPeople()
        {
            try
            {
                List<PeopleDTO> people = clsPerson.GetAllPeople();
                if (people == null || people.Count == 0)
                {
                    return NotFound("No people found.");
                }
                else
                    return Ok(people);
            }
            catch (Exception ex)
            {
            
                clsLogger.LogException(ex, "Error occurred while retrieving all people.");
                return StatusCode(500, "An error occurred while retrieving the data from the server.");
            }
        }

        [HttpGet("{id}", Name = "GetPersonByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PeopleDTO> GetPersonByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than zero.");
            }

            try
            {
                clsPerson? Person = clsPerson.Find(id);
                if (Person == null)
                {
                    return NotFound("No people found.");
                }
                else
                {
                    return Ok(Person.ToDTO());
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving person with ID: {id}.");
                return StatusCode(500, "An error occurred while retrieving the data from the server.");
            }
        }

        [HttpPost("AddNew", Name = "AddPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PeopleDTO> AddPerson(PeopleDTO newPersonDTO)
        {
            if (newPersonDTO == null || string.IsNullOrEmpty(newPersonDTO.FirstName) || string.IsNullOrEmpty(newPersonDTO.LastName))
            {
                return BadRequest("Invalid input. First name and last name are required.");
            }
            try
            {
                clsPerson newPerson = new clsPerson
                {
                    FirstName = newPersonDTO.FirstName,
                    SecondName = newPersonDTO.SecondName,
                    LastName = newPersonDTO.LastName,
                    Phone = newPersonDTO.Phone,
                    Email = newPersonDTO.Email
                };

                if (newPerson.Save())
                {
                    return CreatedAtRoute("GetPersonByID", new { id = newPerson.PersonID }, newPerson.ToDTO());
                }
                else
                {
                    return StatusCode(500, "An error occurred while saving the new person to the database.");
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new person.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PeopleDTO> UpdatePerson(int id, PeopleDTO updatedPersonDTO)
        {
            if (id <= 0 || updatedPersonDTO == null || string.IsNullOrEmpty(updatedPersonDTO.FirstName) || string.IsNullOrEmpty(updatedPersonDTO.LastName))
            {
                return BadRequest("Invalid input. ID must be greater than zero, and first name and last name are required.");
            }
            try
            {
                clsPerson? Updateperson = clsPerson.Find(id);
                if (Updateperson == null)
                {
                    return NotFound("Person not found.");
                }
                Updateperson.FirstName = updatedPersonDTO.FirstName;
                Updateperson.SecondName = updatedPersonDTO.SecondName;
                Updateperson.LastName = updatedPersonDTO.LastName;
                Updateperson.Phone = updatedPersonDTO.Phone;
                Updateperson.Email = updatedPersonDTO.Email;

                if (Updateperson.Save())
                {
                    return Ok(Updateperson.ToDTO());
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the person in the database.");
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating person with ID: {id}.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeletePerson(int id)
        {
            if (id <= 0)
                return BadRequest($"InVaild ID {id}");
            try
            {
                if (clsPerson.Find(id) == null)
                {
                    return NotFound($"Person with ID {id} not found.");
                }

                if (clsPerson.DeletePerson(id))
                {
                    return Ok($"Person with ID {id} has been deleted.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while executing delete.");
                }
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting person with ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while executing delete.");
            }
        }
    }
}