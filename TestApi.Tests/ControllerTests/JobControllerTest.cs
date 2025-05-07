using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers; 
using apiPB.Dto.Models;  
using apiPB.Services.Request.Abstraction; 
using apiPB.Services; 
using Microsoft.IdentityModel.Tokens; 
using apiPB.Services.Utils.Abstraction;

namespace TestApi.Tests.ControllerTests 
{
    public class JobControllerTest 
    {
        private readonly Mock<IJobRequestService> _jobRequestServiceMock;
        private readonly Mock<ILogService> _logServiceMock;
        private readonly JobController _controller;

        // DTO utilizzati per i test
        private readonly JobDto _sampleJobDto = new JobDto
        {
            Job = "JOB001",
            Description = "Sample Job Description"
        };

        public JobControllerTest() 
        {
            _jobRequestServiceMock = new Mock<IJobRequestService>();
            _logServiceMock = new Mock<ILogService>();

            _controller = new JobController(_logServiceMock.Object, _jobRequestServiceMock.Object);

            // Mock HttpContext per requestPath e User
            var httpContextMock = new Mock<HttpContext>();
            var httpRequestMock = new Mock<HttpRequest>();
            
            // Impostazioni predefinite
            httpRequestMock.Setup(r => r.Method).Returns("GET"); 
            httpRequestMock.Setup(r => r.Path).Returns(new PathString("/api/job")); 
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            // Mock User per [Authorize] (utente generico autenticato)
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextMock.Setup(c => c.User).Returns(claimsPrincipal);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContextMock.Object
            };

            // Setup generico per i metodi di log (_logIsActive è false nel controller)
            _logServiceMock.Setup(x => x.AppendMessageToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                false)); // _logIsActive false

            _logServiceMock.Setup(x => x.AppendMessageAndListToLog(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<List<JobDto>>(), 
                false)); // _logIsActive false
        }

        private void MockRequestPath(string method, string path)
        {
            var httpRequestMock = Mock.Get(_controller.HttpContext.Request);
            httpRequestMock.Setup(r => r.Method).Returns(method);
            httpRequestMock.Setup(r => r.Path).Returns(new PathString(path));
        }

        [Fact]
        public void GetVwApiJobs_ShouldReturnOkResult_WhenJobsExist()
        {
            // Arrange
            var expectedPath = "GET api/job"; 
            MockRequestPath("GET", "/api/job");

            var mockDataList = new List<JobDto> { _sampleJobDto, new JobDto { Job = "JOB002", Description = "Another Job" } };
            _jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(mockDataList);

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<JobDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Contains(_sampleJobDto, returnValue); 
            Assert.Equal("JOB001", returnValue.First(j => j.Job == "JOB001").Job);


            _logServiceMock.Verify(log => log.AppendMessageAndListToLog(
                expectedPath, 
                200, 
                "OK", 
                It.Is<List<JobDto>>(list => list.Count == 2 && list.Contains(_sampleJobDto)), 
                false), Times.Once);
        }

        [Fact]
        public void GetVwApiJobs_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            var expectedPath = "GET api/job";
            MockRequestPath("GET", "/api/job");

            var emptyList = new List<JobDto>();
            _jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(emptyList);

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _logServiceMock.Verify(log => log.AppendMessageToLog(
                expectedPath, 
                404, 
                "Not Found", 
                false), Times.Once);
        }

        [Fact]
        public void GetVwApiJobs_ShouldReturnNotFound_WhenServiceReturnsNull()
        {
            // Arrange
            // Come discusso per GiacenzeController, se il servizio restituisce null e il controller
            // chiama .ToList() su questo null, si verificherebbe una NullReferenceException.
            // Il mock del servizio dovrebbe restituire un IEnumerable vuoto per simulare "nessun dato"
            // in modo che il .ToList() del controller non fallisca e il check IsNullOrEmpty() funzioni.
            var expectedPath = "GET api/job";
            MockRequestPath("GET", "/api/job");
            
            //_jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(Enumerable.Empty<JobDto>());
            _jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(new List<JobDto>()); // Lista vuota che IsNullOrEmpty gestisce

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _logServiceMock.Verify(log => log.AppendMessageToLog(
                expectedPath,
                404,
                "Not Found",
                false), Times.Once);
        }
    }
}