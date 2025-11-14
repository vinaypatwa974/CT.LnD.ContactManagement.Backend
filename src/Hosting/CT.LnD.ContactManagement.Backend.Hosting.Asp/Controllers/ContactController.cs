using Azure.Storage.Blobs.Models;
using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Controllers
{
    [ApiController]
    [Route("/contacts")]
    [Authorize(Policy = "ActiveUserOnly")]
    public class ContactController(IContactService contactService, ILogger<ContactController> logger) : ControllerBase
    {
        private readonly IContactService _contactService = contactService;

        /// <summary>
        /// Create a new contact.
        /// </summary>
        /// <param name="contactRequest">Details of the contact to be created.</param>
        /// <returns>Success or failure message.</returns>
        /// <response code="200">Contact created successfully</response>
        /// <response code="400">Failed to create contact</response
        [HttpPost]
        [ProducesResponseType(typeof(CreateContactRequestExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(CreateContactRequestExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [SwaggerRequestExample(typeof(ContactRequest), typeof(CreateContactRequestExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> CreateContact([FromBody] ContactRequest contactRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Input Validation failed.",
                    ModelState
                });
            }

            try
            {
                await _contactService.AddAsync(contactRequest);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Error while creating the contact");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad Request", message = "Failed to create contact", timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "Contact created successfully", data = contactRequest });
        }

        /// <summary>
        /// Update Basic details of an existing contact.
        /// </summary>
        /// <param name="req">Details for updating the contact.</param>
        /// <returns>Success or failure message.</returns>
        /// /// <response code="200">Contact updated successfully</response>
        /// <response code="400">Failed to update contact</response>
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
        [SwaggerRequestExample(typeof(BasicContactDetailsUpdateRequest), typeof(UpdateBasicContactDetailsRequestExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateBasicContactDetails([FromBody] BasicContactDetailsUpdateRequest req, [FromQuery, BindRequired] string contactId)
        {
            try
            {
                await _contactService.UpdateBasicContactDetails(req, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Error while updating the contact");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "Contact update successfully" });
        }

        /// <summary>
        /// Get all contacts.
        /// </summary>
        /// <param name="getAllContactsRequest">filtering and pagination.</param>
        /// <response code="200">Contacts fetched successfully</response>
        /// <response code="400">Failed to fetch contacts</response>
        /// <returns>List of contacts.</returns>    
        [HttpGet]
        [ProducesResponseType(typeof(GetContactExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetContactExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]
        public async Task<ActionResult<List<Contact>>> GetAllContacts([FromQuery, BindRequired] GetAllContactsRequest getAllContactsRequest)
        {
            List<Contact> contacts;
            try
            {
                contacts = await _contactService.GetAllAsync(getAllContactsRequest);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Error while fetching the contacts");
                logger.LogError($"error: {e.Message}");

                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "Contacts fetched successfully", data = contacts });
        }

        /// <summary>
        /// Delete contact by ID.
        /// </summary>
        /// <param name="id">ID of the contact to delete.</param>
        /// <returns>Success or failure message.</returns>
        /// <response code="200">Contact deleted successfully</response>
        /// <response code="400">Failed to delete contact</response>
        [HttpDelete("{contactId}")]
        [ProducesResponseType(typeof(SuccessExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundExample), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(SuccessExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteByIdAsync(string contactId)
        {
            try
            {
                await _contactService.DeleteByIdAsync(contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Error while deleting the contact.");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Contact deleted successfully" });
        }

        /// <summary>
        /// Get contact by ID.
        /// </summary>
        /// <param name="contactId">ID of the contact to retrieve.</param>
        /// <returns>Contact details.</returns>
        /// <response code="200">Contact fetched successfully</response>
        /// <response code="400">Failed to fetch contact</response>
        [HttpGet("{contactId}")]
        [ProducesResponseType(typeof(GetContactByIdExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetContactByIdExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [SwaggerResponseExample(404, typeof(NotFoundExample))]

        [Produces("application/json")]
        public async Task<ActionResult<ContactResponse>> GetByIdAsync(string contactId)
        {
            ContactResponse contactResponse;
            try
            {
                contactResponse = await _contactService.GetByIdAsync(contactId);
                if (contactResponse == null)
                {
                    return NotFound(new { statusCode = 404, status = "Not Found", message = "No contact found with the id", timeStamp = DateTime.UtcNow });
                }
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to fetch contact detalils");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Contact details fetched successfully", data = contactResponse });
        }

        /// <summary>
        /// Export contacts to CSV.
        /// </summary>
        /// <param name="userId">ID of the user whose contacts are to be exported.</param>
        /// <returns>Status of the export operation.</returns>
        /// <response code="200">Contact exported successfully</response>
        /// <response code="400">Failed to export contacts</response>
        [HttpGet("export")]
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
        public async Task<ActionResult<ContactResponse>> ExportContacts([FromQuery, BindRequired] string userId)
        {
            ContactResponse contactResponse;
            try
            {
                await _contactService.ExportContacts(userId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to export contacts");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "Contacts exported successfully" });

        }

        /// <summary>
        /// Updates an email address for a given contact.
        /// </summary>
        /// <param name="emailAddress">The email address details to update.</param>
        /// <param name="contactId">The ID of the contact whose email is to be updated.</param>
        /// <returns>Returns success if the email was updated; otherwise, returns a bad request with error details.</returns>
        [HttpPatch("update-email")]
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
        [SwaggerRequestExample(typeof(EmailAddressDto), typeof(UpdateContactEmailAddressExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateEmail([FromBody] EmailAddressDto emailAddress, [FromQuery, BindRequired] string contactId)
        {
            ContactResponse contactResponse;
            try
            {
                await _contactService.UpdateEmail(emailAddress, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to update Email");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });
            }

            return Ok(new { statusCode = 200, message = "Email updated successfully" });

        }

        /// <summary>
        /// Updates a physical address for a given contact.
        /// </summary>
        /// <param name="physicalAddress">The address details to update.</param>
        /// <param name="contactId">The ID of the contact whose address is to be updated.</param>
        /// <returns>Returns success if the address was updated; otherwise, returns a bad request with error details.</returns>
        [HttpPatch("update-address")]
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
        [SwaggerRequestExample(typeof(PhysicalAddressDto), typeof(UpdateContactPhysicalAddressExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateAddress([FromBody] PhysicalAddressDto physicalAddress, [FromQuery, BindRequired] string contactId)
        {
            ContactResponse contactResponse;
            try
            {
                await _contactService.UpdateAddress(physicalAddress, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to update address");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Address updated successfully" });

        }

        /// <summary>
        /// Updates a phone number for a given contact.
        /// </summary>
        /// <param name="phoneNumber">The phone number details to update.</param>
        /// <param name="contactId">The ID of the contact whose phone number is to be updated.</param>
        /// <returns>Returns success if the phone number was updated; otherwise, returns a bad request with error details.</returns>
        [HttpPatch("update-phone")]
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
        [SwaggerRequestExample(typeof(PhoneNumberDto), typeof(UpdateContactPhoneNumberExample))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdatePhone([FromBody] PhoneNumberDto phoneNumber, [FromQuery, BindRequired] string contactId)
        {
            ContactResponse contactResponse;
            try
            {
                await _contactService.UpdatePhone(phoneNumber, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to update phone number");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "phone number updated successfully" });

        }

        [HttpGet("tag-search")]
        [ProducesResponseType(typeof(GetContactExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetContactExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]

        public async Task<ActionResult> SearchUsingTag([FromQuery, BindRequired] string userId, [FromQuery, BindRequired] string tag)
        {
            List<Contact> res;
            try
            {
                res = await _contactService.SearchUsingTag(tag, userId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to search contacts");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Tag search successful", Data = res });
        }

        [HttpGet("name-search")]
        [ProducesResponseType(typeof(GetContactExample), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(InternalServerErrorResponse), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(500, typeof(InternalServerErrorResponse))]
        [SwaggerResponseExample(200, typeof(GetContactExample))]
        [SwaggerResponseExample(400, typeof(BadRequestExample))]
        [SwaggerResponseExample(401, typeof(AccessDeniedExample))]
        [SwaggerResponseExample(403, typeof(ForbiddenExample))]
        [Produces("application/json")]
        public async Task<ActionResult> SearchUsingName([FromQuery, BindRequired] string name, [FromQuery, BindRequired] string userId)
        {
            List<Contact> res;
            try
            {
                res = await _contactService.SearchUsingName(name, userId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to search contacts");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "search successful", Data = res });
        }

        [HttpDelete("delete-email")]
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
        public async Task<ActionResult> DeleteEmailAddress([FromQuery, BindRequired] string typeId, [FromQuery, BindRequired] string contactId)
        {
            try
            {
                await _contactService.DeleteEmailAddress(typeId, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delte email address");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "email address deleted successfully" });
        }

        [HttpDelete("delete-phone")]
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
        public async Task<ActionResult> DeletePhoneNumber([FromQuery, BindRequired] string typeId, [FromQuery, BindRequired] string contactId)
        {
            try
            {
                await _contactService.DeletePhoneNumber(typeId, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delete phone number");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "phone number deleted successfully" });
        }

        [HttpDelete("delete-address")]
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
        public async Task<ActionResult> DeletePhysicalAddres([FromQuery, BindRequired] string typeId, [FromQuery, BindRequired] string contactId)
        {
            try
            {
                await _contactService.DeletePhysicalAddress(typeId, contactId);
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to delete physical address");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "physical address deleted successfully" });
        }



        [HttpGet("get-email-types")]
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
        public async Task<ActionResult> FindAllEmailTypes()
        {
            List<EmailType> res;
            try
            {
                res = await _contactService.FindAllEmailTypes();
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to fetch email address types");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "email address types fetched successfully", data = res });
        }


        [HttpGet("get-phone-types")]
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
        public async Task<ActionResult> FindAllPhoneTypes()
        {
            List<EmailType> res;
            try
            {
                res = await _contactService.FindAllEmailTypes();
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to fetch phone number types");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "Phone number types fetched successfully", data = res });
        }

        [HttpGet("get-address-types")]
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
        public async Task<ActionResult> FindAllAddressTypes()
        {
            List<AddressType> res;
            try
            {
                res = await _contactService.FindAllAddressTypes();
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx)
            {
                logger.LogError(sqlEx, "A database error occured");
                return StatusCode(500, new
                {
                    statusCode = 500,
                    status = "Internal server error",
                    message = sqlEx.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
            catch (Exception e)
            {
                logger.LogError("Failed to fetch physical address types");
                logger.LogError($"{e.Message}");
                return BadRequest(new { statusCode = 400, status = "Bad request", message = e.Message, timeStamp = DateTime.UtcNow });

            }

            return Ok(new { statusCode = 200, message = "physical address types fetched successfully", data = res });
        }



    }
}
