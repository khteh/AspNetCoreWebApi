using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ResponseBase
    {
        [JsonProperty]
        public bool Success { get; set; } = false;
        [JsonProperty]
        public List<Error> Errors { get; set; } = new List<Error>();
        public ResponseBase(bool success, List<Error> errors)
        {
            Success = success;
            Errors = errors;
        }
    }
}