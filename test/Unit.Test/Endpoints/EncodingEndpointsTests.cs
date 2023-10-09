using Host.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Unit.Test.endpoints;

public class EncodingEndpointsTests
{
    [Fact]
    public void StartJob_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var encodingServiceMock = new Mock<IEncodingService>();

        encodingServiceMock.Setup(service => service.StartEncodingAsync(It.IsAny<string>()))
            .Returns(20);

        var cancellationToken = CancellationToken.None;
        var inputText = "Hello, World!";
        var expectedResult = 20;

        // Act
        var result = Host.Endpoints.Encoding.StartJob(encodingServiceMock.Object, inputText, cancellationToken);

        // Assert
        var okResult = Assert.IsType<Ok<int>>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public void StartJob_CancellationRequested_ReturnsBadRequestResult()
    {
        // Arrange
        var encodingServiceMock = new Mock<IEncodingService>();
        var cancellationToken = new CancellationToken(canceled: true);

        // Act
        var result = Host.Endpoints.Encoding.StartJob(encodingServiceMock.Object, "inputText", cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Processing canceled", badRequestResult.Value);
    }


    [Fact]
    public void StartJob_EmptyInput_ReturnsBadRequestResult()
    {
        // Arrange
        var encodingServiceMock = new Mock<IEncodingService>();
        var cancellationToken = CancellationToken.None;
        var inputText = string.Empty;

        // Act
        var result = Host.Endpoints.Encoding.StartJob(encodingServiceMock.Object, inputText, cancellationToken);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Input text is required.", badRequestResult.Value);
    }

    [Fact]
    public void StopJob_ReturnsOkResult()
    {
        // Arrange
        var encodingServiceMock = new Mock<IEncodingService>();
        var expectedResult = "Encoding canceled";
        encodingServiceMock.Setup(service => service.StopEncodingAsync())
            .Returns(expectedResult);

        // Act
        var result = Host.Endpoints.Encoding.StopJob(encodingServiceMock.Object, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<Ok<string>>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }
}
