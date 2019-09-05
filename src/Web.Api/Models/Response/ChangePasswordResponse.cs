using System;
using System.Collections.Generic;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ChangePasswordResponse : ResponseBase
    {
        public ChangePasswordResponse(bool success, List<Error> errors) : base(success, errors)
        {
        }
    }
}