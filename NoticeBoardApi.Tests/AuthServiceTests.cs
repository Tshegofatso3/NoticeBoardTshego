using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoticeBoardApi.Data;
using NoticeBoardApi.Models;
using NoticeBoardApi.Services;
using Xunit;

namespace NoticeBoardApi.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }
        private IConfiguration CreateConfig()
        {
            var settings = new Dictionary<string, string?>
            {
                { "Jwt:Key",           "TestSecretKeyMin32CharactersLong!!" },
                { "Jwt:Issuer",        "TestIssuer" },
                { "Jwt:Audience",      "TestAudience" },
                { "Jwt:ExpiryMinutes", "60" }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public void Login_ReturnsNull_WhenUserDoesNotExist()
        {
            var context = CreateContext();
            var service = new AuthService(context, CreateConfig());

            var token = service.Login("nobody", "password");

            Assert.Null(token);
        }

        [Fact]
        public void Login_ReturnsNull_WhenPasswordIsWrong()
        {
            var context = CreateContext();
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
                Role = "Admin"
            });
            context.SaveChanges();
            var service = new AuthService(context, CreateConfig());

            var token = service.Login("admin", "wrong");

            Assert.Null(token);
        }

        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var context = CreateContext();
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role ="Admin"
            });
            context.SaveChanges();
            var service = new AuthService(context, CreateConfig());

            var token = service.Login("admin", "Admin@123");

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }
    }
}
