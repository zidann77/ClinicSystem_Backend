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
    public class RolesController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("AllRoles", Name = "GetAllRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<RoleDTO>> GetAllRoles()
        {
            try
            {
                var rolesList = clsRole.GetAllRoles();

                if (rolesList == null || rolesList.Count == 0)
                    return NotFound("No Roles Found.");

                var rolesDtoList = rolesList.Select(role => role.DTO).ToList();
                return Ok(rolesDtoList);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving all roles.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}", Name = "GetRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<RoleDTO> GetRoleByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");

            try
            {
                var role = clsRole.Find(id);

                if (role == null)
                    return NotFound($"Role with ID {id} not found.");
                else
                    return Ok(role.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving role with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddNew", Name = "AddRole")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult AddRole([FromBody] RoleDTO roleDTO)
        {
            if (roleDTO == null || string.IsNullOrWhiteSpace(roleDTO.RoleName))
                return BadRequest("Role data is required.");

            try
            {
                clsRole role = new clsRole
                {
                    RoleName = roleDTO.RoleName
                };

                bool isAdded = role.Save();

                if (!isAdded)
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the role.");
                else
                    return CreatedAtAction("GetRoleByID", new { id = role.RoleID }, role.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while adding a new role.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdateRole(int id, [FromBody] RoleDTO roleDTO)
        {
            if (id <= 0 || roleDTO == null || string.IsNullOrWhiteSpace(roleDTO.RoleName))
                return BadRequest("Invalid ID or role data.");

            try
            {
                var existingRole = clsRole.Find(id);

                if (existingRole == null)
                    return NotFound($"Role with ID {id} not found.");

                existingRole.RoleName = roleDTO.RoleName;

                bool isUpdated = existingRole.Save();

                if (!isUpdated)
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the role.");
                else
                    return Ok(existingRole.DTO);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating role with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteRoleByID(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID provided.");

            try
            {
                bool isDeleted = clsRole.DeleteRole(id);

                if (!isDeleted)
                    return NotFound($"Role with ID {id} not found or could not be deleted.");
                else
                    return Ok($"Role with ID {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting role with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}