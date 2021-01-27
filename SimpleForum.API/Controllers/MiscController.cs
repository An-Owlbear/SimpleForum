using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common.Server;

namespace SimpleForum.API.Controllers
{
    public class MiscController : ApiController
    {
        // Returns a JSON result for the error of the given code
        [Route("/Error/{code}")]
        public Error StatusError(int code)
        {
            return new Error(code);
        }

        // Returns a JSON error
        [Route("/Error")]
        public Error Error()
        {
            IExceptionHandlerFeature errorFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception exception = errorFeature.Error;
            return new Error(500, exception.Message);
        }

        [HttpGet("/ErrorTest")]
        public int ErrorTest()
        {
            return int.Parse("testing");
        }
    }
}