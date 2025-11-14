using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Dtos;
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
    [Route("/upload")]
    [Authorize("ActiveUserOnly")]
    public class FileUploadController(ILogger<FileUploadController> logger, IFileUploadService fileUploadService) : ControllerBase
    {
        /// <summary>
        /// Upload an image for a contact.
        /// </summary>
        [HttpPost]
        [Route("image")]
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<ActionResult> UploadImage([FromForm] FileUploadRequest fileUploadRequest, [FromQuery, BindRequired] string contactId)
        {
            try
            {
                await fileUploadService.UploadImage(fileUploadRequest.File, contactId);

                return Ok(new
                {
                    statusCode = 200,
                    status = "OK",
                    message = "Contact Avatar uploaded successfully"
                });
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
                logger.LogError("Failed to upload image");
                logger.LogError($"{e.Message}");

                return BadRequest(new
                {
                    statusCode = 400,
                    status = "Bad Request",
                    message = e.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Upload and process contacts from a CSV file.
        /// </summary>
        [HttpPost]
        [Route("csv")]
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
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<ActionResult> ProcessCSV([FromForm] FileUploadRequest fileUploadRequest, [FromQuery, BindRequired] string userId)
        {
            try
            {
                await fileUploadService.ProcessCSV(fileUploadRequest.File, userId);

                return Ok(new
                {
                    statusCode = 200,
                    status = "OK",
                    message = "CSV Processed Successfully"
                });
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
                logger.LogError("Failed to process csv");
                logger.LogError($"{e.Message}");

                return BadRequest(new
                {
                    statusCode = 400,
                    status = "Bad Request",
                    message = e.Message,
                    timeStamp = DateTime.UtcNow
                });
            }
        }
    }
}
