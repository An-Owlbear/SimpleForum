using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client
{
    public static class ResponseParser
    {
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        // Attempts to parse a HttpResponseMessage to the given type, returning an error if unsuccessful
        public static async Task<Result<T>> ParseJsonResponse<T>(HttpResponseMessage response)
        {
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await TryDeserializeJson<T>(stream).ConfigureAwait(false);
            }

            Result<Error> result = await TryDeserializeJson<Error>(stream).ConfigureAwait(false);
            return result.Success
                ? Result.Fail<T>(result.Value.Message, result.Value.Type)
                : Result.Fail<T>(result.Error, result.Code);
        }

        // Attempts to parse a HttpResponseMessage when an empty body is expected
        public static async Task<Result> ParseJsonResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return Result.Ok();
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            Result<Error> result = await TryDeserializeJson<Error>(stream).ConfigureAwait(false);
            return result.Success
                ? Result.Fail(result.Value.Message, result.Value.Type)
                : Result.Fail(result.Error, result.Code);
        }

        // Attempts parsing a stream to the given type, returning a Result
        private static async Task<Result<T>> TryDeserializeJson<T>(Stream json)
        {
            try
            {
                T deserialized = await JsonSerializer.DeserializeAsync<T>(json, jsonOptions).ConfigureAwait(false);
                return Result.Ok(deserialized);
            }
            catch
            {
                return Result.Fail<T>("Invalid response", 500);
            }
        }

        // Converts a response to a stream if successful otherwise returns failure
        public static async Task<Result<Stream>> ParseStreamResponse(HttpResponseMessage response)
        {
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode) return Result.Ok(stream);

            Result<Error> result = await TryDeserializeJson<Error>(stream);
            return result.Success
                ? Result.Fail<Stream>(result.Value.Message, result.Value.Type)
                : Result.Fail<Stream>(result.Error, result.Code);
        }
        
        // Converts a response to a string if successful, otherwise returns failure
        public static async Task<Result<string>> ParseStringResponse(HttpResponseMessage response)
        {
            // Gets stream result, returning if failure
            Result<Stream> result = await ParseStreamResponse(response).ConfigureAwait(false);
            if (result.Failure) return Result.Fail<string>(result.Error, result.Code);

            using StreamReader streamReader = new StreamReader(result.Value);
            string responseString = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            return Result.Ok(responseString);
        }
    }
}