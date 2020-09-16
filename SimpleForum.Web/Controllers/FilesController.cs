using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SimpleForum.Web.Controllers
{
    public class FilesController
    {
        // Returns a user's profile picture
        public IActionResult ProfileImg(int? id)
        {
            // Returns if no id
            if (id == null) return new BadRequestResult();
            
            // Checks if profile picture exists and returns default if it doesn't
            string path = $"UploadedImages/ProfilePictures/{id}.jpg";
            if (File.Exists(path))
            {
                Stream image = File.OpenRead(path);
                return new FileStreamResult(image, "image/jpeg");
            }
            else
            {
                Stream image = File.OpenRead("UploadedImages/ProfilePictures/defaultprofile.png");
                return new FileStreamResult(image, "image/png");
            }
        }
    }
}