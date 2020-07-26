using System;
using System.Collections;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        
        // GET
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

        public IActionResult Create()
        {
            return View();
        }
        
        public async Task<IActionResult> CreateThread(string title, string content)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Login");
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

        [HttpPost]
        public async Task<IActionResult> PostComment([FromForm] string content, [FromForm] int threadID)
        {
            if (User.Identity.IsAuthenticated)
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

            return Redirect("/Login");
        }

        public async Task<IActionResult> AdminActions(int? id)
        {
            if (id == null) return Redirect("/");
            if (User.FindFirst(ClaimTypes.Role).Value != "Admin") return Redirect("/");

            // TODO - Add code for thread preview

            ViewData["ThreadID"] = id;
            return View();
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return Redirect("/");
            if (User.FindFirstValue(ClaimTypes.Role) != "Admin") return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Deleted = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = "Thread deleted sucessfully.";
            return View("Message");
        }

        public async Task<IActionResult> Pin(int? id)
        {
            if (id == null) return Redirect("/");
            if (User.FindFirstValue(ClaimTypes.Role) != "Admin") return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Pinned = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = "Thread pinned.";
            return View("Message");
        }

        public async Task<IActionResult> Lock(int? id)
        {
            if (id == null) return Redirect("/");
            if (User.FindFirstValue(ClaimTypes.Role) != "Admin") return Redirect("/");

            Thread thread = _context.Threads.First(x => x.ThreadID == id);
            thread.Locked = true;
            await _context.SaveChangesAsync();

            ViewData["MessageTitle"] = "Thread has been locked";
            return View("Message");
        }
    }
}