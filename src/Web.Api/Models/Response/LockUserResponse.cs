using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class LockUserResponse : ResponseBase
    {
        [JsonConstructor]
        public LockUserResponse(bool success, List<Error> errors) : base(success, errors)
        {
        }
    }
}