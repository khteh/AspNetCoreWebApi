using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ChangePasswordResponse : ResponseBase
    {
        [JsonConstructor]
        public ChangePasswordResponse(bool success, List<Error> errors) : base(success, errors)
        {
        }
    }
}