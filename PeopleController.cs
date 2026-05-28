using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicDTO;
using ClinicBusinessLogic;

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
            catch
            {
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
                clsPerson Person = clsPerson.Find(id);
                if (Person == null)
                {
                    return NotFound("No people found.");
                }
                else
                {

                    return Ok(Person.ToDTO());
                }
            }
            catch
            {
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
                    /* هذا السطر الاحترافي يقوم بثلاثة أشياء طبقاً لمعايير الـ REST API العالمية:
   1. يرجع كود الحالة (201 Created) ليعلم العميل أن الإضافة تمت بنجاح.
   2. البارامتر الأول ("GetPersonByID"): هو اسم الشهرة الداخلي لميثود الجلب، ليقوم السيرفر باستدعائها.
   3. البارامتر الثاني (new { id = newPerson.ID }): عبارة عن (Anonymous Object) نمرر فيه الـ ID الجديد 
      الذي تولد في قاعدة البيانات، ليقوم السيرفر بدمجه تلقائياً وتوليد رابط الوصول للشخص في الـ Header (Location).
   4. البارامتر الثالث (newPerson.ToDTO()): يحول كائن البزنس لـ DTO نظيف ويرسله في الـ Body ليعرضه العميل فوراً.
*/
                    return CreatedAtRoute("GetPersonByID", new { id = newPerson.ID }, newPerson.ToDTO());
                }
                else
                {
                    return StatusCode(500, "An error occurred while saving the new person to the database.");
                }
            }
            catch
            {
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
                clsPerson Updateperson = clsPerson.Find(id);
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
            catch
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeletePerson(int id)
        // EXPLANATION:
        // The 'if' block handles LOGICAL failures from the DB (e.g., Foreign Key blocking the delete, returning false).
        // The 'catch' block handles RUNTIME crashes (e.g., DB connection lost, server down).
        {
            if (id <= 0)
                return BadRequest($"InVaild ID{id}");
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
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while executing delete.");
            }
        }
    }
}