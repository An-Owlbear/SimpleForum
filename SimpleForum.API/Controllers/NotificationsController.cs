using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.API.Models.Requests;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;

namespace SimpleForum.API.Controllers
{
    [ApiController]
    [Route("Notifications")]
    public class NotificationsController : ApiController
    {
        private readonly SimpleForumRepository _repository;

        public NotificationsController(SimpleForumRepository repository)
        {
            _repository = repository;
        }

        // Returns a list of the user's notification
        [HttpGet("")]
        [Authorize]
        public async Task<IEnumerable<ApiNotification>> GetNotifications()
        {
            // Retrieves user and returns notifications
            User user = await _repository.GetUserAsync(User);
            return user.Notifications.Select(x => new ApiNotification(x));
        }

        // Returns an individual notification
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetNotification(int id)
        {
            // Retrieves notification and user id
            int userID = Tools.GetUserID(User);
            Notification notification = await _repository.GetNotificationAsync(id);

            // Returns notification if user owns it, otherwise returns error
            if (notification.UserID == userID) return Json(new ApiNotification(notification));
            return Forbid();
        }

        // Marks a notification as read
        [HttpPatch("{id}")]
        public async Task<IActionResult> MarkRead(int id, UpdateNotificationRequest request)
        {
            // Retrieves notification and user id
            int userID = Tools.GetUserID(User);
            Notification notification = await _repository.GetNotificationAsync(id);
            
            // Returns error if user doesn't own notification
            if (userID != notification.UserID) return Forbid();

            // Updates notification and returns
            if (request.Read)
            {
                notification.Read = true;
                await _repository.SaveChangesAsync();
            }
            
            return Json(new ApiNotification(notification));
        }
    }
}