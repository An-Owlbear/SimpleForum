using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleForum.Internal;
using SimpleForum.Models;
using SimpleForum.Web.Models;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web.Controllers
{
    public class ThreadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;
        private int PostsPerPage = 30;

        public ThreadController(ApplicationDbContext context, IOptions<SimpleForumConfig> config)
        {
            _context = context;
            _config = config.Value;
        }
        
        public IActionResult Index(int? id, int page = 1)
        {
            if (id == null) return Redirect("/");

            Thread thread;

            try
            {
                thread = _context.Threads.First(x => x.ThreadID == id);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(404);
            }

            if (thread.Deleted)
            {
                MessageViewModel messageModel = new MessageViewModel()
                {
                    Title = "Removed",
                    MessageTitle = "This thread has been removed"
                };
                return View("Message", messageModel);
            }
            
            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                user = _context.Users.First(x => x.UserID == userID);
            }
            
            ThreadViewModel model = new ThreadViewModel()
            {
                Title = thread.Title,
                ThreadID = thread.ThreadID,
                Pinned = thread.Pinned,
                Locked = thread.Locked,
                Comments = thread.Comments
                    .Where(x => !x.Deleted)
                    .OrderBy(x => x.DatePosted)
                    .Skip((page - 1) * PostsPerPage)
                    .Take(PostsPerPage),
                Page = page,
                PageCount = (thread.Comments.Count + (PostsPerPage - 1)) / PostsPerPage,
                User = user
            };

            return View("Thread", model);
        }

        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> CreateThread(string title, string content)
        {
            if (title == null || content == null) return Redirect("/Thread/Create");

            DateTime time = DateTime.Now;
            int userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Thread thread = new Thread()
            {
                Title = title,
                DatePosted = time,
                UserID = userID
            };

            await _context.Threads.AddAsync(thread);
            await _context.SaveChangesAsync();

            int threadID = _context.Threads.OrderByDescending(x => x.DatePosted).First(x => x.UserID == userID).ThreadID;

            Comment comment = new Comment()
            {
                Content = content,
                DatePosted = time,
                UserID = userID,
                ThreadID = threadID
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Redirect("/Thread?id=" + threadID);
        }
        
        [Authorize(Policy = "ThreadReply")]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(PreventMuted))]
        public async Task<IActionResult> PostComment(string content, int threadID)
        {
            // Retrieve user and threads
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = await _context.Users.FindAsync(userID);
            Thread thread = await _context.Threads.FindAsync(threadID);
            
            // Create and add comment
            Comment comment = new Comment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                ThreadID = threadID,
                UserID = userID
            };
            await _context.Comments.AddAsync(comment);
            // Save changes so that comment ID can be accessed
            await _context.SaveChangesAsync();


            // Create notification if commenting on another user's thread
            if (thread.UserID != userID)
            {
                Notification notification = new Notification()
                {
                    Title = $"{user.Username} left a comment on your post.",
                    Content = $"Click [here]({_config.InstanceURL}/Thread?id={threadID}#{comment.ID}) to view.",
                    DateCreated = DateTime.Now,
                    UserID = thread.UserID
                };
                await _context.Notifications.AddAsync(notification);
            }
            
            // Saves changes and redirects
            await _context.SaveChangesAsync();
            return Redirect($"/Thread?id={threadID}#{comment.CommentID}");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminActions(int? id)
        {
            if (id == null) return Redirect("/");

            // TODO - Add code for thread preview

            Thread model = await _context.Threads.FindAsync(id);
            return View(model);
        }


        [Authorize(Policy = "ThreadOwner")]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return Redirect("/");
            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Deleted = true;
            await _context.SaveChangesAsync();
            
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread deleted",
                MessageTitle = "Thread deleted successfully"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Pin(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Pinned = true;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread pinned",
                MessageTitle = "Thread pinned"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Lock(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Locked = true;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread locked",
                MessageTitle = "Thread locked"
            };
            return View("Message", model);
        }
        
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Deleted = false;
            await _context.SaveChangesAsync();
            
            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread restored",
                MessageTitle = "Thread restored"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Unpin(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Pinned = false;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread unpinned",
                MessageTitle = "Thread unpinned"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> Unlock(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Locked = false;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Thread unlocked",
                MessageTitle = "Thread unlocked"
            };
            return View("Message", model);
        }

        [Authorize(Policy = "CommentOwner")]
        [ServiceFilter(typeof(VerifiedEmail))]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null) return Redirect("/");

            Comment comment = _context.Comments.First(x => x.CommentID == id);
            comment.Deleted = true;
            await _context.SaveChangesAsync();

            MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CheckPassword))]
        public async Task<IActionResult> AdminDeleteComment(int? id)
        {
            if (id == null) return Redirect("/");

            Comment comment = _context.Comments.First(x => x.CommentID == id);
            comment.Deleted = true;
            await _context.SaveChangesAsync();

                MessageViewModel model = new MessageViewModel()
            {
                Title = "Comment deleted",
                MessageTitle = "Comment deleted"
            };
            return View("Message", model);
        }
    }
}