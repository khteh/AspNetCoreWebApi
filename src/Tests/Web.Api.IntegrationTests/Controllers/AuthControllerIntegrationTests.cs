using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Extensions;
using Web.Api.Infrastructure.Auth;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Mapping;
using Web.Api.Infrastructure.Helpers;
using Web.Api.Infrastructure.Identity;
using Web.Api.Models.Settings;
using Web.Api.Presenters;
using Microsoft.Extensions.Configuration;
using Web.Api.Core.Interfaces.UseCases;
using Microsoft.Extensions.Options;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Web.Api.IntegrationTests.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private CustomWebApplicationFactory<Startup> _factory;
        public AuthControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CanLoginWithValidCredentials()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest{UserName = "mickeymouse", Password = "Pa$$W0rd1" }), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.NotNull(result.accessToken.token);
            Assert.Equal(7200,(int)result.accessToken.expiresIn);
            Assert.NotNull(result.refreshToken);
        }

        [Fact]
        public async Task CantLoginWithInvalidCredentials()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest { UserName = "unknown", Password = "Rhcp1234" }), Encoding.UTF8, "application/json"));
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid username or password.", stringResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [Fact]
        public async Task CanExchangeValidRefreshToken()
        {
            var httpResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonConvert.SerializeObject(new Models.Request.ExchangeRefreshTokenRequest {
                AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtaWNrZXltb3VzZSIsImp0aSI6IjhkYjQ0ODgzLWJiNzEtNDc4ZS1hYWJiLWY1ZmY0Nzg1YmU5ZSIsImlhdCI6MTU1Njc3NDkxOSwicm9sIjoiYXBpX2FjY2VzcyIsImlkIjoiNDE1MzI5NDUtNTk5ZS00OTEwLTk1OTktMGU3NDAyMDE3ZmJlIiwibmJmIjoxNTU2Nzc0OTE5LCJleHAiOjE1NTY3ODIxMTksImlzcyI6IndlYkFwaSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.xKocIPlwAF2_gT8oyZBOo3i7sXRwQ6ZaALd09f22MJm2LCqZuKLBJnfog_v7P9gu9CDD2YMAmzAU_j8xMSNWog",
                RefreshToken = "cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU="
            }), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.NotNull(result.accessToken.token);
            Assert.Equal(7200, (int)result.accessToken.expiresIn);
            Assert.NotNull(result.refreshToken);
        }

        [Fact]
        public async Task CantExchangeInvalidRefreshToken()
        {
            var httpResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonConvert.SerializeObject(new Models.Request.ExchangeRefreshTokenRequest { AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtbWFjbmVpbCIsImp0aSI6IjA0YjA0N2E4LTViMjMtNDgwNi04M2IyLTg3ODVhYmViM2ZjNyIsImlhdCI6MTUzOTUzNzA4Mywicm9sIjoiYXBpX2FjY2VzcyIsImlkIjoiNDE1MzI5NDUtNTk5ZS00OTEwLTk1OTktMGU3NDAyMDE3ZmJlIiwibmJmIjoxNTM5NTM3MDgyLCJleHAiOjE1Mzk1NDQyODIsImlzcyI6IndlYkFwaSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.xzDQOKzPZarve68Np8Iu8sh2oqoCpHSmp8fMdYRHC_k", RefreshToken = "unknown" }), Encoding.UTF8, "application/json"));
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid token.", stringResponse);
        }
    }
}