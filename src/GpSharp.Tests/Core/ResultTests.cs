using System;
using GpSharp.Core;
using Xunit;
using FluentAssertions;

namespace GpSharp.Tests.Core;

public class ResultTests
{
    [Fact]
    public void Success_ShouldHaveValueAndNoError()
    {
        // Arrange
        var expectedValue = "Success";

        // Act
        var result = Result<string>.Success(expectedValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedValue);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldHaveErrorAndNoValue()
    {
        // Arrange
        var expectedError = new Exception("Failure");

        // Act
        var result = Result<string>.Failure(expectedError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be(expectedError);
    }

    [Fact]
    public void Match_OnSuccess_ShouldReturnExpectedValue()
    {
        // Arrange
        var value = "Success";
        var result = Result<string>.Success(value);

        // Act
    	var output = result.Match(
            success => $"Got {success}",
            failure => throw new Exception("Should not fail")
        );

        // Assert
        output.Should().Be("Got Success");
    }

    [Fact]
    public void Match_OnFailure_ShouldThrowException()
    {
        // Arrange
        var error = new Exception("Error occurred");
        var result = Result<string>.Failure(error);

        // Act
        Action act = () => result.Match(
            success => success,
            failure => throw new InvalidOperationException("Called result.Match with failure.")
        );

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Called result.Match with failure.");
    }
}
