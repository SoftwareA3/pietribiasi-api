using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using apiPB.Controllers; 
using apiPB.Dto.Models;  
using apiPB.Services.Abstraction; 
using apiPB.Services; 
using Microsoft.IdentityModel.Tokens; 
using apiPB.Utils.Abstraction;

namespace TestApi.Tests.ControllerTests 
{
    public class JobControllerTest 
    {
        private readonly Mock<IJobRequestService> _jobRequestServiceMock;
        private readonly Mock<IResponseHandler> _responseHandlerMock;
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
            _responseHandlerMock = new Mock<IResponseHandler>();

            _controller = new JobController(_responseHandlerMock.Object, _jobRequestServiceMock.Object);

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

            // Setup generico per i metodi di log (_isLogActive è false nel controller)
            _responseHandlerMock.Setup(x => x.HandleBadRequest(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new BadRequestObjectResult("La richiesta non può essere vuota."));
            _responseHandlerMock.Setup(x => x.HandleNotFound(It.IsAny<HttpContext>(), It.IsAny<bool>()))
                .Returns(new NotFoundObjectResult("Non risultato trovato."));
        }

        [Fact]
        public void GetVwApiJobs_ShouldReturnOkResult_WhenJobsExist()
        {
            // Arrange

            var mockDataList = new List<JobDto> { _sampleJobDto, new JobDto { Job = "JOB002", Description = "Another Job" } };
            _jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(mockDataList);
            _responseHandlerMock.Setup(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<JobDto>>(), 
                false)).Returns(new OkObjectResult(mockDataList));

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<JobDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Contains(_sampleJobDto, returnValue); 
            Assert.Equal("JOB001", returnValue.First(j => j.Job == "JOB001").Job);

            _responseHandlerMock.Verify(log => log.HandleOkAndList(
                It.IsAny<HttpContext>(), 
                It.IsAny<List<JobDto>>(), 
                false), Times.Once);
        }

        [Fact]
        public void GetVwApiJobs_ShouldReturnNotFound_WhenServiceReturnsEmptyList()
        {
            // Arrange
            _jobRequestServiceMock.Setup(service => service.GetJobs()).Returns(new List<JobDto>()); 

            // Act
            var result = _controller.GetVwApiJobs();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _responseHandlerMock.Verify(log => log.HandleNotFound(
                It.IsAny<HttpContext>(), 
                false), Times.Once);
        }
    }
}