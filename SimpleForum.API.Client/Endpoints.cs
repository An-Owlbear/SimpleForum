using System.Net.Http;

namespace SimpleForum.API.Client
{
    /// <summary>
    /// A class containing all API endpoints
    /// </summary>
    public static class Endpoints
    {
        // Authentication endpoint
        public static readonly Endpoint Login = new Endpoint("/Auth/Login", HttpMethod.Post);
        public static readonly Endpoint GenerateToken = new Endpoint("/Auth/GenerateTempToken", HttpMethod.Post, true);
        
        // Thread endpoints
        public static readonly Endpoint FrontPage = new Endpoint("/Threads", HttpMethod.Get);
        public static readonly Endpoint Thread = new Endpoint("/Threads/:id", HttpMethod.Get);
        public static readonly Endpoint ThreadComments = new Endpoint("/Threads/:id/Comments", HttpMethod.Get);
        public static readonly Endpoint CreateThread = new Endpoint("/Threads", HttpMethod.Put, true);
        public static readonly Endpoint PostComment = new Endpoint("/Threads/:id/Comments", HttpMethod.Put, true);
        public static readonly Endpoint DeleteThread = new Endpoint("/Threads/:id", HttpMethod.Delete, true);
        
        // Comment endpoints
        public static readonly Endpoint GetComment = new Endpoint("/Comments/:id", HttpMethod.Get);
        public static readonly Endpoint DeleteComment = new Endpoint("/Comments/:id", HttpMethod.Delete, true);
        
        // User endpoints
        public static readonly Endpoint GetUser = new Endpoint("/Users/:id", HttpMethod.Get);
        public static readonly Endpoint UserComments =  new Endpoint("/Users/:id/Comments", HttpMethod.Get);
        public static readonly Endpoint PostUserComment = new Endpoint("/Users/:id/Comments", HttpMethod.Put, true);
        
        // UserComment endpoints
        public static readonly Endpoint GetUserComment = new Endpoint("/UserComments/:id", HttpMethod.Get);
        public static readonly Endpoint DeleteUserComment = new Endpoint("/UserComments/:id", HttpMethod.Delete, true);
        
        // Admin endpoints
        public static readonly Endpoint AdminUpdateThread = new Endpoint("/Admin/Threads/:id", HttpMethod.Patch, true);
        public static readonly Endpoint AdminDeleteThread = new Endpoint("/Admin/Threads/:id", HttpMethod.Delete, true);
        public static readonly Endpoint AdminDeleteComment = new Endpoint("/Admin/Comments/:id", HttpMethod.Delete, true);
        public static readonly Endpoint AdminDeleteUserComment = new Endpoint("/Admin/UserComments/:id", HttpMethod.Delete, true);
        
        // Files related endpoints
        public static readonly Endpoint ProfilePicture = new Endpoint("/Files/ProfileImg", HttpMethod.Get);
    }
}