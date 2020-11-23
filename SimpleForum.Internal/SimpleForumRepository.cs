using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
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
        private readonly IEmailService _emailService;
        private readonly SimpleForumConfig _config;
        private readonly HttpContext _httpContext;

        private const int ThreadsPerPage = 30;
        private const int PostsPerPage = 30;
        private const int CommentsPerPage = 15;
        private readonly List<PendingEmail> PendingEmails = new List<PendingEmail>();

        /// <summary>
        /// Creates an instance of <see cref="SimpleForumRepository"/>
        /// </summary>
        /// <param name="context">The database context for which to initialise the repository with</param>
        /// <param name="emailService">The email service used to send emails</param>
        /// <param name="config">The filename of settings file to use</param>
        /// <param name="contextAccessor">Used to access the HTTP context</param>
        public SimpleForumRepository(ApplicationDbContext context, IEmailService emailService,
            IOptions<SimpleForumConfig> config, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _emailService = emailService;
            _config = config.Value;
            _httpContext = contextAccessor.HttpContext;
        }

        /// <summary>
        /// Saves any changes made to the database and sends pending emails
        /// </summary>
        public async Task SaveChangesAsync()
        {
            // Saves database changes
            await _context.SaveChangesAsync();

            // Creates list of tasks for sending pending emails
            IEnumerable<Task> emailTasks = PendingEmails.Select(async x =>
            {
                await _emailService.SendAsync(x.MailTo, x.Subject, x.Message, x.IsHTML);
            });

            // Awaits sending emails
            await Task.WhenAll(emailTasks);
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
        /// Gets a user of the given username or email
        /// </summary>
        /// <param name="username">The username/email of the user to find</param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> GetUserAsync(string username)
        {
            return username switch
            {
                _ when username.Contains('@') => await _context.Users.FirstOrDefaultAsync(x => x.Email == username),
                _ => await _context.Users.FirstOrDefaultAsync(x => x.Username == username)
            };
        }

        /// <summary>
        /// Returns the user of the given ClaimsPrincipal
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal of the user to find</param>
        /// <returns><see cref="User"/></returns>
        public async Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated) return null;
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

        /// <summary>
        /// Returns the matching authentication token
        /// </summary>
        /// <param name="token">The token to return</param>
        /// <returns><see cref="AuthToken"/></returns>
        public async Task<AuthToken> GetAuthTokenAsync(string token)
        {
            return await _context.AuthTokens.FindAsync(token);
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

        /// <summary>
        /// Adds an authentication token to the database 
        /// </summary>
        /// <param name="authToken">The authentication token to be added</param>
        /// <returns>the added <see cref="AuthToken"/></returns>
        public async Task<AuthToken> AddAuthTokenAsync(AuthToken authToken)
        {
            EntityEntry<AuthToken> addedToken = await _context.AuthTokens.AddAsync(authToken);
            return addedToken.Entity;
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
        /// Returns the number of pages of threads
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetPageCountAsync()
        {
            return (await _context.Threads.CountAsync(x => !x.Deleted) + (ThreadsPerPage - 1)) / ThreadsPerPage;
        }

        /// <summary>
        /// Returns a list of replies to a thread
        /// </summary>
        /// <param name="thread">The thread to get replies from</param>
        /// <param name="page">The page of which to get replies from</param>
        /// <returns>The list of <see cref="Comment">comments</see> for the given thread and page</returns>
        /// <remarks>The number of comments on each page depends on the property <see cref="PostsPerPage"/></remarks>
        public IEnumerable<Comment> GetThreadReplies(Thread thread, int page)
        {
            // Returns empty list if thread is null
            if (thread == null) return Enumerable.Empty<Comment>();
            
            // Returns list of comments
            return thread.Comments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderBy(x => x.DatePosted)
                .Skip((page - 1) * PostsPerPage)
                .Take(PostsPerPage);
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
            return GetThreadReplies(thread, page);
        }

        /// <summary>
        /// Returns a list of comments on a user's page
        /// </summary>
        /// <param name="user">The user to get UserComments from</param>
        /// <param name="page">The page of which to get comments of</param>
        /// <returns>The list of <see cref="UserComment">UserComments</see> for the profile</returns>
        /// <remarks>The number of comments per page depends on the property <see cref="CommentsPerPage"/></remarks>
        public IEnumerable<UserComment> GetUserComments(User user, int page)
        {
            // Returns empty list of user is null
            if (user == null) return Enumerable.Empty<UserComment>();
            
            // Returns list of user comments
            return user.UserPageComments
                .Where(x => !x.Deleted && !x.User.Deleted)
                .OrderByDescending(x => x.DatePosted)
                .Skip((page - 1) * CommentsPerPage)
                .Take(CommentsPerPage);
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
            return GetUserComments(user, page);
        }

        /// <summary>
        /// Posts a comment, creating a notification if needed
        /// </summary>
        /// <param name="comment">The comment to post</param>
        /// <returns>The posted <see cref="Comment"/></returns>
        public async Task<Comment> PostCommentAsync(Comment comment)
        {
            // Retrieves user and adds comment to database
            User user = await GetUserAsync(_httpContext.User);
            comment.User = user;
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
        /// Posts a UserComment, creating a notification if needed
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
                    Content = $"Click [here]({_config.InstanceURL}/User?id={comment.UserPageID}) to view.",
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
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Post does not exist - 404
        /// - User does not own post - 403
        /// </returns>
        public Result DeleteIPost(IPost post)
        {
            // Returns 404 if post null
            if (post == null) return Result.Fail("Not found", 404);
            
            // Checks user owns the thread
            int userID = Tools.GetUserID(_httpContext.User);
            if (post.UserID != userID) return Result.Fail("Access denied", 403);

            // Sets value as deleted
            post.Deleted = true;
            return Result.Ok();
        }

        /// <summary>
        /// Deletes a thread of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="threadID">The <see cref="Thread"/> to be deleted</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Thread does not exist - 404
        /// - User does not own thread - 403
        /// </returns>
        public async Task<Result> DeleteThreadAsync(int threadID)
        {
            // Retrieves the thread and deletes thread
            Thread thread = await GetThreadAsync(threadID);
            return DeleteIPost(thread);
        }

        /// <summary>
        /// Deletes a comment of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="commentID">The comment to delete</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Comment does not exist - 404
        /// - User does not own comment - 403
        /// </returns>
        public async Task<Result> DeleteCommentAsync(int commentID)
        {
            // Retrieves comment and deletes thread
            Comment comment = await GetCommentAsync(commentID);
            return DeleteIPost(comment);
        }

        /// <summary>
        /// Deletes a UserComment of the given id, ensuring the user is verified to do so
        /// </summary>
        /// <param name="userCommentID">The UserComment to delete</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - UserComment does not exist - 404
        /// - User does not own UserComment - 403
        /// </returns>
        public async Task<Result> DeleteUserCommentAsync(int userCommentID)
        {
            // Retrieves the userComment and deletes the thread
            UserComment comment = await GetUserCommentAsync(userCommentID);
            return DeleteIPost(comment);
        }

        /// <summary>
        /// Deletes an IPost as an admin
        /// </summary>
        /// <param name="post">The post to delete</param>
        /// <param name="reason">The reason for deleting the post</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Post does not exist - 404
        /// - User has already deleted post - 404
        /// </returns>
        public async Task<Result> AdminDeleteIPostAsync(IPost post, string reason)
        {
            // Returns error if post doesn't exist or is already deleted by user
            if (post.DeletedBy == "User") return Result.Fail("Post not found", 404);

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
            return Result.Ok();
        }

        /// <summary>
        /// Deletes a thread as an admin from a given id
        /// </summary>
        /// <param name="id">The id of the thread to delete</param>
        /// <param name="reason">The reason to delete the thread</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Thread does not exist - 404
        /// - User has already deleted thread - 404
        /// </returns>
        public async Task<Result> AdminDeleteThreadAsync(int id, string reason)
        {
            Thread thread = await GetThreadAsync(id);
            return await AdminDeleteIPostAsync(thread, reason);
        }

        /// <summary>
        /// Deletes a thread as an admin for the given id
        /// </summary>
        /// <param name="id">The id of the comment to delete</param>
        /// <param name="reason">The reason to delete the comment</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - Comment does not exist - 404
        /// - User has already deleted comment - 404
        /// </returns>
        public async Task<Result> AdminDeleteCommentAsync(int id, string reason)
        {
            Comment comment = await GetCommentAsync(id);
            return await AdminDeleteIPostAsync(comment, reason);
        }

        /// <summary>
        /// Deletes a UserComment as an admin for the given 
        /// </summary>
        /// <param name="id">The id of the UserComment to delete</param>
        /// <param name="reason">The reason to delete the UserComment</param>
        /// <returns>
        /// Returns failure under the following circumstances:
        /// - UserComment does not exist - 404
        /// - User has already deleted UserComment - 404
        /// </returns>
        public async Task<Result> AdminDeleteUserCommentAsync(int id, string reason)
        {
            UserComment comment = await GetUserCommentAsync(id);
            return await AdminDeleteIPostAsync(comment, reason);
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

        /// <summary>
        /// Initiates the deletion of a users' account
        /// </summary>
        /// <param name="user">The user to delete</param>
        public async Task StartDeleteAccountAsync(User user)
        {
            // Creates and adds the email code
            DateTime timeNow = DateTime.Now;
            EmailCode code = new EmailCode()
            {
                Code = Tools.GenerateCode(32),
                Valid = true,
                DateCreated = timeNow,
                ValidUntil = timeNow.AddHours(1),
                UserID = user.UserID
            };
            await AddEmailCodeAsync(code);

            // Adds email to pending emails
            string url = _config.InstanceURL + "/User/SendDelete?code=" + code.Code;
            PendingEmail email = new PendingEmail()
            {
                MailTo = user.Email,
                Subject = "SimpleForum account deletion confirmation",
                Message =
                    $"<p>Please click the following link to confirm your account deletion: <a href=\"{url}\">{url}</a>" +
                    "<br>If you did not try to delete your account please change your password to prevent further unauthorised access" +
                    "of your account.</p>",
                IsHTML = true
            };
            PendingEmails.Add(email);
        }

        /// <summary>
        /// Signs up a new user
        /// </summary>
        /// <param name="user">The user to be signed up</param>
        /// <returns>The signed up user</returns>
        public async Task<Result<User>> SignupAsync(User user)
        {
            // Returns failure if any relevant details are null
            if (user.Username == null || user.Email == null || user.Password == null)
            {
                return Result.Fail<User>("Incomplete details", 400);
            }

            // Returns failure if the email or username are already in use
            if (_context.Users.Any(x => x.Email == user.Email))
            {
                return Result.Fail<User>("Duplicate email", 400);
            }

            if (_context.Users.Any(x => x.Username == user.Username))
            {
                return Result.Fail<User>("Duplicate username", 400);
            }

            // Finds next user id and sets new user to it
            int highestID = 0;
            if (_context.Users.Count() != 0)
            {
                highestID = _context.Users.OrderByDescending(x => x.UserID).First().UserID;
            }

            user.UserID = highestID + 1;

            // Sets signup date and activated
            user.SignupDate = DateTime.Now;
            user.Activated = false;

            // Creates EmailCode
            string code = Tools.GenerateCode(32);
            DateTime currentTime = DateTime.Now;
            EmailCode emailCode = new EmailCode()
            {
                Code = code,
                Type = "signup",
                Valid = true,
                DateCreated = currentTime,
                ValidUntil = currentTime.AddHours(24),
                UserID = user.UserID
            };

            // Adds user and email code
            await AddUserAsync(user);
            await AddEmailCodeAsync(emailCode);

            // Adds pending email to list
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + emailCode.Code;
            PendingEmail email = new PendingEmail()
            {
                MailTo = user.Email,
                Subject = "SimpleForum email confirmation",
                Message = "<p>please confirm your email by clicking the following link: <a href='" + url + "'>" + url
                          + "</a></p>",
                IsHTML = true
            };
            PendingEmails.Add(email);

            return Result.Ok(user);
        }

        /// <summary>
        /// Creates a new EmailCode for email verification
        /// </summary>
        /// <param name="user">The user to send a new code for</param>
        /// <returns>The created EmailCode</returns>
        public async Task<EmailCode> ResendSignupCode(User user)
        {
            // Retrieves a list of previous signup email codes and sets them as no longer valid
            IEnumerable<EmailCode> codes =
                _context.EmailCodes.Where(x => x.Type == "signup" && x.UserID == user.UserID);
            foreach (EmailCode emailCode in codes)
            {
                emailCode.Valid = false;
            }

            // Retrieves user and creates new EmailCode
            DateTime timeNow = DateTime.Now;
            EmailCode newCode = new EmailCode()
            {
                Code = Tools.GenerateCode(32),
                Type = "signup",
                Valid = true,
                DateCreated = timeNow,
                ValidUntil = timeNow.AddHours(24),
                UserID = user.UserID
            };
            await AddEmailCodeAsync(newCode);

            // Adds email to pending emails
            string url = _config.InstanceURL + "/Signup/VerifyEmail?code=" + newCode.Code;
            PendingEmail email = new PendingEmail()
            {
                MailTo = user.Email,
                Subject = "SimpleForum email confirmation",
                Message = "<p>please confirm your email by clicking the following link: <a href='" + url +
                          "'>" + url + "</a></p>",
                IsHTML = true
            };
            PendingEmails.Add(email);

            return newCode;
        }

        /// <summary>
        /// Requests a user's password reset, sending an email
        /// </summary>
        /// <param name="user">The user to reset the password of</param>
        public async Task RequestPasswordResetAsync(User user)
        {
            // Creates EmailCode
            DateTime timeNow = DateTime.Now;
            EmailCode emailCode = new EmailCode()
            {
                Code = Tools.GenerateCode(32),
                DateCreated = timeNow,
                Type = "password_reset",
                Valid = true,
                ValidUntil = timeNow.AddHours(1),
                UserID = user.UserID
            };
            await AddEmailCodeAsync(emailCode);

            // Schedules password reset email
            string url = _config.InstanceURL + "/Login/ResetPassword?code=" + emailCode.Code;
            PendingEmail email = new PendingEmail()
            {
                MailTo = user.Email,
                Subject = "SimpleForum password reset",
                Message = "<p>To reset your password, please click the following link: <a href=\"" + url +
                          "\">" + url + "</a></p>",
                IsHTML = true
            };
            PendingEmails.Add(email);
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="password">The new password</param>
        /// <param name="code">The code to verify the change</param>
        /// <param name="userID">The id of the user having their password changed</param>
        /// <returns>Returns failure when the code or user is invalid</returns>
        public async Task<Result> ResetPasswordAsync(string password, string code, int userID)
        {
            // Retrieves user and EmailCode
            User user = await GetUserAsync(userID);
            EmailCode emailCode = await GetEmailCodeAsync(code);

            // Returns failure if code is invalid
            if (emailCode.Code != code || emailCode.User != user || emailCode.ValidUntil < DateTime.Now)
            {
                return Result.Fail("Access denied", 403);
            }

            // Updates the password
            user.Password = password;
            return Result.Ok();
        }

        /// <summary>
        /// Creates an authentication token, ensuring it is unique
        /// </summary>
        /// <param name="userID">The user to create an authentication token for</param>
        /// <returns>The created <see cref="AuthToken"/></returns>
        public async Task<AuthToken> CreateAuthTokenAsync(int userID)
        {
            // Loops until a unique auth token is created
            while (true)
            {
                // Creates and adds auth token if token is unique
                string code = Tools.GenerateCode(32);
                if (await GetAuthTokenAsync(code) == null)
                {
                    AuthToken token = new AuthToken()
                    {
                        Token = code,
                        ValidUntil = DateTime.Now.AddMonths(1),
                        UserID = userID
                    };
                    await AddAuthTokenAsync(token);
                    
                    // Returns auth token
                    return token;
                }
            }
        }
    }
}