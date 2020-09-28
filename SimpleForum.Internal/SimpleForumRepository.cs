using System;
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
        private readonly SimpleForumConfig _config;
        private const int ThreadsPerPage = 30;
        private const int PostsPerPage = 30;
        private const int CommentsPerPage = 15;

        public SimpleForumRepository(ApplicationDbContext context, string filename)
        {
            _context = context;
            _config = Tools.GetConfig(filename);
        }

        // Saves changes made
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        //
        // Methods for accessing a single item in the database
        //
        
        // Returns a user for the given id
        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        } 
        
        // Returns a user for the given ClaimsPrincipal
        public async Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            Claim claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return await _context.Users.FindAsync(int.Parse(claim.Value));
        }
        
        // Returns a thread of the given id
        public async Task<Thread> GetThreadAsync(int id)
        {
            return await _context.Threads.FindAsync(id);
        }
        
        // Returns a comment of the given id
        public async Task<Comment> GetCommentAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }
        
        // Returns a UserComment of the give id
        public async Task<UserComment> GetUserCommentAsync(int id)
        {
            return await _context.UserComments.FindAsync(id);
        }
        
        // Returns an EmailCode of the give id
        public async Task<EmailCode> GetEmailCodeAsync(int id)
        {
            return await _context.EmailCodes.FindAsync(id);
        }
        
        // Returns a notification of the given id
        public async Task<Notification> GetNotificationAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }
        
        
        
        //
        // Methods for adding a single item to the database
        //
        
        // Adds a user to the database
        public async Task<User> AddUserAsync(User user)
        {
            EntityEntry<User> addedUser = await _context.Users.AddAsync(user);
            return addedUser.Entity;
        }
        
        // Adds a thread to the database
        public async Task<Thread> AddThreadAsync(Thread thread)
        {
            EntityEntry<Thread> addedThread = await _context.Threads.AddAsync(thread);
            return addedThread.Entity;
        }
        
        // Adds a comment to the database
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            EntityEntry<Comment> addedComment = await _context.Comments.AddAsync(comment);
            return addedComment.Entity;
        }
        
        // Adds a UserComment to the database
        public async Task<UserComment> AddUserCommentAsync(UserComment userComment)
        {
            EntityEntry<UserComment> addedUserComment = await _context.UserComments.AddAsync(userComment);
            return addedUserComment.Entity;
        }
        
        // Adds an EmailCode to the database
        public async Task<EmailCode> AddEmailCodeAsync(EmailCode emailCode)
        {
            EntityEntry<EmailCode> addedEmailCode = await _context.EmailCodes.AddAsync(emailCode);
            return addedEmailCode.Entity;
        }
        
        // Adds a notification to the database
        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            EntityEntry<Notification> addedNotification = await _context.Notifications.AddAsync(notification);
            return addedNotification.Entity;
        }
        
        
        
        //
        // Methods for more specific tasks
        //
        
        // Returns a list of threads for the frontpage for the given page
        public async Task<IEnumerable<Thread>> GetFrontPageAsync(int page)
        {
            return await _context.Threads
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.DatePosted)
                .Skip((page - 1) * ThreadsPerPage)
                .Take(ThreadsPerPage).ToListAsync();
        }
        
        // Returns a list of replies to a thread
        public async Task<IEnumerable<Comment>> GetThreadRepliesAsync(Thread thread, int page)
        {
            return await thread.Comments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderBy(x => x.DatePosted)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        // Returns a list of replies to a thread of a given id
        public async Task<IEnumerable<Comment>> GetThreadRepliesAsync(int threadID, int page)
        {
            Thread thread = await GetThreadAsync(threadID);
            return await GetThreadRepliesAsync(thread, page);
        }
        
        // Returns a list of comments on a user's page
        public async Task<IEnumerable<UserComment>> GetUserCommentsAsync(User user, int page)
        {
            return await user.UserPageComments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage)
                .Take(CommentsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        // Returns a list of comments of a user's page of a given user id
        public async Task<IEnumerable<UserComment>> GetUserCommentsAsync(int userID, int page)
        {
            User user = await GetUserAsync(userID);
            return await GetUserCommentsAsync(user, page);
        }
        
        // Posts a comment to a specific thread
        public async Task<Comment> PostCommentAsync(Comment comment)
        {
            // Adds comment and updates the database
            await AddCommentAsync(comment);
            await SaveChangesAsync();
            
            // Creates a notification if the comment creator is not the thread creator
            if (comment.UserID != comment.Thread.UserID)
            {
                Notification notification = new Notification()
                {
                    Title = $"{comment.User.Username} left a comment on your post.",
                    Content = $"Click [here]({_config.InstanceURL}/Thread?id={comment.ThreadID}#{comment.ID}) to view.",
                    DateCreated = DateTime.Now,
                    UserID = comment.Thread.UserID
                };
                await AddNotificationAsync(notification);
            }

            await SaveChangesAsync();
            return comment;
        }

        // Deletes a thread as an admin
        public async Task AdminDeleteThreadAsync(Thread thread, string reason)
        {
            // Throws exception if deleted by user
            if (thread.DeletedBy == "User") throw new InvalidOperationException("404 not found");

            // Sets thread as deleted and gives a reason
            thread.Deleted = true;
            thread.DeleteReason = reason;
            thread.DeletedBy = "Admin";
            
            // Creates and adds notification
            Notification notification = new Notification()
            {
                Title = $"Your thread '{thread.Title}' was deleted",
                Content = $"Reason: {reason}",
                DateCreated = DateTime.Now,
                UserID = thread.UserID
            };
            await AddNotificationAsync(notification);
            await SaveChangesAsync();
        }
        
        // Deletes a thread as an admin from a given id
        public async Task AdminDeleteThreadAsync(int id, string reason)
        {
            Thread thread = await GetThreadAsync(id);
            await AdminDeleteThreadAsync(thread, reason);
        }
    }
}