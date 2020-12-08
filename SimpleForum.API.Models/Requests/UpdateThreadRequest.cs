namespace SimpleForum.API.Models.Requests
{
    public class UpdateThreadRequest
    {
        public bool? Pinned { get; set; }
        public bool? Locked { get; set; }
    }
}
