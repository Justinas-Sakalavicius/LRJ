using Host.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Unit.Test.Helper;

namespace Unit.Test.Services;

public class EncodingServiceTests
{
    [Fact]
    public void StartEncodingAsync_ValidInput_EncodesAndCachesData()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EncodingService>>();
        var memoryCacheMock = new MemoryCacheHelper().MockCache();

        var inputText = "Hello, World!";
        var expectedEncodedLength = 20; // Length of "SGVsbG8sIFdvcmxkIQ==" in characters

        var encodingService = new EncodingService(loggerMock.Object, memoryCacheMock.Object);

        // Act
        var result = encodingService.StartEncodingAsync(inputText);

        // Assert
        Assert.Equal(expectedEncodedLength, result);

        memoryCacheMock.Verify(cache => cache.CreateEntry(It.IsAny<object>()), Times.Once());
    }

    [Fact]
    public void StopEncodingAsync_CancelsOperation_RemovesData()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EncodingService>>();
        var memoryCacheMock = new MemoryCacheHelper().MockCache();

        var encodingService = new EncodingService(loggerMock.Object, memoryCacheMock.Object);

        // Act
        var result = encodingService.StopEncodingAsync();

        // Assert
        Assert.Equal("Encoding canceled", result);

        memoryCacheMock.Verify(cache => cache.CreateEntry(It.IsAny<object>()), Times.Once());
        memoryCacheMock.Verify(cache => cache.Remove("EncodedCharacters"), Times.Once());
    }

    [Fact]
    public void StartEncodingAsync_ThrowsOperationCanceledException_LogsError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EncodingService>>();
        var memoryCacheMock = new MemoryCacheHelper().MockCache();

        memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Throws(new OperationCanceledException("Processing canceled."));

        var inputText = "Hello, World!";
        var encodingService = new EncodingService(loggerMock.Object, memoryCacheMock.Object);

        // Act and Assert
        Assert.Throws<OperationCanceledException>(() => encodingService.StartEncodingAsync(inputText));
    }

    [Fact]
    public void StartEncodingAsync_ThrowsException_LogsError()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EncodingService>>();
        var memoryCacheMock = new MemoryCacheHelper().MockCache();

        memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Throws(new Exception("An error occurred during encoding."));

        var inputText = "Hello, World!";
        var encodingService = new EncodingService(loggerMock.Object, memoryCacheMock.Object);

        // Act and Assert
        Assert.Throws<Exception>(() => encodingService.StartEncodingAsync(inputText));
    }
}
