using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Options;
using CardholderApi.Services;
using CardholderApi.Models;
using System.IdentityModel.Tokens.Jwt;

namespace CardholderTests
{
    [TestClass]
    public class AuthServiceTests
    {
        private AuthService _authService;

        [TestInitialize]
        public void Setup()
        {
            // Arrange - dummy JwtSettings
            var jwtSettings = new JwtSettings
            {
                SecretKey = "SuperSecretKeyForTesting1234567890!",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            var options = Options.Create(jwtSettings);

            _authService = new AuthService(options);
        }

        [TestMethod]
        public void GenerateJwtToken_ShouldReturnValidToken()
        {
            var token = _authService.GenerateJwtToken();

            Assert.IsFalse(string.IsNullOrEmpty(token), "Token should not be null or empty");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.AreEqual("TestIssuer", jwtToken.Issuer);
            Assert.AreEqual("TestAudience", jwtToken.Audiences.FirstOrDefault());
        }
    }
}
