using Microsoft.AspNetCore.WebUtilities;

namespace SimpleForum.API.Models.Responses
{
    /// <summary>
    /// An error object
    /// </summary>
    public class Error
    {
        public int Type { get; set; }
        public string Message { get; set; }
        
        // Parameterless constructor for use with json deserialization
        public Error() { }

        /// <summary>
        /// Creates an error with a message attached
        /// </summary>
        /// <param name="code">The HTTP status code to use</param>
        /// <param name="message">The message to use</param>
        public Error(int code, string message)
        {
            Type = code;
            Message = message;
        }

        /// <summary>
        /// Creates an error with a generic message
        /// </summary>
        /// <param name="code">The HTTP status code to use</param>
        public Error(int code)
        {
            Type = code;
            Message = ReasonPhrases.GetReasonPhrase(code);
        }
    }
}