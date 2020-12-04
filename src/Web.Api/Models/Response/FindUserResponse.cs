using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class FindUserResponse : ResponseBase
    {
        public string Id { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public User User {get; set;}
        [JsonConstructor]
        public FindUserResponse(string id, bool success, User user, string message, List<Error> errors) : base(success, errors)
        {
            Id = id;
            User = user;
            Message = message;
        }
    }
}