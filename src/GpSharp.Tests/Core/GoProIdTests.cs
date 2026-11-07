using System;
using GpSharp.Core;
using Xunit;
using FluentAssertions;

namespace GpSharp.Tests.Core;

public class GoProIdTests
{
    [Fact]
    public void Constructor_WithValidFourDigits_ShouldSucceed()
    {
        // Arrange
        var validId = "1234";

        // Act
        var id = new GoProId(validId);

        // Assert
        id.PartialSerial.Should().Be(validId);
        id.ToString().Should().Be(validId);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public void Constructor_WithInvalidId_ShouldThrowArgumentException(string? invalidId)
    {
    	// Act
    	Action act = () => new GoProId(invalidId!);

    	// Assert
    	act.Should().Throw<ArgumentException>()
           .WithMessage("GoProId must be exactly 4 characters. (Parameter 'value')");
    }

    [Fact]
    public void ImplicitConversionToString_ShouldWork()
    {
        // Arrange
        var id = new GoProId("9876");

        // Act
        string value = id;

        // Assert
        value.Should().Be("9876");
    }

    [Fact]
    public void Equality_ShouldBeBasedOnPartialSerial()
    {
        var first = new GoProId("1234");
        var second = new GoProId("1234");

        first.Should().Be(second);
    }
}
