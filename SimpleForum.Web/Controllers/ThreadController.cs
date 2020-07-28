using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleForum.Internal;
using SimpleForum.Models;

namespace SimpleForum.Web.Controllers
{
    public class ThreadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int PostsPerPage = 30;

        public ThreadController(ApplicationDbContext context)
        {
            _context = context;
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
                return Redirect("/");
            }

            ViewData["Title"] = thread.Title;
            ViewData["ThreadTitle"] = thread.Title;
            ViewData["ThreadID"] = thread.ThreadID;
            ViewData["Comments"] = thread.Comments.OrderBy(x => x.DatePosted)
                .Skip((page - 1) * PostsPerPage).Take(PostsPerPage);
            ViewData["Page"] = page;
            ViewData["PageCount"] = (thread.Comments.Count + (PostsPerPage - 1)) / PostsPerPage;

            return View("Thread");
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize]
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
        
        [Authorize]
        public async Task<IActionResult> PostComment(string content, int threadID)
        {
            Comment comment = new Comment()
            {
                Content = content,
                DatePosted = DateTime.Now,
                ThreadID = threadID,
                UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            
            return Redirect("/Thread?id=" + threadID.ToString());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminActions(int? id)
        {
            if (id == null) return Redirect("/");

            // TODO - Add code for thread preview

            ViewData["Thread"] = _context.Threads.First(x => x.ThreadID == id);
            ViewData["ThreadID"] = id;
            return View();
        }


        [Authorize(Policy = "OwnerOrAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return Redirect("/");
            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Deleted = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = ViewData["Title"] = "Thread deleted successfully.";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pin(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Pinned = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = ViewData["Title"] = "Thread pinned.";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Lock(int? id)
        {
            if (id == null) return Redirect("/");
            if (User.FindFirstValue(ClaimTypes.Role) != "Admin") return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Locked = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = ViewData["Title"] = "Thread has been locked";
            return View("Message");
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return Redirect("/");
            if (!User.IsInRole("Admin")) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Deleted = false;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = ViewData["Title"] = "Thread restored.";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unpin(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Pinned = false;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = ViewData["Title"] = "Thread unpinned";
            return View("Message");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unlock(int? id)
        {
            if (id == null) return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Locked = false;
            await _context.SaveChangesAsync();

            ViewData["Message"] = ViewData["Title"] = "Thread unlocked";
            return View("Message");
        }
    }
}