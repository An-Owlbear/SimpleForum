using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Responses;

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
    }
}