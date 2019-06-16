using System;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Interfaces;
using Newtonsoft.Json;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class SignInResponse : UseCaseResponseMessage
    {
        [JsonProperty]
        public string Id { get; private set; } = string.Empty;

        public SignInResponse(List<Error> errors, bool success = false, string message = null) : base(success, message, errors) { }

        public SignInResponse(string id, bool success = false, string message = null) : base(success, message) {}
        [JsonConstructor]
        public SignInResponse(string id, bool success = false, string message = null, List<Error> errors = null) : base(success, message, errors) {}
    }
}