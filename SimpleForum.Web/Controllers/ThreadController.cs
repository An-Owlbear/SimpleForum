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
            ViewData["Comments"] = thread.Comments.Skip((page - 1) * PostsPerPage).Take(PostsPerPage);
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
            // TODO - complete create thread method
            return Redirect("/");
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