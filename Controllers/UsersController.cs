using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicDTO;
using ClinicBusinessLogic;
using BackendClinicProject.GlobalClasses;
using System;
using System.Collections.Generic;

namespace BackendClinicProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("AllUsers", Name = "GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                List<UserDTO> users = clsUser.GetAllUsers();
                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
              
                clsLogger.LogException(ex, "Error occurred while retrieving all users.");
                return StatusCode(500, "An error occurred while retrieving data from the server.");
            }
        }

        [HttpGet("AllUsersFull", Name = "GetAllUsersView")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<UserFullDTO>> GetAllUsersFull()
        {
            try
            {
                List<UserFullDTO> usersFull = clsUser.GetAllUsers_FullDAata();
                if (usersFull == null || usersFull.Count == 0)
                {
                    return NotFound("No users view records found.");
                }

                return Ok(usersFull);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while retrieving full view records for users.");
                return StatusCode(500, "An error occurred while retrieving detailed data from the server.");
            }
        }

        [HttpGet("{id}", Name = "GetUserByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> GetUserByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID. ID must be greater than zero.");
            }

            try
            {
                clsUser? user = clsUser.Find(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(new UserDTO
                {
                    ID = user.UserID,
                    PersonID = user.PersonINFO.PersonID,
                    UserName = user.UserName,
                    Active = user.Active,
                    LastSeen = user.LastSeen
                });
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while retrieving user with ID: {id}.");
                return StatusCode(500, "An error occurred while retrieving the user from the server.");
            }
        }

        [HttpPost("AddNew", Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> AddUser(UserDTO newUserDTO)
        {
            if (newUserDTO == null || string.IsNullOrEmpty(newUserDTO.UserName) || string.IsNullOrEmpty(newUserDTO.Password) || newUserDTO.PersonID <= 0)
            {
                return BadRequest("Invalid input. PersonID, Username, and Password are required.");
            }

            try
            {
                if (clsUser.IsUserNameExists(newUserDTO.UserName))
                {
                    return BadRequest("Username is already taken. Please choose another one.");
                }

                clsUser user = new clsUser
                {
                    PersonINFO = clsPerson.Find( newUserDTO.PersonID)?? new clsPerson(),
                    UserName = newUserDTO.UserName,
                    Password = newUserDTO.Password,
                    Active = newUserDTO.Active,
                    LastSeen = newUserDTO.LastSeen
                };

                if (user.Save())
                {
                    UserDTO createdUserDTO = new UserDTO
                    {
                        ID = user.UserID,
                        PersonID = user.PersonINFO.PersonID,
                        UserName = user.UserName,
                        Active = user.Active,
                        LastSeen = user.LastSeen
                    };

                    return CreatedAtRoute("GetUserByID", new { id = user.UserID }, createdUserDTO);
                }

                return StatusCode(500, "An error occurred while saving the new user to the database.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, "Error occurred while creating a new user.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> UpdateUser(int id, UserDTO updatedUserDTO)
        {
            if (id <= 0 || updatedUserDTO == null || string.IsNullOrEmpty(updatedUserDTO.UserName))
            {
                return BadRequest("Invalid input. ID must be greater than zero, and Username is required.");
            }

            try
            {
                clsUser? user = clsUser.Find(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                if (user.UserName != updatedUserDTO.UserName && clsUser.IsUserNameExists(updatedUserDTO.UserName))
                {
                    return BadRequest("Username is already taken by another user.");
                }

                //   user.PersonID = updatedUserDTO.PersonID; mistakenly allowing to change the personID which is a foreign key to the person table and should not be changed after creation
                user.UserName = updatedUserDTO.UserName;
                user.Active = updatedUserDTO.Active;
                user.LastSeen = updatedUserDTO.LastSeen;

                if (user.Save())
                {
                    UserDTO resultDTO = new UserDTO
                    {
                        ID = user.UserID,
                        PersonID = user.PersonINFO.PersonID,
                        UserName = user.UserName,
                        Active = user.Active,
                        LastSeen = user.LastSeen
                    };

                    return Ok(resultDTO);
                }

                return StatusCode(500, "An error occurred while updating the user in the database.");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while updating user with ID: {id}.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest($"Invalid ID: {id}");

            try
            {
                if (clsUser.Find(id) == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                if (clsUser.DeleteUser(id))
                {
                    return NoContent();
                }

                return StatusCode(500, "An error occurred while executing delete (Database restriction).");
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred while deleting user with ID: {id}.");
                return StatusCode(500, "An error occurred while executing delete due to a server error.");
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            try
            {
                clsUser? user = clsUser.Login(loginRequest.UserName, loginRequest.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password.");
                }

                if (!user.Active)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "This user account is deactivated.");
                }

                UserDTO loggedInUser = new UserDTO
                {
                    ID = user.UserID,
                    PersonID = user.PersonINFO.PersonID,
                    UserName = user.UserName,
                    Active = user.Active,
                    LastSeen = user.LastSeen
                };

                return Ok(loggedInUser);
            }
            catch (Exception ex)
            {
                clsLogger.LogException(ex, $"Error occurred during login process for username: {loginRequest.UserName}.");
                return StatusCode(500, "An error occurred during the login process.");
            }
        }
    }

    public class LoginRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}