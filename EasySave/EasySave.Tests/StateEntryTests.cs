using EasyLog;
using Xunit;

namespace EasySave.Tests;

/// <summary>
/// Unit tests for StateEntry class.
/// Tests all required fields for real-time state tracking.
/// </summary>
public class StateEntryTests
{
    [Fact]
    public void StateEntry_ShouldInitializeWithAllRequiredFields()
    {
        // Arrange & Act
        var state = new StateEntry
        {
            Name = "Backup Job 1",
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            State = JobState.ACTIVE,
            TotalFiles = 100,
            TotalSize = 1024000,
            Progression = 50,
            NbFilesLeftToDo = 50,
            NbFilesLeftToDoSize = 512000,
            CurrentSourceFilePath = "C:\\source\\file.txt",
            CurrentTargetFilePath = "D:\\target\\file.txt"
        };

        // Assert
        Assert.Equal("Backup Job 1", state.Name);
        Assert.Equal(JobState.ACTIVE, state.State);
        Assert.Equal(100, state.TotalFiles);
        Assert.Equal(1024000, state.TotalSize);
        Assert.Equal(50, state.Progression);
        Assert.Equal(50, state.NbFilesLeftToDo);
        Assert.Equal(512000, state.NbFilesLeftToDoSize);
        Assert.NotNull(state.CurrentSourceFilePath);
        Assert.NotNull(state.CurrentTargetFilePath);
    }

    [Fact]
    public void StateEntry_ShouldSupportEndState()
    {
        // Arrange & Act
        var state = new StateEntry
        {
            Name = "Completed Job",
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            State = JobState.END,
            TotalFiles = 150,
            TotalSize = 2048000,
            Progression = 100,
            NbFilesLeftToDo = 0,
            NbFilesLeftToDoSize = 0,
            CurrentSourceFilePath = string.Empty,
            CurrentTargetFilePath = string.Empty
        };

        // Assert
        Assert.Equal(JobState.END, state.State);
        Assert.Equal(100, state.Progression);
        Assert.Equal(0, state.NbFilesLeftToDo);
        Assert.Equal(0, state.NbFilesLeftToDoSize);
    }

    [Fact]
    public void StateEntry_ProgressionShouldBeAccurate()
    {
        // Arrange
        var state = new StateEntry
        {
            Name = "Progress Test",
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            State = JobState.ACTIVE,
            TotalFiles = 200,
            TotalSize = 4096000,
            Progression = 75,
            NbFilesLeftToDo = 50,
            NbFilesLeftToDoSize = 1024000
        };

        // Act & Assert
        int expectedCompletedFiles = state.TotalFiles - state.NbFilesLeftToDo;
        Assert.Equal(150, expectedCompletedFiles);
        Assert.Equal(75, state.Progression);
        Assert.True(state.NbFilesLeftToDo < state.TotalFiles);
    }

    [Fact]
    public void StateEntry_ShouldSupportPausedState()
    {
        // Arrange & Act
        var state = new StateEntry
        {
            Name = "Paused Job",
            LastActionTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            State = JobState.PAUSED,
            TotalFiles = 100,
            TotalSize = 1024000,
            Progression = 33,
            NbFilesLeftToDo = 67,
            NbFilesLeftToDoSize = 686080,
            CurrentSourceFilePath = "C:\\source\\paused-file.txt",
            CurrentTargetFilePath = "D:\\target\\paused-file.txt"
        };

        // Assert
        Assert.Equal(JobState.PAUSED, state.State);
        Assert.True(state.Progression < 100);
        Assert.True(state.NbFilesLeftToDo > 0);
    }
}
