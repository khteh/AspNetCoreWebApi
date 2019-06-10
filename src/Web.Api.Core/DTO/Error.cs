using System;
namespace Web.Api.Core.DTO
{
    public sealed class Error
    {
        public string Code { get; }
        public string Description { get; }
        public Error(string code, string description)
        {
            Code = code;
            Description = description;
        }
    }
}