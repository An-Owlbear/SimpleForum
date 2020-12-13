namespace SimpleForum.API.Client
{
    public interface ITokenStorage
    {
        /// <summary>
        /// Sets the given token
        /// </summary>
        /// <param name="token">The token value</param>
        public void SetToken(string token);

        /// <summary>
        /// Gets the value of the token
        /// </summary>
        public string GetToken();
    }
}