using System;
using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.Dto.UseCaseResponses
{
    public class ChangePasswordResponse : UseCaseResponseMessage
    {
        public string Id { get; }
        public IEnumerable<Error> Errors { get; }
        public ChangePasswordResponse(IEnumerable<Error> errors, bool success = false, string message = null) : base(success, message)
        {
            Errors = errors;
        }

        public ChangePasswordResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            Id = id;
        }
    }
}