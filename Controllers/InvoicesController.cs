using BackendClinicProject.GlobalClasses;
using ClinicBusinessLogic;
using ClinicDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        [HttpGet("AllInvoices", Name = "GetAllInvoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<InvoiceDTO>> GetAllInvoices()
        {
            try
            {
                var List = clsInvoice.GetAllInvoices();

                if (List == null || List.Count == 0)
                    return NotFound("No Invoice Found.");
                else
                    return Ok(List);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving all invoices.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}", Name = "GetInvoiceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<DoctorDTO> GetDoctorByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");

            try
            {
                var invoice = clsInvoice.Find(id);
                if (invoice == null)
                    return NotFound($"Invoice with ID {id} not found.");
                else
                    return Ok(invoice.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving invoice with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");

            }
        }

        [HttpDelete("{id}", Name = "DeleteInvoiceById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteInvoiceByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");
            try
            {
                bool isDeleted = clsInvoice.Delete(id);
                if (!isDeleted)
                    return NotFound($"Invoice with ID {id} not found or could not be deleted.");
                else
                    return Ok($"Invoice with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting invoice with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("AddNew", Name = "AddInvoice")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddInvoice([FromBody] InvoiceDTO invoiceDTO)
        {
            if (invoiceDTO == null)
                return BadRequest("Invoice data is required.");
            try
            {
                clsInvoice invoice = new clsInvoice
                {
                    Date = invoiceDTO.Date,
                    Amount = invoiceDTO.Amount,
                    Method = invoiceDTO.Method,
                    Status = invoiceDTO.Status
                };
                bool isAdded = invoice.Save();
                if (!isAdded)
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the invoice.");
                else
                    return CreatedAtAction("GetDoctorByID", new { id = invoice.ID }, invoice.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new invoice.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }


        }

        [HttpPut("{id}", Name = "UpdateInvoice")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateInvoice(int id, [FromBody] InvoiceDTO invoiceDTO)
        {
            if (id <= 0 || invoiceDTO == null || id <=0)
                return BadRequest("Invalid ID or invoice data.");
            try
            {
                var existingInvoice = clsInvoice.Find(id);
                if (existingInvoice == null)
                    return NotFound($"Invoice with ID {id} not found.");
                existingInvoice.Date = invoiceDTO.Date;
                existingInvoice.Amount = invoiceDTO.Amount;
                existingInvoice.Method = invoiceDTO.Method;
                existingInvoice.Status = invoiceDTO.Status;
                bool isUpdated = existingInvoice.Save();
                if (!isUpdated)
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the invoice.");
                else
                    return Ok(existingInvoice.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating invoice with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }

        }
    }
}
