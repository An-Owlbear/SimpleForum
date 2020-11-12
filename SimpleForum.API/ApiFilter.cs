using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API
{
    public class ApiFilter
    {
        /// <summary>
        /// Returns a JSON result for status code 401
        /// </summary>
        /// <returns></returns>
        protected UnauthorizedObjectResult Unauthorized()
        {
            return new UnauthorizedObjectResult(new Error(401));
        }
        
        /// <summary>
        /// Returns an unauthorised JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        protected UnauthorizedObjectResult Unauthorized(string message)
        {
            return new UnauthorizedObjectResult(new Error(401, message));
        }
        
        /// <summary>
        /// Returns a JSON result for status code 404 
        /// </summary>
        /// <returns></returns>
        protected NotFoundObjectResult NotFound()
        {
            return new NotFoundObjectResult(new Error(404));
        }

        /// <summary>
        /// Returns a not found JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        protected NotFoundObjectResult NotFound(string message)
        {
            return new NotFoundObjectResult(new Error(404, message));
        }

        /// <summary>
        /// Returns a JSON result for status code 400
        /// </summary>
        /// <returns></returns>
        protected BadRequestObjectResult BadRequest()
        {
            return new BadRequestObjectResult(new Error(400));
        }

        /// <summary>
        /// Returns a bad request JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        protected BadRequestObjectResult BadRequest(string message)
        {
            return new BadRequestObjectResult(new Error(400, message));
        }

        /// <summary>
        /// Returns a access denied JSON result
        /// </summary>
        /// <returns></returns>
        protected ObjectResult Forbid()
        {
            return new ObjectResult(new Error(403)) {StatusCode = 403};
        }
        
        /// <summary>
        /// Returns a access denied JSON result for a message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        protected ObjectResult Forbid(string message)
        {
            return new ObjectResult(new Error(403, message)) {StatusCode = 403};
        }
    }
}