using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API
{
    /// <summary>
    /// Extension of Controller which implements new error result methods
    /// </summary>
    public class ApiController : Controller
    {
        /// <summary>
        /// Returns a JSON result for status code 401
        /// </summary>
        /// <returns></returns>
        public new UnauthorizedObjectResult Unauthorized()
        {
            return new UnauthorizedObjectResult(new Error(401));
        }
        
        /// <summary>
        /// Returns an unauthorised JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public UnauthorizedObjectResult Unauthorized(string message)
        {
            return new UnauthorizedObjectResult(new Error(401, message));
        }
        
        /// <summary>
        /// Returns a JSON result for status code 404 
        /// </summary>
        /// <returns></returns>
        public new NotFoundObjectResult NotFound()
        {
            return new NotFoundObjectResult(new Error(404));
        }

        /// <summary>
        /// Returns a not found JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public NotFoundObjectResult NotFound(string message)
        {
            return new NotFoundObjectResult(new Error(404, message));
        }

        /// <summary>
        /// Returns a JSON result for status code 400
        /// </summary>
        /// <returns></returns>
        public new BadRequestObjectResult BadRequest()
        {
            return new BadRequestObjectResult(new Error(400));
        }

        /// <summary>
        /// Returns a bad request JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public BadRequestObjectResult BadRequest(string message)
        {
            return new BadRequestObjectResult(new Error(400, message));
        }

        public new ObjectResult Forbid()
        {
            return new ObjectResult(new Error(403)) {StatusCode = 403};
        }
        
        /// <summary>
        /// Returns a access denied JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public ObjectResult Forbid(string message)
        {
            return new ObjectResult(new Error(403, message)) {StatusCode = 403};
        }

        /// <summary>
        /// Returns a result of the given status code
        /// </summary>
        /// <param name="statusCode">The status code to use for the result</param>
        /// <param name="value">The accompanying message</param>
        /// <returns></returns>
        public ObjectResult StatusCode(int statusCode, string value)
        {
            return base.StatusCode(statusCode, new Error(statusCode, value));
        }
    }
}