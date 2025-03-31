using System.Reflection;
using GameOfLife.Presentation.ErrorHandling;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.UnitTest.Presentation.ErrorHandling;

public class ExceptionHandlerTest
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TryHandleAsync_WithArgumentOutOfRangeExceptionFromSameAssembly_ReturnsTrueAndWritesBadRequest(bool isSameAssembly)
    {
        // Arrange
        var exceptionHandler = new ExceptionHandler();
        var httpContextMock = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        var cancellationToken = CancellationToken.None;

        var exception = new ArgumentOutOfRangeException(nameof(isSameAssembly), "Test message")
        {
            Source = isSameAssembly ? Assembly.GetEntryAssembly()?.GetName().Name : "DifferentAssembly"
        };

        var expectedStatusCode = isSameAssembly ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK; // 200 in mock scenario only

        // Act
        var result = await exceptionHandler.TryHandleAsync(httpContextMock, exception, cancellationToken);

        // Assert
        result.Should().Be(isSameAssembly);
        httpContextMock.Response.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WithOtherException_ReturnsFalse()
    {
        // Arrange
        var exceptionHandler = new ExceptionHandler();
        var httpContextMock = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        var cancellationToken = CancellationToken.None;

        var exception = new InvalidOperationException("Test message");

        // Act
        var result = await exceptionHandler.TryHandleAsync(httpContextMock, exception, cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
