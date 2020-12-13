namespace SimpleForum.API.Client
{
    /// <summary>
    /// A basic implementation of ITokenStorage
    /// </summary>
    public class TokenStorage : ITokenStorage
    {
        private string _token;
        
        public void SetToken(string token)
        {
            _token = token;
        }

        public string GetToken()
        {
            return _token;
        }
    }
}