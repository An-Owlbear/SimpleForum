﻿using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleForum.API.Models.Responses;
using SimpleForum.Common;

namespace SimpleForum.API.Client
{
    public static class Json
    {
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        // Attempts to parse a HttpResponseMessage to the given type, returning an error if unsuccessful
        public static async Task<Result<T>> ParseHttpResponse<T>(HttpResponseMessage response)
        {
            Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await TryDeserialize<T>(stream).ConfigureAwait(false);
            }

            Result<Error> result = await TryDeserialize<Error>(stream).ConfigureAwait(false);
            return result.Success
                ? Result.Fail<T>(result.Value.Message, result.Value.Type)
                : Result.Fail<T>(result.Error, result.Code);
        }

        // Attempts parsing a stream to the given type, returning a Result
        private static async Task<Result<T>> TryDeserialize<T>(Stream json)
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
    }
}