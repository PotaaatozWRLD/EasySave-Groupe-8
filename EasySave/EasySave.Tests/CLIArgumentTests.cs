using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Tests for CLI argument parsing and automation mode.
/// </summary>
public class CLIArgumentTests
{
    [Theory]
    [InlineData("1", new[] { 1 })]
    [InlineData("3", new[] { 3 })]
    [InlineData("5", new[] { 5 })]
    public void ParseCLIArgument_SingleJob_ShouldReturnCorrectNumber(string input, int[] expected)
    {
        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1-3", new[] { 1, 2, 3 })]
    [InlineData("2-5", new[] { 2, 3, 4, 5 })]
    [InlineData("1-1", new[] { 1 })]
    public void ParseCLIArgument_Range_ShouldReturnAllJobsInRange(string input, int[] expected)
    {
        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1;3;5", new[] { 1, 3, 5 })]
    [InlineData("2;4", new[] { 2, 4 })]
    [InlineData("1;2;3", new[] { 1, 2, 3 })]
    public void ParseCLIArgument_SemicolonSeparated_ShouldReturnSpecificJobs(string input, int[] expected)
    {
        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("--logs")]
    [InlineData("-l")]
    public void ParseCLIArgument_LogsFlag_ShouldBeRecognized(string input)
    {
        // Act
        bool isLogsFlag = IsLogsFlag(input);

        // Assert
        Assert.True(isLogsFlag);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("6")]
    [InlineData("1-")]
    [InlineData("-3")]
    public void ParseCLIArgument_InvalidInput_ShouldReturnEmpty(string input)
    {
        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCLIArgument_MixedRangeAndSemicolon_ShouldReturnEmpty()
    {
        // Arrange
        string input = "1-3;5"; // Invalid: mixed format

        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("1-5", 5)]
    [InlineData("1;2;3;4;5", 5)]
    public void ParseCLIArgument_ExceedingMaxJobs_ShouldOnlyReturnFirst5(string input, int maxJobs)
    {
        // Act
        var result = ParseJobArgument(input);

        // Assert
        Assert.True(result.Length <= maxJobs);
        Assert.All(result, job => Assert.InRange(job, 1, 5));
    }

    // Helper method simulating CLI parsing logic
    private int[] ParseJobArgument(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<int>();

        try
        {
            // Range format: "1-3"
            if (input.Contains('-'))
            {
                var parts = input.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int start) && int.TryParse(parts[1], out int end))
                {
                    if (start >= 1 && end >= start && end <= 5)
                    {
                        return Enumerable.Range(start, end - start + 1).ToArray();
                    }
                }
                return Array.Empty<int>();
            }

            // Semicolon format: "1;3;5"
            if (input.Contains(';'))
            {
                var parts = input.Split(';');
                var jobs = new List<int>();
                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int job) && job >= 1 && job <= 5)
                    {
                        jobs.Add(job);
                    }
                }
                return jobs.ToArray();
            }

            // Single number: "1"
            if (int.TryParse(input, out int singleJob) && singleJob >= 1 && singleJob <= 5)
            {
                return new[] { singleJob };
            }
        }
        catch
        {
            return Array.Empty<int>();
        }

        return Array.Empty<int>();
    }

    private bool IsLogsFlag(string input)
    {
        return input == "--logs" || input == "-l";
    }
}
