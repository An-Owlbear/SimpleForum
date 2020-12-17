using System;
using SimpleForum.API.Models.Responses;

namespace SimpleForum.API.Client.Tests
{
    public class DisplayItems
    {
        public static void DisplayThread(ApiThread thread)
        {
            Console.WriteLine($"Title - {thread.Title}\n" +
                              $"ID - {thread.ID}\n" +
                              $"Content - {thread.Content}\n" +
                              $"Pinned - {thread.Pinned}\n" +
                              $"Locked - {thread.Locked}\n" +
                              $"Replies - {thread.Replies}");
        }

        public static void DisplayError(Result error)
        {
            Console.WriteLine($"Error code - {error.Code}" +
                              $"Error message - {error.Error}");
        }
    }
}