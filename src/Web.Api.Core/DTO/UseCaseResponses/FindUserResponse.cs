using System;
using System.Collections.Generic;
using System.Text;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class FindUserResponse : UseCaseResponseMessage
    {
        public string Id { get; }
        public FindUserResponse(List<Error> errors, bool success = false, string message = null) : base(success, message, errors)
        {
        }
        public FindUserResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            Id = id;
        }
    }
}