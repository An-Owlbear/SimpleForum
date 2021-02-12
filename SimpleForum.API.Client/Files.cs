using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleForum.Common;

namespace SimpleForum.API.Client
{
    public partial class SimpleForumClient
    {
        /// <summary>
        /// Download the profile picture of the given user as a stream
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns></returns>
        public async Task<Result<Stream>> GetProfileImg(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };
            
            // Sends request and returns result
            HttpResponseMessage response = await _requestsClient.SendRequest(Endpoints.ProfilePicture, parameters)
                .ConfigureAwait(false);
            return await ResponseParser.ParseStreamResponse(response);
        }

        public async Task<Uri> GetProfileImgUrl(int id)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "id", id.ToString() }
            };

            return await _requestsClient.BuildUrl(Endpoints.ProfilePicture, parameters);
        }
    }
}