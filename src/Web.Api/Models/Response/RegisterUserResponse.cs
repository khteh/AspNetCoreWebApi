using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class RegisterUserResponse : ResponseBase
    {
        [JsonProperty]
        public string Id { get; private set; } = string.Empty;
        public RegisterUserResponse(string id, bool success, List<Error> errors) : base(success, errors)
        {
            Id = id;
        }
    }
}