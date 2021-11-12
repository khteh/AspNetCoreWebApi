using System.Collections.Generic;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record ChangePasswordResponse(bool Success, List<Error> Errors) : ResponseBase(Success, Errors);