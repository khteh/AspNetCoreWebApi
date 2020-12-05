using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public record RegisterUserResponse(string Id, bool Success, List<Error> Errors) : ResponseBase(Success, Errors);
}