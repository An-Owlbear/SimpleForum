using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SimpleForum.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SimpleForum.Internal
{
    /// <summary>
    /// A repository for the SimpleForum database.
    /// </summary>
    public class SimpleForumRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly SimpleForumConfig _config;
        private const int ThreadsPerPage = 30;
        private const int PostsPerPage = 30;
        private const int CommentsPerPage = 15;

        /// <summary>
        /// Creates an instance of <see cref="SimpleForumRepository"/>
        /// </summary>
        /// <param name="context">The database context for which to initialise the repository with</param>
        /// <param name="filename">The filename of settings file to use</param>
        public SimpleForumRepository(ApplicationDbContext context, string filename)
        {
            _context = context;
            _config = Tools.GetConfig(filename);
        }

        /// <summary>
        /// Saves any changes made to the database
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        //
        // Methods for accessing a single item in the database
        //
        
        /// <summary>
        /// Gets a user of the given ID from the database
        /// </summary>
        /// <param name="id">The id of user to find</param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        } 
        
        /// <summary>
        /// Returns the user of the given ClaimsPrincipal
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal of the user to find</param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            Claim claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return await _context.Users.FindAsync(int.Parse(claim.Value));
        }
        
        /// <summary>
        /// Returns a thread of the given ID.
        /// </summary>
        /// <param name="id">The id of the thread to find</param>
        /// <returns><see cref="Thread"/></returns>
        public async Task<Thread> GetThreadAsync(int id)
        {
            return await _context.Threads.FindAsync(id);
        }
        
        /// <summary>
        /// Returns a comment of the given id
        /// </summary>
        /// <param name="id">The id of the comment to find</param>
        /// <returns><see cref="Comment"/></returns>
        public async Task<Comment> GetCommentAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }
        
        /// <summary>
        /// Returns a UserComment of the give id
        /// </summary>
        /// <param name="id">The id of the UserComment to find</param>
        /// <returns><see cref="UserComment"/></returns>
        public async Task<UserComment> GetUserCommentAsync(int id)
        {
            return await _context.UserComments.FindAsync(id);
        }
        
        /// <summary>
        /// Returns an EmailCode matching the given code
        /// </summary>
        /// <param name="code">The code to find</param>
        /// <returns><see cref="EmailCode"/></returns>
        public async Task<EmailCode> GetEmailCodeAsync(string code)
        {
            return await _context.EmailCodes.FindAsync(code);
        }
        
        /// <summary>
        /// Returns a notification of the given id
        /// </summary>
        /// <param name="id">The id of the notification to find</param>
        /// <returns><see cref="Notification"/></returns>
        public async Task<Notification> GetNotificationAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }
        
        
        
        //
        // Methods for adding a single item to the database
        //
        
        /// <summary>
        /// Adds a user to the database
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>The added <see cref="User"/></returns>
        public async Task<User> AddUserAsync(User user)
        {
            EntityEntry<User> addedUser = await _context.Users.AddAsync(user);
            return addedUser.Entity;
        }
        
        /// <summary>
        /// Adds a thread to the database
        /// </summary>
        /// <param name="thread">The thread to add</param>
        /// <returns>The added <see cref="Thread"/></returns>
        public async Task<Thread> AddThreadAsync(Thread thread)
        {
            EntityEntry<Thread> addedThread = await _context.Threads.AddAsync(thread);
            return addedThread.Entity;
        }
        
        /// <summary>
        /// Adds a comment to the database
        /// </summary>
        /// <param name="comment">The comment to add</param>
        /// <returns>The added <see cref="Comment"/></returns>
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            EntityEntry<Comment> addedComment = await _context.Comments.AddAsync(comment);
            return addedComment.Entity;
        }
        
        /// <summary>
        /// Adds a UserComment to the database
        /// </summary>
        /// <param name="userComment">The UserComment to add</param>
        /// <returns>The added <see cref="UserComment"/></returns>
        public async Task<UserComment> AddUserCommentAsync(UserComment userComment)
        {
            EntityEntry<UserComment> addedUserComment = await _context.UserComments.AddAsync(userComment);
            return addedUserComment.Entity;
        }
        
        /// <summary>
        /// Adds an EmailCode to the database
        /// </summary>
        /// <param name="emailCode">The emailCode to add</param>
        /// <returns>The added <see cref="EmailCode"/></returns>
        public async Task<EmailCode> AddEmailCodeAsync(EmailCode emailCode)
        {
            EntityEntry<EmailCode> addedEmailCode = await _context.EmailCodes.AddAsync(emailCode);
            return addedEmailCode.Entity;
        }
        
        /// <summary>
        /// Adds a notification to the database
        /// </summary>
        /// <param name="notification">The notification to add</param>
        /// <returns>the added <see cref="Notification"/></returns>
        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            EntityEntry<Notification> addedNotification = await _context.Notifications.AddAsync(notification);
            return addedNotification.Entity;
        }
        
        
        
        //
        // Methods for more specific tasks
        //
        
        /// <summary>
        /// Get a list of threads for the give page, ordered newest to oldest, with pinned threads at the top.
        /// </summary>
        /// <param name="page">The page for which to get threads for</param>
        /// <returns>The list of <see cref="Thread">threads</see> for the given page</returns>
        /// <remarks>The number of threads on each page depends on the property <see cref="ThreadsPerPage"/></remarks>
        public async Task<IEnumerable<Thread>> GetFrontPageAsync(int page)
        {
            return await _context.Threads
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.Pinned)
                .ThenByDescending(x => x.DatePosted)
                .Skip((page - 1) * ThreadsPerPage)
                .Take(ThreadsPerPage).ToListAsync();
        }
        
        /// <summary>
        /// Returns a list of replies to a thread
        /// </summary>
        /// <param name="thread">The thread to get replies from</param>
        /// <param name="page">The page of which to get replies from</param>
        /// <returns>The list of <see cref="Comment">comments</see> for the given thread and page</returns>
        /// <remarks>The number of comments on each page depends on the property <see cref="PostsPerPage"/></remarks>
        public async Task<IEnumerable<Comment>> GetThreadRepliesAsync(Thread thread, int page)
        {
            return await thread.Comments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderBy(x => x.DatePosted)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        /// <summary>
        /// Returns a list of replies to a thread of a given id
        /// </summary>
        /// <param name="threadID">The id of the thread to retrieve replies for</param>
        /// <param name="page">The page of which to get replies from</param>
        /// <returns>The list of <see cref="CommentsPerPage">comments</see> for the given id and page</returns>
        /// <remarks>The number of comments on each page depends on the property <see cref="PostsPerPage"/></remarks>
        public async Task<IEnumerable<Comment>> GetThreadRepliesAsync(int threadID, int page)
        {
            Thread thread = await GetThreadAsync(threadID);
            return await GetThreadRepliesAsync(thread, page);
        }
        
        /// <summary>
        /// Returns a list of comments on a user's page
        /// </summary>
        /// <param name="user">The user to get UserComments from</param>
        /// <param name="page">The page of which to get comments of</param>
        /// <returns>The list of <see cref="UserComment">UserComments</see> for the profile</returns>
        /// <remarks>The number of comments per page depends on the property <see cref="CommentsPerPage"/></remarks>
        public async Task<IEnumerable<UserComment>> GetUserCommentsAsync(User user, int page)
        {
            return await user.UserPageComments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage)
                .Take(CommentsPerPage)
                .AsQueryable().ToListAsync();
        }
        
        /// <summary>
        /// Returns a list of comments on a user's page of a given id
        /// </summary>
        /// <param name="userID">The id to get UserComments from</param>
        /// <param name="page">The page of which to get comments of</param>
        /// <returns>The list of <see cref="UserComment">UserComments</see> for the profile</returns>
        /// <remarks>The number of comments per page depends on the property <see cref="CommentsPerPage"/></remarks>
        public async Task<IEnumerable<UserComment>> GetUserCommentsAsync(int userID, int page)
        {
            User user = await GetUserAsync(userID);
            return await GetUserCommentsAsync(user, page);
        }
        
        /// <summary>
        /// Posts a comment
        /// </summary>
        /// <param name="comment">The comment to post</param>
        /// <returns>The posted <see cref="Comment"/></returns>
        public async Task<Comment> PostCommentAsync(Comment comment)
        {
            // Adds comment and updates the database
            await AddCommentAsync(comment);
            
            // Creates a notification if the comment creator is not the thread creator
            Thread thread = await GetThreadAsync(comment.ThreadID);
            if (comment.UserID != thread.UserID)
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
            
            return comment;
        }
        
        /// <summary>
        /// Posts a UserComment
        /// </summary>
        /// <param name="comment">The UserComment to post</param>
        /// <returns>The posted <see cref="UserComment"/></returns>
        public async Task<UserComment> PostUserCommentAsync(UserComment comment)
        {
            // Adds comment to database and saves changes
            await AddUserCommentAsync(comment);

            // Adds notification to the database if user is commenting on another profile 
            if (comment.UserID != comment.UserPageID)
            {
                Notification notification = new Notification()
                {
                    Title = $"{comment.User.Username} left a comment on your profile",
                    DateCreated = DateTime.Now,
                    UserID = comment.UserPageID
                };
                await AddNotificationAsync(notification);
            }

            return comment;
        }

        /// <summary>
        /// Deletes an IPost, checking the user is authorised to do so
        /// </summary>
        /// <param name="post">The <see cref="IPost"/> to delete</param>
        /// <param name="claimsPrincipal">The user deleting the post</param>
        /// <exception cref="InvalidOperationException">Thrown when the user does not own the poset</exception>
        public void DeleteIPost(IPost post, ClaimsPrincipal claimsPrincipal)
        {
            // Checks user owns the thread
            int userID = Tools.GetUserID(claimsPrincipal);
            if (post.UserID != userID) throw new InvalidOperationException("403 access denied");
            
            // Sets value as deleted
            post.Deleted = true;
        }
        
        /// <summary>
        /// Deletes a thread of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="threadID">The <see cref="Thread"/> to be deleted</param>
        /// <param name="claimsPrincipal">The user deleting the thread</param>
        /// <exception cref="InvalidOperationException">Thrown when the user does not own the thread</exception>
        public async Task DeleteThreadAsync(int threadID, ClaimsPrincipal claimsPrincipal)
        {
            // Retrieves the thread and deletes thread
            Thread thread = await GetThreadAsync(threadID);
            DeleteIPost(thread, claimsPrincipal);
        }

        /// <summary>
        /// Deletes a comment of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="commentID">The comment to delete</param>
        /// <param name="claimsPrincipal">The user deleting the comment</param>
        /// <exception cref="InvalidOperationException">Thrown when the user does not own the comment</exception>
        public async Task DeleteCommentAsync(int commentID, ClaimsPrincipal claimsPrincipal)
        {
            // Retrieves comment and deletes thread
            Comment comment = await GetCommentAsync(commentID);
            DeleteIPost(comment, claimsPrincipal);
        }

        /// <summary>
        /// Deletes a UserComment of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="userCommentID">The UserComment to delete</param>
        /// <param name="claimsPrincipal">The user deleting the comment</param>
        /// <exception cref="InvalidOperationException">Thrown when the user does not own the comment</exception>
        public async Task DeleteUserCommentAsync(int userCommentID, ClaimsPrincipal claimsPrincipal)
        {
            // Retrieves the userComment and deletes the thread
            UserComment comment = await GetUserCommentAsync(userCommentID);
            DeleteIPost(comment, claimsPrincipal);
        }
        
        /// <summary>
        /// Deletes an IPost as an admin
        /// </summary>
        /// <param name="post">The post to delete</param>
        /// <param name="reason">The reason for deleting the post</param>
        /// <exception cref="InvalidOperationException">Thrown when the post has already been deleted by the origin author</exception>
        public async Task AdminDeleteIPostAsync(IPost post, string reason)
        {
            // Throws exception if already deleted by user
            if (post.DeletedBy == "User") throw new InvalidOperationException("404 not found");
            
            // Sets post as deleted and sets reason
            post.Deleted = true;
            post.DeletedBy = "Admin";
            post.DeleteReason = reason;
            
            // Sets notification message based on post type
            string message = post switch
            {
                Thread thread => $"Your thread {thread.Title} has been deleted by an administrator",
                Comment comment => $"Your comment on {comment.Thread.Title} has been deleted by an administrator",
                UserComment userComment =>
                    $"Your comment on {userComment.UserPage.Username}'s profile has been deleted by an administrator",
                _ => "Your post has been removed"
            };
            
            // Creates and adds notification to database
            Notification notification = new Notification()
            {
                Title = message,
                Content = $"Reason: {reason}",
                DateCreated = DateTime.Now,
                UserID = post.UserID
            };
            await AddNotificationAsync(notification);
        }
        
        /// <summary>
        /// Deletes a thread as an admin
        /// </summary>
        /// <param name="thread">The thread to delete</param>
        /// <param name="reason">The reason to delete the thread</param>
        /// <exception cref="InvalidOperationException">Thrown when the thread has already been deleted by the original author</exception>
        public async Task AdminDeleteThreadAsync(Thread thread, string reason)
        {
            await AdminDeleteIPostAsync(thread, reason);
        }
        
        /// <summary>
        /// Deletes a thread as an admin from a given id
        /// </summary>
        /// <param name="id">The id of the thread to delete</param>
        /// <param name="reason">The reason to delete the thread</param>
        /// <exception cref="InvalidOperationException">Thrown when the thread has already been deleted by the original author</exception>
        public async Task AdminDeleteThreadAsync(int id, string reason)
        {
            Thread thread = await GetThreadAsync(id);
            await AdminDeleteThreadAsync(thread, reason);
        }
        
        /// <summary>
        /// Deletes a comment as an admin
        /// </summary>
        /// <param name="comment">The comment to delete</param>
        /// <param name="reason">The reason to delete the comment</param>
        /// <exception cref="InvalidOperationException">Thrown when the comment has already been deleted by the original author</exception>
        public async Task AdminDeleteCommentAsync(Comment comment, string reason)
        {
            await AdminDeleteIPostAsync(comment, reason);
        }

        /// <summary>
        /// Deletes a UserComment as an admin
        /// </summary>
        /// <param name="comment">The UserComment to delete</param>
        /// <param name="reason">The reason for the UserComment to be deleted</param>
        /// <exception cref="InvalidOperationException">Thrown when the UserComment has already been deleted by the original author</exception>
        public async Task AdminDeleteUserComment(UserComment comment, string reason)
        {
            await AdminDeleteIPostAsync(comment, reason);
        }
        
        /// <summary>
        /// Updates the user's profile
        /// </summary>
        /// <param name="email">The new value for the email</param>
        /// <param name="password">The new password</param>
        /// <param name="bio">The new contents of the bio</param>
        /// <param name="profilePicture">The new profile picture</param>
        /// <param name="user">The user to update</param>
        /// <remarks>Values which aren't to be updated should be passed as null</remarks>
        public async Task UpdateProfileAsync(string email, string password, string bio, Stream profilePicture, 
            User user)
        {
            // Changes information if required
            if (email != null) user.Email = email;
            if (password != null) user.Password = password;
            if (bio != null) user.Bio = bio;

            // Changes profile picture
            if (profilePicture != null)
            {
                await using MemoryStream outputImage = new MemoryStream();
                Image imageObject = await Image.LoadAsync(profilePicture);
                
                // Calculates values and crops image
                int diff = Math.Abs(imageObject.Height - imageObject.Width);
                int crop = (diff + 1) / 2;

                if (imageObject.Width > imageObject.Height)
                {
                    imageObject.Mutate(x =>
                        x.Crop(new Rectangle(crop, 0, imageObject.Height, imageObject.Height)));
                }
                else if (imageObject.Width < imageObject.Height)
                {
                    imageObject.Mutate(x =>
                        x.Crop(new Rectangle(0, crop, imageObject.Width, imageObject.Width)));
                }

                // Resizes image to 1000x1000 px if greater
                if (imageObject.Width > 1000)
                    imageObject.Mutate(x => x.Resize(1000, 1000));

                // Writes image to file
                // TODO - Update method of saving files
                await imageObject.SaveAsJpegAsync(outputImage);
                await File.WriteAllBytesAsync(
                    $"UploadedImages/ProfilePictures/{user.UserID}.jpg", outputImage.ToArray());
            }
        }
    }
}