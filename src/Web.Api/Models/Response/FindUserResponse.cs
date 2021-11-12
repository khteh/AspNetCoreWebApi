using System.Collections.Generic;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record FindUserResponse(string Id, string Message, User User, bool Success, List<Error> Errors) : ResponseBase(Success, Errors);