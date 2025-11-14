using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.User;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UserController(IUserService userService, ILogger<UserController> logger, IConfiguration configuration, IJWTService jwtService, IEmailService emailService) : ControllerBase
    {
        public readonly IUserService UserService = userService;

        /// <summary>
        /// Get user details by ID.
        /// </summary>
        /// <param name="id">The user's unique ID.</param>
        /// <returns>User details if found.</returns>
        /// <response code="200">User found and returned successfully.</response>
        /// <response code="400">Validation failed or fetch error`.</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(NotFoundExample), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetUserResponseExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(404, typeof(NotFoundExample))]
        [Produces("application/json")]
        public async Task<ActionResult<GetUserResponse>> GetByIdAsync(string id)
        {
            GetUserResponse userDetails;
            try
            {
                userDetails = await UserService.GetByIdAsync(id);
                if (userDetails == null)
                {
                    logger.LogInformation("No user found with the given ID");
                    return NotFound(new { statusCode = 404, status = "Not Found", timeStamp = DateTime.UtcNow, Message = "No user found !" });
                }
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
            catch (Exception ex)
            {
                logger.LogError("Error while fetching the user");
                logger.LogError($"{ex.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = ex.Message, timeStamp = DateTime.UtcNow });
            }

            logger.LogInformation("User Fetched successfully .");
            return Ok(new { statusCode = 200, Message = "User fetched successfully", data = userDetails });
        }

        /// <summary>
        /// Register a new user and send email verification.
        /// </summary>
        /// <param name="user">User registration request.</param>
        /// <returns>JWT token and status message.</returns>
        /// <response code="200">User created and email sent.</response>
        /// <response code="400">Validation failed or error during creation.</response>
        [HttpPost("/signup")]
        [ProducesResponseType(typeof(UserRegisterRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(UserRegisterRequestExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerRequestExample(typeof(UserRegisterRequest), typeof(UserRegisterRequestExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> CreateUser([FromBody] UserRegisterRequest user)
        {
            SecurityToken token;
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    status = "Bad Request",
                    message = "Input Validation failed.",
                    ModelState
                });
            }

            try
            {
                await UserService.CreateUserAsync(user);
                token = jwtService.GenerateJSONWebToken(configuration, user.Email);
                emailService.SendVerificationEmail(user.Email, token);
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
            catch (Exception ex)
            {
                logger.LogError("Failed to create user");
                logger.LogError($"{ex.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = ex.Message, timeStamp = DateTime.UtcNow });
            }

            logger.LogInformation("User created successfully , please verify email to use the account");
            return Ok(new { statusCode = 200, message = "User created successfully", token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        /// <summary>
        /// Authenticate a user and return JWT token.
        /// </summary>
        /// <param name="userLoginRequest">User login credentials.</param>
        /// <returns>User info and JWT token.</returns>
        /// <response code="200">Login successful.</response>
        /// <response code="404">Invalid credentials.</response>
        /// <response code="400">Validation failed or login error.</response>
        [HttpPost]
        [Route("/login")]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(SuccessExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerRequestExample(typeof(UserLoginRequest), typeof(UserLoginRequestExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> LoginUser([FromBody] UserLoginRequest userLoginRequest)
        {
            GetUserResponse response;
            SecurityToken token;
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        status = "Bad Request",
                        message = "Input Validation failed.",
                        ModelState
                    });
                }

                response = await UserService.VerifyUser(userLoginRequest);

                if (response == null)
                {
                    return BadRequest(new { success = false, message = "Invalid credentials please check" });
                }

                token = jwtService.GenerateJSONWebToken(configuration, response);
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
                logger.LogError("Failed to login user");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "User login successfull", user = response, token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        /// <summary>
        /// Verify user email using token sent via email.
        /// </summary>
        /// <param name="token">JWT token from email.</param>
        /// <returns>HTML content confirming verification.</returns>
        /// <response code="200">Email verified successfully.</response>
        /// <response code="400">Invalid or expired token.</response>
        [HttpGet]
        [Route("/verify-email")]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [Produces("application/json")]
        public async Task<ActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                ClaimsPrincipal principals = jwtService.ValidateToken(configuration, token);

                if (principals == null)
                {
                    logger.LogError("Failed to validate token");
                    return BadRequest(new { statusCode = 400, status = "Bad request", message = "Failed to validate token", timeStamp = DateTime.UtcNow });
                }

                Claim claim = principals.FindFirst(ClaimTypes.Email);

                if (claim == null)
                {
                    logger.LogError("No email found in the token");
                    return BadRequest(new { statusCode = 400, status = "Bad request", message = "Token not found", timeStamp = DateTime.UtcNow });

                }

                await UserService.VerifyEmail(claim.Value);
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
                logger.LogError("Error while Verifying user email");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Content(@"<H3> Email Verification Successfull<H3/>", "text/html");
        }

        /// <summary>
        /// Updates the name of the currently authenticated user.
        /// </summary>
        /// <param name="user">An object containing the new name for the user.</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>Returns a success message if the name was updated successfully.</returns>
        /// <response code="200">User name updated successfully.</response>
        /// <response code="400">Failed to update name of user.</response>
        [HttpPatch("update-name")]
        [Authorize("ActiveUserOnly")]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(SuccessExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerRequestExample(typeof(UserUpdateNameRequest), typeof(UserUpdateNameRequestExample))]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateName([FromBody] UserUpdateNameRequest user, [FromQuery, BindRequired] string userId)
        {
            try
            {
                await UserService.UpdateName(user, userId);
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
                logger.LogError("Failed to update name of user");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }


            return Ok(new { statusCode = 200, message = "User name updated successfully" });
        }

        /// <summary>
        /// Updates the email address of the currently authenticated user.
        /// </summary>
        /// <param name="user">An object containing the new email for the user.</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>Returns a success message if the email was updated successfully.</returns>
        /// <response code="200">User email updated successfully.</response>
        /// <response code="400">Failed to update email of user.</response>
        [HttpPatch("update-email")]
        [Authorize("ActiveUserOnly")]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(SuccessExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerRequestExample(typeof(UserUpdateNameRequest), typeof(UserUpdateNameRequestExample))]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateEmail([FromBody] UserEmailUpdateRequest user, [FromQuery, BindRequired] string userId)
        {
            try
            {
                await UserService.UpdateEmail(user, userId);
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
                logger.LogError("Failed to update email of user");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }


            return Ok(new { statusCode = 200, message = "User email updated successfully" });
        }
    }
}
