using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class RegisterUserResponse : ResponseBase
    {
        public string Id { get; init; } = string.Empty;
        [JsonConstructor]
        public RegisterUserResponse(string id, bool success, List<Error> errors) : base(success, errors) => Id = id;
    }
}