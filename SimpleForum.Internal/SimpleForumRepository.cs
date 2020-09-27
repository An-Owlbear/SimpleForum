using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleForum.Models;

namespace SimpleForum.Internal
{
    public class SimpleForumRepository
    {
        private readonly ApplicationDbContext _context;
        private const int ThreadsPerPage = 30;
        private const int PostsPerPage = 30;
        private const int CommentsPerPage = 15;

        public SimpleForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Saves changes made
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }


        //
        // Methods for accessing a single item in the database
        //
        
        // Returns a user for the given id
        public async Task<User> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        } 
        
        // Returns a user for the given ClaimsPrincipal
        public async Task<User> Get(ClaimsPrincipal principal)
        {
            Claim claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return await _context.Users.FindAsync(int.Parse(claim.Value));
        }
        
        // Returns a thread of the given id
        public async Task<Thread> GetThread(int id)
        {
            return await _context.Threads.FindAsync(id);
        }
        
        // Returns a comment of the given id
        public async Task<Comment> GetComment(int id)
        {
            return await _context.Comments.FindAsync(id);
        }
        
        // Returns a UserComment of the give id
        public async Task<UserComment> GetUserComment(int id)
        {
            return await _context.UserComments.FindAsync(id);
        }
        
        // Returns an EmailCode of the give id
        public async Task<EmailCode> GetEmailCode(int id)
        {
            return await _context.EmailCodes.FindAsync(id);
        }
        
        
        
        //
        // Methods for adding a single item to the database
        //
        
        // Adds a user to the database
        public async Task<User> AddUser(User user)
        {
            EntityEntry<User> addedUser = await _context.Users.AddAsync(user);
            return addedUser.Entity;
        }
        
        // Adds a thread to the database
        public async Task<Thread> AddThread(Thread thread)
        {
            EntityEntry<Thread> addedThread = await _context.Threads.AddAsync(thread);
            return addedThread.Entity;
        }
        
        // Adds a comment to the database
        public async Task<Comment> AddComment(Comment comment)
        {
            EntityEntry<Comment> addedComment = await _context.Comments.AddAsync(comment);
            return addedComment.Entity;
        }
        
        // Adds a UserComment to the database
        public async Task<UserComment> AddUserComment(UserComment userComment)
        {
            EntityEntry<UserComment> addedUserComment = await _context.UserComments.AddAsync(userComment);
            return addedUserComment.Entity;
        }
        
        // Adds an EmailCode to the database
        public async Task<EmailCode> AddEmailCode(EmailCode emailCode)
        {
            EntityEntry<EmailCode> addedEmailCode = await _context.EmailCodes.AddAsync(emailCode);
            return addedEmailCode.Entity;
        }
        
        
        
        // Returns a list of threads for the frontpage for the given page
        public async Task<IEnumerable<Thread>> GetFrontPage(int page)
        {
            return await _context.Threads
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.DatePosted)
                .Skip((page - 1) * ThreadsPerPage)
                .Take(ThreadsPerPage).ToListAsync();
        }
        
        // Returns a list of replies to a thread
        public async Task<IEnumerable<Comment>> GetThreadReplies(Thread thread, int page)
        {
            return await thread.Comments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderBy(x => x.DatePosted)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        // Returns a list of replies to a thread of a given id
        public async Task<IEnumerable<Comment>> GetThreadReplies(int threadID, int page)
        {
            Thread thread = await GetThread(threadID);
            return await GetThreadReplies(thread, page);
        }
        
        // Returns a list of comments on a user's page
        public async Task<IEnumerable<UserComment>> GetUserComments(User user, int page)
        {
            return await user.UserPageComments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage)
                .Take(CommentsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        // Returns a list of comments of a user's page of a given user id
        public async Task<IEnumerable<UserComment>> GetUserComments(int userID, int page)
        {
            User user = await GetUser(userID);
            return await GetUserComments(user, page);
        }
    }
}