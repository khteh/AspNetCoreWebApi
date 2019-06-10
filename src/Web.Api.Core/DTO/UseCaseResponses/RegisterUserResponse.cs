using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class RegisterUserResponse : UseCaseResponseMessage 
    {
        public string Id { get; }
        public RegisterUserResponse(List<Error> errors, bool success=false, string message=null) : base(success, message, errors)
        {
        }
        public RegisterUserResponse(string id, bool success = false, string message = null) : base(success, message)
        {
            Id = id;
        }
    }
}