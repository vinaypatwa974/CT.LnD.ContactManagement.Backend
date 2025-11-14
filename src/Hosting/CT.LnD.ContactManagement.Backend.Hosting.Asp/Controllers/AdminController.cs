using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Controllers
{
    /// <summary>
    /// Controller for admin operations such as fetching users and changing user roles.
    /// </summary>
    [ApiController]
    [Route("/admin")]
    [Authorize("AdminOnly")]
    public class AdminController(ILogger<AdminController> logger, IUserService userService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <returns>
        /// A list of users with their details.
        /// </returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="400">If an error occurs while fetching users</response>
        [HttpGet]
        [Route("get-users")]
        [ProducesResponseType(typeof(GetUserResponseExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetUserResponseExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]
        public async Task<ActionResult> GetAllUsers()
        {
            List<GetUserResponse> res;
            try
            {
                res = await userService.GetAllUsers();
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal Server Error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to fetch users.");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Users fetched successfully", data = res });
        }

        /// <summary>
        /// Changes the status or role of a user.
        /// </summary>
        /// <param name="userId">The ID of the user to update. This is a required query parameter.</param>
        /// <param name="roleId">The new role ID to assign to the user.</param>
        /// <returns>
        /// A success message if the status was changed.
        /// </returns>
        /// <response code="200">If the user status was changed successfully</response>
        /// <response code="400">If an error occurs while updating the user</response>
        [HttpPatch]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(SuccessExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]
        public async Task<ActionResult> ChangeUserStatus([FromQuery, BindRequired] string userId, string statusId)
        {
            try
            {
                await userService.ChangeUserStatus(userId, statusId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal Server Error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to change user status");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "User status changed successfully" });
        }
    }
}
