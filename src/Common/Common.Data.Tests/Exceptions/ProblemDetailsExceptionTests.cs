namespace ForexMiner.Heimdallr.Common.Data.Tests.Exceptions
{
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;
    using Xunit;

    public class ProblemDetailsExceptionTests
    {
        [Fact]
        public void Constructor_Empty()
        {
            // Arrange
            var defaultStatusCode = HttpStatusCode.InternalServerError;

            // Act
            var problemDetails = new ProblemDetailsException();

            // Assert
            Assert.Equal(defaultStatusCode, problemDetails.Status);
        }

        [Fact]
        public void Constructor_Status()
        {
            // Arrange
            var statusCode = HttpStatusCode.BadRequest;

            // Act
            var problemDetails = new ProblemDetailsException(statusCode);

            // Assert
            Assert.Equal(statusCode, problemDetails.Status);
        }

        [Fact]
        public void Constructor_StatusAndMessage()
        {
            // Arrange
            var statusCode = HttpStatusCode.BadRequest;
            var message = "Unit testing error thrown";

            // Act
            var problemDetails = new ProblemDetailsException(statusCode, message);

            // Assert
            Assert.Equal(statusCode, problemDetails.Status);
            Assert.Equal(message, problemDetails.Message);
        }
    }
}
