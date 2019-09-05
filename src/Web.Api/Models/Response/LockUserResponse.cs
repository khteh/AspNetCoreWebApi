using System;
using System.Collections.Generic;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class LockUserResponse : ResponseBase
    {
        public LockUserResponse(bool success, List<Error> errors) : base(success, errors)
        {
        }
    }
}