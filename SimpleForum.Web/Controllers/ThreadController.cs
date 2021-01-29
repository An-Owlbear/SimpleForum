using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Common;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class ThreadController : WebController
    {
        private readonly SimpleForumRepository _repository;
        private int PostsPerPage = 30;

        public ThreadController(SimpleForumRepository repository)
        {
            _repository = repository;
        }
        
        // Returns a page of the requested thread
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            // Retrieves the requested thread
            Thread thread = await _repository.GetThreadAsync(id);
            
            // Returns 404 if thread is null
            if (thread == null) return NotFound();

            // Returns message of thread has been removed
            if (thread.Deleted || thread.User.Deleted)
            {
                MessageViewModel messageModel = new MessageViewModel()
                {
                    Title = "Removed",
                    MessageTitle = "This thread has been removed"
                };
                return View("Message", messageModel);
            }
            
            // Sets the current user
            User user = await _repository.GetUserAsync(User);
            
            // Retrieves comments for the requested page
            Result<IEnumerable<Comment>> result = _repository.GetThreadReplies(thread, page);

            if (result.Failure) return StatusCode(result.Code);

            // Creates model and returns view
            ThreadViewModel model = new ThreadViewModel()
            {
                Thread = thread,
                Comments = result.Value,
                Page = page,
                PageCount = ((thread.Comments.Count + 1) + (PostsPerPage - 1)) / PostsPerPage,
                User = user
            };
            return View("Thread", model);
        }

        // Returns view for creating threads
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public IActionResult Create()
        {
            return View();
        }
        
        // Creates a thread
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> CreateThread(string title, string content)
        {
            // Redirects if the title or content are empty
            if (title == null || content == null) return Redirect("/Thread/Create");

            // Creates and adds thread to database
            DateTime time = DateTime.Now;
            int userID = Tools.GetUserID(User);

            Thread thread = new Thread()
            {
                Title = title,
                Content = content,
                DatePosted = time,
                UserID = userID
            };

            await _repository.AddThreadAsync(thread);
            await _repository.SaveChangesAsync();

            // Redirects to thread
            int threadID = thread.ID;
            return Redirect("/Thread?id=" + threadID);
        }
        
        // Posts a comment to a thread
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostComment(string content, int threadID)
        {
            // Retrieves user
            User user = await _repository.GetUserAsync(User);

            // Create and add comment
            Comment comment = new Comment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                ThreadID = threadID,
                UserID = user.UserID
            };
            Result result = await _repository.PostCommentAsync(comment);

            // Saves changes and redirects if successful, otherwise return error
            if (result.Success)
            {
                await _repository.SaveChangesAsync();
                return Redirect($"/Thread?id={threadID}#{comment.CommentID}");
            }
            return StatusCode(result.Code);
        }

        // Returns page for admin actions on threads
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminActions(int id)
        {
            // Retrieves thread
            Thread thread = await _repository.GetThreadAsync(id);
            
            // Returns 404 if the thread was deleted by user, otherwise returns view
            if (thread.DeletedBy == "User") return NotFound();
            return View(thread);
        }
        
        // Deletes a thread
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Delete(int id)
        {
            // Deletes thread and returns error if needed
            Result result = await _repository.DeleteThreadAsync(id);
            if (result.Failure) return new StatusCodeResult(result.Code);
            
            await _repository.SaveChangesAsync();
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread deleted",
                MessageTitle = "Thread deleted successfully"
            };
            return View("Message", model);
        }

        // Page for admins submitting thread deletion and reason
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDelete(int id)
        {
            // Retrieves thread and returns view
            Thread thread = await _repository.GetThreadAsync(id);
            return View(thread);
        }
        
        // Deletes a thread and provides a reason, sending a notification to the user
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendAdminDelete(int id, string reason)
        {
            // Deletes thread and saves changes
            await _repository.AdminDeleteThreadAsync(id, reason);
            await _repository.SaveChangesAsync();
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread deleted",
                MessageTitle = "Thread deleted successfully"
            };
            return View("Message", model);
        }

        // Pins a thread to the top of the homepage
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Pin(int id)
        {
            // Retrieves and pins thread
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Pinned = true;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread pinned",
                MessageTitle = "Thread pinned"
            };
            return View("Message", model);
        }

        // Locks a thread
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Lock(int id)
        {
            // Retrieves and locks thread
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Locked = true;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread locked",
                MessageTitle = "Thread locked"
            };
            return View("Message", model);
        }
        
        // Restores a thread of the given id
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Restore(int id)
        {
            // Retrieves and restores thread
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Deleted = false;
            thread.DeletedBy = null;
            await _repository.SaveChangesAsync();
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread restored",
                MessageTitle = "Thread restored"
            };
            return View("Message", model);
        }

        // Unpins a thread
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Unpin(int id)
        {
            // Retrieves and unpins thread
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Pinned = false;
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread unpinned",
                MessageTitle = "Thread unpinned"
            };
            return View("Message", model);
        }

        // Unlocks a thread
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Unlock(int id)
        {
            // Retrieves and unlocks thread
            Thread thread = await _repository.GetThreadAsync(id);
            thread.Locked = false;
            await _repository.SaveChangesAsync();
            
            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread unlocked",
                MessageTitle = "Thread unlocked"
            };
            return View("Message", model);
        }

        // Deletes a comment from a thread
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> DeleteComment(int id)
        {
            // Deletes the comment and saves changes if successful
            Result result = await _repository.DeleteCommentAsync(id);
            if (result.Failure) return StatusCode(result.Code);
            
            await _repository.SaveChangesAsync();

            // Creates model and returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }
        
        // Page for admins deleting comments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteComment(int id)
        {
            // Retrieves comment and returns view
            Comment comment = await _repository.GetCommentAsync(id);
            return View(comment);
        }
        
        // Deletes a comment as admin with a reason
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> SendAdminDeleteComment(int id, string reason)
        {
            // Deletes comment and saves changes
            await _repository.AdminDeleteCommentAsync(id, reason);
            await _repository.SaveChangesAsync();

            // Returns view
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }
    }
}