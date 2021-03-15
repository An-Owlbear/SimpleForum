using System;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client.Tests
{
    public class DisplayItems
    {
        // Displays a thread in the console
        public static void DisplayThread(ApiThread thread)
        {
            Console.WriteLine($"Title - {thread.Title}\n" +
                              $"ID - {thread.ID}\n" +
                              $"Content - {thread.Content}\n" +
                              $"Date posted - {thread.DatePosted.ToShortDateString()} {thread.DatePosted.ToShortTimeString()}\n" +
                              $"Pinned - {thread.Pinned}\n" +
                              $"Locked - {thread.Locked}\n" +
                              $"Replies - {thread.Replies}");
            Console.WriteLine("========= User ========");
            DisplayUser(thread.User);
        }

        // Displays a user in the console
        public static void DisplayUser(ApiUser user)
        {
            Console.WriteLine($"Username - {user.Username}\n" +
                              $"ID - {user.ID}\n" +
                              $"Date joined - {user.DateJoined}\n" +
                              $"Posts - {user.Posts}\n" +
                              $"Comments - {user.Comments}\n" +
                              $"Banned - {user.Banned}\n" +
                              $"Comments locked - {user.CommentsLocked}");
        }

        // Displays a comment in the console
        public static void DisplayComment(ApiComment comment)
        {
            Console.WriteLine($"ID - {comment.ID}\n" +
                              $"Content - {comment.Content}\n" +
                              $"Date posted - {comment.DatePosted}\n" +
                              $"Type - {comment.Type}");
            Console.WriteLine("========= User ========"); 
            DisplayUser(comment.User);
        }

        // Displays error in the console
        public static void DisplayError(Result error)
        {
            Console.WriteLine($"Error code - {error.Code}\n" +
                              $"Error message - {error.Error}");
        }
    }
}