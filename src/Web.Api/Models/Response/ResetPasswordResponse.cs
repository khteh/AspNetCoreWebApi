using System;
using System.Collections.Generic;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ResetPasswordResponse : ResponseBase
    {
        public ResetPasswordResponse(bool success, List<Error> errors) : base(success, errors)
        {
        }
    }
}