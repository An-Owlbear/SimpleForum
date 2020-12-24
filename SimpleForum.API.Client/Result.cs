using System.Diagnostics.CodeAnalysis;

namespace SimpleForum.API.Client
{
    /// <summary>
    /// Represents the result of an operation
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool Success { get; private set; }
        
        /// <summary>
        /// Contains the error message if the operation failed
        /// </summary>
        public string Error { get; private set; }
        
        /// <summary>
        /// Contains the http error code
        /// </summary>
        public int Code { get; private set; }
        
        /// <summary>
        /// Indicates whether the operation failed
        /// </summary>
        public bool Failure => !Success;

        protected Result(bool success, string error, int code)
        {
            Success = success;
            Error = error;
            Code = code;
        }
        
        /// <summary>
        /// Returns a generic failure result of the given message
        /// </summary>
        /// <param name="message">The message to return a result for</param>
        /// <param name="code">The HTTP status code of the result</param>
        /// <returns></returns>
        public static Result Fail(string message, int code) => new Result(false, message, code);

        /// <summary>
        /// Returns a failure result of the given message and type
        /// </summary>
        /// <param name="message">The message to return a result for</param>
        /// <param name="code">The HTTP status code the result</param>
        /// <typeparam name="T">The type to return a result for</typeparam>
        /// <returns></returns>
        public static Result<T> Fail<T>(string message, int code) => new Result<T>(default,false, message, code);

        /// <summary>
        /// Returns a generic success result
        /// </summary>
        /// <returns></returns>
        public static Result Ok() => new Result(true, string.Empty, 200);
        
        /// <summary>
        /// Returns a success result of the given type
        /// </summary>
        /// <param name="value">The value for the result to contain</param>
        /// <typeparam name="T">The type of the result</typeparam>
        /// <returns></returns>
        public static Result<T> Ok<T>(T value) => new Result<T>(value, true, string.Empty, 400);
    }

    /// <summary>
    /// Represents a result class of a given type
    /// </summary>
    public class Result<T> : Result
    {
        public T Value { get; private set; }
        
        protected internal Result([AllowNull] T value, bool success, string error, int code) : base(success, error, code)
        {
            Value = value;
        }
    }
}