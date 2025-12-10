using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests;

public class SignInWithClaimsRequest : IUseCaseRequest<SignInResponse>
{
    public required string IdentityId { get; init; }
    public List<Claim> Claims { get; init; }
    public AuthenticationProperties AuthProperties { get; init; }
    [SetsRequiredMembers]
    public SignInWithClaimsRequest(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
    {
        IdentityId = identityId;
        Claims = claims;
        AuthProperties = authProperties;
    }
}