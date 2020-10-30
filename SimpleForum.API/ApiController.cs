﻿using Microsoft.AspNetCore.Mvc;
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
    }
}