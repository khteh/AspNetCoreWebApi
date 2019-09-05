using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class FindUserResponse : ResponseBase
    {
        [JsonProperty]
        public string Id { get; private set; } = string.Empty;
        [JsonProperty]
        public string Message { get; private set; } = string.Empty;
        [JsonProperty]
        public User User {get; set;}
        public FindUserResponse(string id, bool success, User user, string message, List<Error> errors) : base(success, errors)
        {
            Id = id;
            User = user;
            Message = message;
        }
    }
}