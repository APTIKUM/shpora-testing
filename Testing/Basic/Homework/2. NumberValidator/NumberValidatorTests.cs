using NUnit.Framework;
using FluentAssertions;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1)]
    [TestCase(0)]
    public void NumberValidator_ThrowsArgumentException_ForNonPositivePrecision(int precision)
    {
        var action = () => new NumberValidator(precision, 0);

        action.Should().Throw<ArgumentException>($"precision must be positive");
    }

    [Test]
    public void NumberValidator_ThrowsArgumentException_ForNegativeScale()
    {
        var action = () => new NumberValidator(10, -1);

        action.Should().Throw<ArgumentException>("scale must be non-negative");
    }

    [TestCase(1)]
    [TestCase(2)]
    public void NumberValidator_ThrowsArgumentException_ForScaleMoreOrEqualThanPrecision(int scale)
    {
        var action = () => new NumberValidator(1, scale);

        action.Should().Throw<ArgumentException>($"scale must be less than precision");
    }

    [TestCase(2, 0)]
    [TestCase(2, 1)]
    [TestCase(10, 5)]
    public void NumberValidator_DoesNotThrow_WithValidPrecisionAndScale(int precision, int scale)
    {
        var action = () => new NumberValidator(precision, scale);

        action.Should().NotThrow($"precision={precision} and scale={scale} are valid");
    }

    [TestCase("1234", true)]
    [TestCase("12345", true)]
    [TestCase("+1234",  true)]
    [TestCase("-1234",  true)]
    [TestCase("123456",  false)]
    [TestCase("+12345",  false)]
    [TestCase("-12345",  false)]
    [TestCase("123.45",  true)]
    [TestCase("1234.56",  false)]
    public void IsValidNumber_ReturnsExpected_ForPrecision(string number, bool expected)
    {
        const int precision = 5;
        const int scale = 2;

        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(number).Should().Be(expected,
            $"number={number}, precision={precision}, scale={scale}");
    }

    [TestCase("1", true)]
    [TestCase("+1", false)]
    [TestCase("-1", false)]
    [TestCase("12", false)]
    public void IsValidNumber_ReturnsExpected_ForMinimumPrecision(string number, bool expected)
    {
        const int precision = 1;
        const int scale = 0;

        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(number).Should().Be(expected,
            $"number={number}, precision={precision}, scale={scale}");
    }

    [TestCase("00", true)]
    [TestCase("00123", true)]
    [TestCase("000123", false)]
    [TestCase("00.12", true)]
    [TestCase("0000.12", false)]
    public void IsValidNumber_ReturnsExpected_ForLeadingZeros(string number, bool expected)
    {
        const int precision = 5;
        const int scale = 2;

        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(number).Should().Be(expected,
            $"number={number}, precision={precision}, scale={scale}");
    }

    [TestCase("123", 0, true)]
    [TestCase("123.0", 0, false)]
    [TestCase("123.4", 1, true)]
    [TestCase("123.45", 1, false)]
    [TestCase("123.45", 2, true)]
    [TestCase("123.45", 3, true)]
    public void IsValidNumber_ReturnsExpected_ForScale(
        string number, int scale, bool expected)
    {
        const int precision = 10;

        var validator = new NumberValidator(precision, scale);

        validator.IsValidNumber(number).Should().Be(expected,
            $"number={number}, precision={precision}, scale={scale}");
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("abc")]
    [TestCase("12.a")]
    [TestCase("1.2.3")]
    [TestCase("1,2,3")]
    [TestCase("1.2,3")]
    [TestCase("+-123")]
    [TestCase("++123")]
    [TestCase("--123")]
    [TestCase(".1")]
    [TestCase("1.")]
    [TestCase("12 ")]
    [TestCase(" 12")]
    [TestCase("12\n1")]
    [TestCase("\n1")]
    public void IsValidNumber_ReturnsFalse_ForInvalidFormat(string? number)
    {
        var validator = new NumberValidator(10, 2);

        validator.IsValidNumber(number).Should().BeFalse(
            $"number='{number}' is invalid format");
    }

    [TestCase("0", true, true)]
    [TestCase("+0", true, true)]
    [TestCase("-0", true, false)]
    [TestCase("123", true, true)]
    [TestCase("+123", true, true)]
    [TestCase("-123", true, false)]
    [TestCase("12.34", true, true)]
    [TestCase("-12.34", true, false)]
    [TestCase("-0", false, true)]
    [TestCase("-123", false, true)]
    [TestCase("-12.34", false, true)]
    public void IsValidNumber_ReturnsExpected_ForOnlyPositive(
        string number, bool onlyPositive, bool expected)
    {
        var validator = new NumberValidator(10, 2, onlyPositive);

        validator.IsValidNumber(number).Should().Be(expected,
            $"number={number} must be invalid for onlyPositive={onlyPositive}");
    }

    [TestCase("12,34")]
    [TestCase("+12,34")]
    [TestCase("-12,34")]
    [TestCase("0,5")]
    public void IsValidNumber_ReturnsTrue_ForDecimalComma(string number)
    {
        var validator = new NumberValidator(10, 2);

        validator.IsValidNumber(number).Should().BeTrue(
            $"decimal comma is allowed");
    }
}
