using System;
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

            int threadID = _context.Threads.OrderByDescending(x => x.DatePosted).Where(x => x.UserID == userID).First().ThreadID;

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
    }
}