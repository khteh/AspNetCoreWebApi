using System.Security.Claims;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.UseCaseRequests;

public class SignInWithClaimsRequest : IUseCaseRequest<SignInResponse>
{
    public string IdentityId { get; init; }
    public List<Claim> Claims { get; init; }
    public AuthenticationProperties AuthProperties { get; init; }
    public SignInWithClaimsRequest() { }
    public SignInWithClaimsRequest(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
    {
        IdentityId = identityId;
        Claims = claims;
        AuthProperties = authProperties;
    }
}