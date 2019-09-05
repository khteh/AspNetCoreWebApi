using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class FindUserResponse : UseCaseResponseMessage
    {
        [JsonProperty]
        public User User { get; private set;}
        public FindUserResponse(User user, string id, bool success = false, string message = null, List<Error> errors = null) : base(id, success, message, errors) { User = user; }
    }
}