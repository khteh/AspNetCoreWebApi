
using System.Collections.Generic;
using Web.Api.Core.DTO;

namespace Web.Api.Core.Interfaces
{
    public abstract class UseCaseResponseMessage
    {
        public bool Success { get; }
        public string Message { get; }
        public List<Error> Errors { get; }
        protected UseCaseResponseMessage(bool success = false, string message = null, List<Error> errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }
    }
}