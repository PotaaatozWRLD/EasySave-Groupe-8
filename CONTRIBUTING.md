# Contributing to EasySave

Thank you for your interest in contributing to **EasySave**! This document provides guidelines and standards for contributing to the project.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Coding Standards](#coding-standards)
- [Git Workflow](#git-workflow)
- [Development Process](#development-process)
- [Testing](#testing)
- [Pull Request Process](#pull-request-process)
- [Architecture Guidelines](#architecture-guidelines)

---

## 🤝 Code of Conduct

- Be respectful and professional in all interactions
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Report unacceptable behavior to project maintainers

---

## 🚀 Getting Started

### Prerequisites

- **Visual Studio 2022** or later
- **.NET 10.0 SDK** (or later)
- **Git** for version control
- **Windows OS** (primary target platform)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/PotaaatozWRLD/EasySave-Groupe-8.git
cd EasySave-Groupe-8

# Restore dependencies
dotnet restore EasySave/EasySave.sln

# Build the solution
dotnet build EasySave/EasySave.sln

# Run tests
cd EasySave/EasySave.Tests
dotnet test
```

---

## 📐 Coding Standards

### C# Microsoft Conventions (STRICT)

#### Naming Conventions

```csharp
// Classes, methods, properties: PascalCase
public class BackupService { }
public void ProcessBackup() { }
public string SourcePath { get; set; }

// Variables, parameters: camelCase
int fileCount = 0;
void CopyFile(string sourcePath, string targetPath) { }

// Private fields: _camelCase (underscore prefix)
private readonly ILogger _logger;
private string _currentFilePath;

// Constants: PascalCase or UPPER_SNAKE_CASE
public const int MaxJobCount = 5;
public const string DEFAULT_LOG_PATH = "Logs";
```

#### Code Quality Rules

✅ **REQUIRED:**

- **All code and comments in English** (international team)
- **XML documentation for public APIs**

  ```csharp
  /// <summary>
  /// Performs a full backup of the specified source directory.
  /// </summary>
  /// <param name="sourcePath">The source directory path</param>
  /// <param name="targetPath">The target directory path</param>
  /// <returns>True if backup succeeded, false otherwise</returns>
  public bool ExecuteFullBackup(string sourcePath, string targetPath)
  ```

- **Functions under 50 lines** (ideally, exceptions allowed if justified)
- **No code duplication** (DRY principle - Don't Repeat Yourself)
- **No hardcoded absolute paths** (use `%AppData%` or relative paths)

  ```csharp
  // ❌ BAD
  string logPath = @"C:\temp\logs\";
  
  // ✅ GOOD
  string logPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
      "ProSoft", "EasySave", "Logs");
  ```

❌ **FORBIDDEN:**

- French comments or variable names
- Copy-pasted code blocks (create reusable methods)
- Functions over 100 lines (refactor into smaller methods)
- Ignoring nullable reference warnings

#### Modern C# Features

```csharp
// Use nullable reference types (enabled in project)
public string? GetOptionalValue() => null;

// Use pattern matching
if (result is SuccessResult success) { }

// Use records for DTOs
public record BackupJobDto(string Name, string Source, string Target);

// Use file-scoped namespaces (.NET 6+)
namespace EasySave.Core.Services;

public class BackupService { }
```

---

## 🌿 Git Workflow

### Branch Naming Convention

```bash
feature/<feature-name>    # New features
bugfix/<issue-number>     # Bug fixes
hotfix/<critical-issue>   # Production hotfixes
refactor/<area>           # Code refactoring
docs/<topic>              # Documentation updates

# Examples
git checkout -b feature/encryption-support
git checkout -b bugfix/123-file-lock-issue
git checkout -b hotfix/critical-data-loss
```

### Commit Message Format

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```bash
<type>: <short description>

<optional detailed description>

<optional footer>
```

**Types:**

- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code refactoring
- `docs`: Documentation changes
- `test`: Adding or updating tests
- `chore`: Maintenance tasks
- `perf`: Performance improvements
- `style`: Code style changes (formatting)

**Examples:**

```bash
feat: Add differential backup support

Implemented differential backup to only copy changed files
since last full backup. Reduces backup time by 70%.

Closes #42

---

fix: Handle locked files during backup

Added retry logic with exponential backoff for files
locked by other processes.

Fixes #38
```

### Commit Frequency

- Commit **often** (3-5 times per day minimum)
- Make **atomic commits** (one logical change per commit)
- Keep commits **focused** (don't mix unrelated changes)

---

## 💻 Development Process

### 1. Pick an Issue

- Browse [open issues](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/issues)
- Comment on the issue to claim it
- Ask questions if requirements are unclear

### 2. Create a Branch

```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

### 3. Write Code

- Follow coding standards (see above)
- Write tests for new functionality
- Update documentation as needed
- Run tests frequently

### 4. Test Locally

```bash
# Run all tests
cd EasySave/EasySave.Tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Build entire solution
cd ..
dotnet build EasySave.sln
```

### 5. Commit and Push

```bash
git add .
git commit -m "feat: Add your feature description"
git push origin feature/your-feature-name
```

---

## 🧪 Testing

### Running Tests

```bash
# All tests
cd EasySave/EasySave.Tests
dotnet test

# Specific test
dotnet test --filter "FullyQualifiedName~BackupServiceTests.ShouldCreateBackup"

# With detailed output
dotnet test --logger "console;verbosity=detailed"

# With coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Writing Tests

```csharp
using Xunit;

public class BackupServiceTests
{
    [Fact]
    public void ShouldCreateFullBackup()
    {
        // Arrange
        var service = new BackupService();
        var sourcePath = @"C:\TestSource";
        var targetPath = @"C:\TestTarget";
        
        // Act
        var result = service.ExecuteBackup(sourcePath, targetPath, BackupType.Full);
        
        // Assert
        Assert.True(result.Success);
        Assert.Equal(0, result.ErrorCount);
    }
    
    [Theory]
    [InlineData("test.txt", true)]
    [InlineData("invalid|file.txt", false)]
    public void ShouldValidateFileName(string fileName, bool expected)
    {
        // Test implementation
    }
}
```

### Test Requirements

- ✅ All new features must have tests
- ✅ Bug fixes must include regression tests
- ✅ Maintain >70% code coverage (aim for >80%)
- ✅ Tests must be deterministic (no flaky tests)
- ✅ Use meaningful test names (`ShouldDoSomethingWhenCondition`)

---

## 🔄 Pull Request Process

### Before Submitting

- [ ] All tests pass locally
- [ ] No compiler warnings
- [ ] Code follows style guidelines
- [ ] XML documentation added for public APIs
- [ ] No code duplication introduced
- [ ] Backward compatibility maintained (especially **EasyLog.dll**)

### Creating the PR

1. Push your branch to GitHub
2. Go to [Pull Requests](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/pulls)
3. Click "New Pull Request"
4. Select your branch
5. Fill out the PR template completely
6. Link related issues (`Fixes #123`)
7. Request review from maintainers

### PR Review Process

- Maintainers will review within 2-3 business days
- Address feedback promptly
- Make requested changes in new commits (don't force-push)
- Once approved, maintainers will merge

### After Merge

```bash
# Update your local main branch
git checkout main
git pull origin main

# Delete your feature branch
git branch -d feature/your-feature-name
git push origin --delete feature/your-feature-name
```

---

## 🏗️ Architecture Guidelines

### Project Structure

```
EasySave/
├── EasySave.Console/     # CLI interface (v1.0-1.1)
├── EasySave.GUI/         # Future: WPF/Avalonia interface (v2.0+)
├── EasySave.Core/        # Business logic (shared)
├── EasyLog/              # Logging library (BACKWARD COMPATIBLE)
├── EasySave.Shared/      # Common models and utilities
└── EasySave.Tests/       # Unit and integration tests
```

### Design Patterns Used

- **Singleton**: `LanguageManager` for global language access
- **Repository**: Data access abstraction
- **Factory**: Logger creation (JSON/XML)
- **Strategy**: Backup types (Full/Differential)
- **MVVM**: Future GUI architecture (v2.0+)

### Critical Rules

#### EasyLog.dll - Backward Compatibility 🔒

**CRITICAL**: This library must maintain **absolute backward compatibility** between all versions.

✅ **ALLOWED:**

- Add new methods
- Add optional properties with default values
- Add new overloads

❌ **FORBIDDEN:**

- Modify existing method signatures
- Remove methods or properties
- Change serialization formats (JSON/XML structure)
- Change namespace or assembly name

```csharp
// ✅ GOOD: Adding new optional property
public class LogEntry
{
    public string FileName { get; set; }
    public long FileSize { get; set; }
    
    // New in v2.0 - SAFE because optional
    public long? EncryptionTime { get; set; } = null;
}

// ❌ BAD: Changing existing property
public class LogEntry
{
    // BREAKING CHANGE - Don't do this!
    public DateTime Timestamp { get; set; } // Was string before
}
```

### File Handling

```csharp
// Always use standard Windows paths
string appDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);
string configPath = Path.Combine(appDataPath, "ProSoft", "EasySave");

// Support UNC paths
bool isUncPath = path.StartsWith(@"\\");

// Always validate paths
if (!Directory.Exists(sourcePath))
    throw new DirectoryNotFoundException($"Source not found: {sourcePath}");
```

### Internationalization

```csharp
// Use LanguageManager for all user-facing text
string message = LanguageManager.Instance.GetString("error.file_not_found");

// Always provide English fallback
string text = GetString("key") ?? "[MISSING_KEY]";

// Language files: Languages/lang.{culture}.json
// Keys use dot notation: "menu.create_job", "error.invalid_path"
```

---

## 📚 Additional Resources

- [C# Coding Conventions (Microsoft)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [.NET API Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [xUnit Documentation](https://xunit.net/docs/getting-started/netcore/cmdline)
- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)

---

## ❓ Questions?

- Open a [Discussion](https://github.com/PotaaatozWRLD/EasySave-Groupe-8/discussions)
- Join our weekly meetings (check Discussions for schedule)
- Contact maintainers via issues

---

**Thank you for contributing to EasySave! 🚀**
