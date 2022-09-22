using Web.Api.Core.DTO;

namespace Web.Api.Models.Response;

public record SignInResponse(bool Success, List<Error> Errors) : ResponseBase(Success, Errors);