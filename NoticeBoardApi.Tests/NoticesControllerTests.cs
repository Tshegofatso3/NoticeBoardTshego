using com.sun.source.util;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NoticeBoardApi.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace NoticeBoardApi.Tests
{
    public class NoticesControllerTests : IClassFixture<NoticeBoardApiFactory>
    {
        private readonly HttpClient _client;

        public NoticesControllerTests(NoticeBoardApiFactory factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAll_ReturnsOk_WithoutAuthentication()
        {
            // Act 
            var response = await _client.GetAsync("/api/notices");

            // Assert 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNoticeDoesNotExist()
        {
            var response = await _client.GetAsync("/api/notices/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Fact]
        public async Task Post_ReturnsUnauthorized_WithoutToken()
        {
            var notice = new { Title = "Test", Body = "Body" };
            var response = await _client.PostAsJsonAsync("/api/notices", notice);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        private string GenerateTestToken(string username = "admin", string role ="Admin")
        {
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("This-Is-A-Dev-Secret-Replace-In-Production-Min32Chars!")); 
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name,username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role,role),
            };
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: "NoticeBoardApi", audience: "NoticeBoardApiUsers",
                claims: claims, expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task Post_ReturnsCreated_WhenAuthenticatedAsAdmin()
        {
            // Arrange 
            var token = GenerateTestToken("admin", "Admin");
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var notice = new { Title = "Auth Test", Body = "Body text" };

            // Act 
            var response = await _client.PostAsJsonAsync("/api/notices", notice);

            // Assert 
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Clean up auth header for other tests 
            _client.DefaultRequestHeaders.Authorization = null;
        }
        [Fact]
        public async Task Delete_ReturnsForbidden_WhenAuthenticatedAsViewer()
        {
            // Arrange: create a notice first using admin token 
            var adminToken = GenerateTestToken("admin", "Admin");
            var viewerToken = GenerateTestToken("viewer", "Viewer");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",adminToken);
            var created = await _client.PostAsJsonAsync("/api/notices",
                     new { Title = "To Delete", Body = "Body" });
            var notice = await created.Content.ReadFromJsonAsync<NoticeBoardApi.Models.Notice>();

            // Act: try to delete as Viewer 
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",viewerToken);
            var response = await _client.DeleteAsync($"/api/notices/{notice!.Id}");

            // Assert 
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            _client.DefaultRequestHeaders.Authorization = null;
        }
    }
}












