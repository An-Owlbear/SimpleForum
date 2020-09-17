using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SimpleForum.Web.Controllers
{
    public class FilesController : Controller
    {
        // Returns a user's profile picture
        public IActionResult ProfileImg(int? id)
        {
            // Returns if no id
            if (id == null) return new BadRequestResult();
            
            // Checks if profile picture exists and returns default if it doesn't
            string path = $"UploadedImages/ProfilePictures/{id}.jpg";
            if (System.IO.File.Exists(path))
            {
                Stream image = System.IO.File.OpenRead(path);
                return new FileStreamResult(image, "image/jpeg");
            }
            else
            {
                Stream image = System.IO.File.OpenRead("UploadedImages/ProfilePictures/defaultprofile.png");
                return new FileStreamResult(image, "image/png");
            }
        }
    }
}