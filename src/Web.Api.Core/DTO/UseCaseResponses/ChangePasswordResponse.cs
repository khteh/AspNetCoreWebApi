using System;
using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class ChangePasswordResponse : UseCaseResponseMessage
    {
        public string Id { get; }
        public ChangePasswordResponse() {}
        public ChangePasswordResponse(List<Error> errors, bool success = false, string message = null) : base(success, message, errors)
        {
        }

        public ChangePasswordResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            Id = id;
        }
    }
}