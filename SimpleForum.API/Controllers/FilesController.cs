using System.IO;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Common.Server;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Files")]
    public class FilesController : ApiController
    {
        // Returns a user's profile picture
        [HttpGet("ProfileImg")]
        public IActionResult ProfileImg(int? id)
        {
            // Returns if no id
            if (id == null) return new BadRequestResult();
            
            // Sets response headers
            Response.Headers["Cache-Control"] = "max-age=604800, must-revalidate";
            
            // Checks if profile picture exists and returns default if it doesn't
            string path = $"../UploadedImages/ProfilePictures/{id}.jpg";
            if (System.IO.File.Exists(path))
            {
                Stream image = System.IO.File.OpenRead(path);
                return new FileStreamResult(image, "image/jpeg");
            }
            else
            {
                Stream image = System.IO.File.OpenRead("../UploadedImages/ProfilePictures/defaultprofile.png");
                return new FileStreamResult(image, "image/png");
            }
        }
    }
}