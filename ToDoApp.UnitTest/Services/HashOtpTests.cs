using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Text.RegularExpressions;
using ToDoApp.Application.Services.OTP;
using Xunit;
namespace ToDoApp.UnitTest.Services;

public class HashOtpTests
{
    private readonly HashOtp _hashOtp;

    public HashOtpTests()
    {
        _hashOtp = new HashOtp();
    }

    [Fact]
    public void GenerateOtp_ShouldReturn6DigitNumber()
    {
        // Act
        var otp = _hashOtp.GenerateOtp();

        // Assert
        otp.Length.Should().Be(6);
        otp.Should().MatchRegex("^[0-9]{6}$");
    }

    [Fact]
    public void GenerateOtp_ShouldGenerateDifferentValues()
    {
        var otp1 = _hashOtp.GenerateOtp();
        var otp2 = _hashOtp.GenerateOtp();

        otp1.Should().NotBe(otp2);
    }

    [Fact]
    public void Hashotp_ShouldReturnSameHashForSameInput()
    {
        // Arrange
        var otp = "123456";

        // Act
        var hash1 = _hashOtp.Hashotp(otp);
        var hash2 = _hashOtp.Hashotp(otp);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void Hashotp_ShouldReturnDifferentHashForDifferentInput()
    {
        var hash1 = _hashOtp.Hashotp("123456");
        var hash2 = _hashOtp.Hashotp("654321");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Hashotp_ShouldReturnBase64String()
    {
        var otp = "123456";
        var hash = _hashOtp.Hashotp(otp);

        // Base64 regex validation
        hash.Should().MatchRegex(@"^[A-Za-z0-9+/=]+$");
    }
}

