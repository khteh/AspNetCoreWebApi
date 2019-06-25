
using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.DTO;

namespace Web.Api.Core.Interfaces
{
    public class UseCaseResponseMessage
    {
        [JsonProperty]
        public string Id { get; private set; } = string.Empty;
        [JsonProperty]
        public bool Success { get; private set; } = false;
        [JsonProperty]
        public string Message { get; private set; } = string.Empty;
        [JsonProperty]
        public List<Error> Errors { get; private set; } = new List<Error>();
        [JsonConstructor]
        public UseCaseResponseMessage(string id, bool success = false, string message = null, List<Error> errors = null)
        {
            Id = id;
            Success = success;
            Message = message;
            Errors = errors;
        }
        public UseCaseResponseMessage(string message, List<Error> errors)
        {
            Message = message;
            Errors = errors;
        }
        public UseCaseResponseMessage(List<Error> errors) => Errors = errors;
        public UseCaseResponseMessage(string message)
        {
            Message = message;
            Errors.Add(new Error(string.Empty, message));
        }
    }

}