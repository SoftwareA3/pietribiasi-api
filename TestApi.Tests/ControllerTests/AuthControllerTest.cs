using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using apiPB.Controllers;

namespace TestApi.Tests.ControllerTests
{
    public class AuthControllerTest
    {
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _controller = new AuthController();

            // Configura un HttpContext di base
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/auth/validate"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Configura l'identità dell'utente
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "43")
            }, "mock"));

            httpContextMock.Setup(c => c.User).Returns(user);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };
        }

        [Fact]
        public void Login_ShouldReturnOkResult_WithUserIdentity()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void Login_ShouldReturnOkResult_WithNullIdentity()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            
            httpRequestMock.Setup(r => r.Method).Returns("GET");
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/auth/validate"));
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Configura un'identità nulla
            var identity = new Mock<System.Security.Principal.IIdentity>();
            identity.Setup(i => i.Name).Returns((string)null);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            httpContextMock.Setup(c => c.User).Returns(user);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = _controller.Login();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}