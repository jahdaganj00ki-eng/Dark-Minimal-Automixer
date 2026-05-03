# AutoDJMix Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Create a first compiling .NET 8 solution with the full project structure, core Domain models (incl. MixEngine score + hard overrides), and SQLite persistence with first migration for the library table.

**Architecture:** Clean Architecture + MVVM. Domain is pure. Persistence is an adapter implementing repositories. MixEngine hard rules are represented as immutable Domain rule objects loaded from `mixEngine.json` (file shipped with app resources later).

**Tech Stack:** .NET 8, WPF, xUnit, EF Core (SQLite), Microsoft.Data.Sqlite, System.Text.Json.

---

## 0. File/Project Map (new)

**Solution root**
- Create: `/workspace/AutoDJMix.sln`
- Keep: `/workspace/mixEngine.json`

**Production projects**
- Create: `/workspace/src/AutoDJMix.App/AutoDJMix.App.csproj` (WPF .NET 8)
- Create: `/workspace/src/AutoDJMix.Application/AutoDJMix.Application.csproj`
- Create: `/workspace/src/AutoDJMix.Domain/AutoDJMix.Domain.csproj`
- Create: `/workspace/src/AutoDJMix.Analysis/AutoDJMix.Analysis.csproj`
- Create: `/workspace/src/AutoDJMix.Mixing/AutoDJMix.Mixing.csproj`
- Create: `/workspace/src/AutoDJMix.Transition/AutoDJMix.Transition.csproj`
- Create: `/workspace/src/AutoDJMix.Rendering/AutoDJMix.Rendering.csproj`
- Create: `/workspace/src/AutoDJMix.Persistence/AutoDJMix.Persistence.csproj`
- Create: `/workspace/src/AutoDJMix.Infrastructure/AutoDJMix.Infrastructure.csproj`
- Create: `/workspace/src/AutoDJMix.Migrator/AutoDJMix.Migrator.csproj` (console tool for EF migrations; runs on non-Windows)

**Tests**
- Create: `/workspace/tests/AutoDJMix.Domain.Tests/AutoDJMix.Domain.Tests.csproj`
- Create: `/workspace/tests/AutoDJMix.Persistence.Tests/AutoDJMix.Persistence.Tests.csproj`

**Build/Installer placeholders (folders only for now)**
- Create: `/workspace/build/`
- Create: `/workspace/installer/nsis/`

## Task 1: Create solution + project skeleton (no business logic yet)

**Files:**
- Create: `src/*/*.csproj` (as listed)
- Create: `tests/*/*.csproj` (as listed)
- Create: `Directory.Build.props`
- Create: `Directory.Packages.props`

- [ ] **Step 1: Create directories**

Run:
```bash
mkdir -p src/AutoDJMix.App src/AutoDJMix.Application src/AutoDJMix.Domain src/AutoDJMix.Analysis src/AutoDJMix.Mixing src/AutoDJMix.Transition src/AutoDJMix.Rendering src/AutoDJMix.Persistence src/AutoDJMix.Infrastructure src/AutoDJMix.Migrator
mkdir -p tests/AutoDJMix.Domain.Tests tests/AutoDJMix.Persistence.Tests
mkdir -p build installer/nsis
```

- [ ] **Step 2: Create `Directory.Packages.props`**

Create: `Directory.Packages.props`
```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.10.0" />
    <PackageVersion Include="xunit" Version="2.9.0" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageVersion Include="coverlet.collector" Version="6.0.2" />

    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="8.0.5" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.5" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.5" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Create `Directory.Build.props`**

Create: `Directory.Build.props`
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
</Project>
```

- [ ] **Step 4: Create `AutoDJMix.Domain.csproj`**

Create: `src/AutoDJMix.Domain/AutoDJMix.Domain.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>AutoDJMix.Domain</AssemblyName>
  </PropertyGroup>
</Project>
```

- [ ] **Step 5: Create `AutoDJMix.Application.csproj`**

Create: `src/AutoDJMix.Application/AutoDJMix.Application.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 6: Create other class library csprojs**

Create each file similarly with Domain reference where needed:

`src/AutoDJMix.Analysis/AutoDJMix.Analysis.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

`src/AutoDJMix.Mixing/AutoDJMix.Mixing.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

`src/AutoDJMix.Transition/AutoDJMix.Transition.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

`src/AutoDJMix.Rendering/AutoDJMix.Rendering.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

`src/AutoDJMix.Persistence/AutoDJMix.Persistence.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
  </ItemGroup>
</Project>
```

`src/AutoDJMix.Infrastructure/AutoDJMix.Infrastructure.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Application\AutoDJMix.Application.csproj" />
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 7: Create `AutoDJMix.Migrator` (console)**

Create: `src/AutoDJMix.Migrator/AutoDJMix.Migrator.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <AssemblyName>AutoDJMix.Migrator</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Persistence\AutoDJMix.Persistence.csproj" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

Create: `src/AutoDJMix.Migrator/Program.cs`
```csharp
using AutoDJMix.Persistence;
using Microsoft.EntityFrameworkCore;

var dbPath = args.Length > 0 ? args[0] : "autodjmix.db";

var options = new DbContextOptionsBuilder<AutoDjMixDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var db = new AutoDjMixDbContext(options);
db.Database.Migrate();
```

- [ ] **Step 7: Create WPF app project**

Create: `src/AutoDJMix.App/AutoDJMix.App.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <UseWPF>true</UseWPF>
    <TargetFramework>net8.0-windows</TargetFramework>
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
    <AssemblyName>AutoDJMix.App</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Application\AutoDJMix.Application.csproj" />
    <ProjectReference Include="..\AutoDJMix.Infrastructure\AutoDJMix.Infrastructure.csproj" />
    <ProjectReference Include="..\AutoDJMix.Persistence\AutoDJMix.Persistence.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 8: Create test csprojs**

Create: `tests/AutoDJMix.Domain.Tests/AutoDJMix.Domain.Tests.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\src\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>
</Project>
```

Create: `tests/AutoDJMix.Persistence.Tests/AutoDJMix.Persistence.Tests.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\src\AutoDJMix.Persistence\AutoDJMix.Persistence.csproj" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
  </ItemGroup>
</Project>
```

- [ ] **Step 9: Create minimal WPF entry files**

Create: `src/AutoDJMix.App/App.xaml`
```xml
<Application x:Class="AutoDJMix.App.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             ShutdownMode="OnMainWindowClose">
  <Application.Resources>
  </Application.Resources>
</Application>
```

Create: `src/AutoDJMix.App/App.xaml.cs`
```csharp
using System.Windows;

namespace AutoDJMix.App;

public partial class App : Application
{
}
```

Create: `src/AutoDJMix.App/MainWindow.xaml`
```xml
<Window x:Class="AutoDJMix.App.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="AutoDJMix" Height="600" Width="1000" Background="#FF0B0B0B">
  <Grid>
    <TextBlock Text="AutoDJMix" Foreground="#FFF2F2F2" FontSize="28" VerticalAlignment="Center" HorizontalAlignment="Center"/>
  </Grid>
</Window>
```

Create: `src/AutoDJMix.App/MainWindow.xaml.cs`
```csharp
using System.Windows;

namespace AutoDJMix.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

- [ ] **Step 10: Create solution and add projects**

Run:
```bash
dotnet new sln -n AutoDJMix
dotnet sln AutoDJMix.sln add src/AutoDJMix.App/AutoDJMix.App.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Application/AutoDJMix.Application.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Domain/AutoDJMix.Domain.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Analysis/AutoDJMix.Analysis.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Mixing/AutoDJMix.Mixing.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Transition/AutoDJMix.Transition.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Rendering/AutoDJMix.Rendering.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Persistence/AutoDJMix.Persistence.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Infrastructure/AutoDJMix.Infrastructure.csproj
dotnet sln AutoDJMix.sln add src/AutoDJMix.Migrator/AutoDJMix.Migrator.csproj
dotnet sln AutoDJMix.sln add tests/AutoDJMix.Domain.Tests/AutoDJMix.Domain.Tests.csproj
dotnet sln AutoDJMix.sln add tests/AutoDJMix.Persistence.Tests/AutoDJMix.Persistence.Tests.csproj
```

- [ ] **Step 11: Compile**

Run:
```bash
dotnet build AutoDJMix.sln -c Release
```
Expected: SUCCESS

## Task 2: Implement Domain primitives (Track, Role, Key/Camelot, BeatGrid)

**Files:**
- Create: `src/AutoDJMix.Domain/Primitives/Result.cs`
- Create: `src/AutoDJMix.Domain/Tracks/TrackId.cs`
- Create: `src/AutoDJMix.Domain/Tracks/TrackRole.cs`
- Create: `src/AutoDJMix.Domain/Tracks/RoleAssignment.cs`
- Create: `src/AutoDJMix.Domain/Tracks/Track.cs`
- Create: `src/AutoDJMix.Domain/Music/CamelotKey.cs`
- Create: `src/AutoDJMix.Domain/Music/BeatGrid.cs`

- [ ] **Step 1: Write Domain tests for parsing CamelotKey**

Create: `tests/AutoDJMix.Domain.Tests/Music/CamelotKeyTests.cs`
```csharp
using AutoDJMix.Domain.Music;
using Xunit;

namespace AutoDJMix.Domain.Tests.Music;

public sealed class CamelotKeyTests
{
    [Theory]
    [InlineData("1A")]
    [InlineData("2B")]
    [InlineData("6B")]
    public void Parse_valid_values(string value)
    {
        var key = CamelotKey.Parse(value);
        Assert.Equal(value, key.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0A")]
    [InlineData("1C")]
    [InlineData("AA")]
    public void Parse_invalid_values_throw(string value)
    {
        Assert.ThrowsAny<Exception>(() => CamelotKey.Parse(value));
    }
}
```

- [ ] **Step 2: Implement `CamelotKey`**

Create: `src/AutoDJMix.Domain/Music/CamelotKey.cs`
```csharp
using System.Text.RegularExpressions;

namespace AutoDJMix.Domain.Music;

public readonly record struct CamelotKey(string Value)
{
    private static readonly Regex Pattern = new(@"^(?:[1-9]|1[0-2])[AB]$", RegexOptions.Compiled);

    public static CamelotKey Parse(string value)
    {
        if (!TryParse(value, out var key))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid Camelot key.");
        }

        return key;
    }

    public static bool TryParse(string? value, out CamelotKey key)
    {
        if (value is null)
        {
            key = default;
            return false;
        }

        var normalized = value.Trim().ToUpperInvariant();
        if (!Pattern.IsMatch(normalized))
        {
            key = default;
            return false;
        }

        key = new CamelotKey(normalized);
        return true;
    }
}
```

- [ ] **Step 3: Run Domain tests**

Run:
```bash
dotnet test tests/AutoDJMix.Domain.Tests/AutoDJMix.Domain.Tests.csproj -c Release
```
Expected: PASS

- [ ] **Step 4: Add remaining Domain models**

Create: `src/AutoDJMix.Domain/Tracks/TrackRole.cs`
```csharp
namespace AutoDJMix.Domain.Tracks;

public enum TrackRole
{
    DarkHypnotic = 0,
    ProgressiveDriver = 1,
    MinimalBridge = 2,
    MelodicOpen = 3,
    PsyEdge = 4,
    NeutralTool = 5
}
```

Create: `src/AutoDJMix.Domain/Tracks/TrackId.cs`
```csharp
namespace AutoDJMix.Domain.Tracks;

public readonly record struct TrackId(Guid Value)
{
    public static TrackId New() => new(Guid.NewGuid());
}
```

Create: `src/AutoDJMix.Domain/Tracks/RoleAssignment.cs`
```csharp
namespace AutoDJMix.Domain.Tracks;

public enum RoleSource
{
    AnalysisAuto = 0,
    UserOverride = 1
}

public sealed record RoleAssignment(TrackRole Role, RoleSource Source, bool IsLocked, TrackRole? WeakHint);
```

Create: `src/AutoDJMix.Domain/Music/BeatGrid.cs`
```csharp
namespace AutoDJMix.Domain.Music;

public sealed record BeatGrid(TimeSpan StartTime, TimeSpan BeatInterval, double DriftPpm);
```

Create: `src/AutoDJMix.Domain/Tracks/Track.cs`
```csharp
namespace AutoDJMix.Domain.Tracks;

public sealed record Track(
    TrackId Id,
    string SourcePath,
    string DisplayName,
    TimeSpan Duration,
    string ContentHashSha256,
    RoleAssignment RoleAssignment);
```

- [ ] **Step 5: Compile**

Run:
```bash
dotnet build AutoDJMix.sln -c Release
```
Expected: SUCCESS

## Task 3: Implement MixEngine rule objects (score formula + hard overrides)

**Files:**
- Create: `src/AutoDJMix.Domain/Mixing/MixScore.cs`
- Create: `src/AutoDJMix.Domain/Mixing/MixDecisionBand.cs`
- Create: `src/AutoDJMix.Domain/Mixing/HardOverride.cs`
- Create: `src/AutoDJMix.Domain/Mixing/MixEngineScoreFormula.cs`
- Create: `tests/AutoDJMix.Domain.Tests/Mixing/MixEngineScoreFormulaTests.cs`

- [ ] **Step 1: Write tests for score formula**

Create: `tests/AutoDJMix.Domain.Tests/Mixing/MixEngineScoreFormulaTests.cs`
```csharp
using AutoDJMix.Domain.Mixing;
using Xunit;

namespace AutoDJMix.Domain.Tests.Mixing;

public sealed class MixEngineScoreFormulaTests
{
    [Fact]
    public void Compute_matches_spec_weights()
    {
        var score = MixEngineScoreFormula.ComputeFinalScore(baseCompat: 80, phaseBoost: 10, riskPenalty: -20);
        Assert.Equal(80 * 0.6 + 10 * 0.2 + (-20) * 0.2, score, 6);
    }
}
```

- [ ] **Step 2: Implement formula + decision band**

Create: `src/AutoDJMix.Domain/Mixing/MixEngineScoreFormula.cs`
```csharp
namespace AutoDJMix.Domain.Mixing;

public static class MixEngineScoreFormula
{
    public static double ComputeFinalScore(double baseCompat, double phaseBoost, double riskPenalty)
        => (baseCompat * 0.6) + (phaseBoost * 0.2) + (riskPenalty * 0.2);
}
```

Create: `src/AutoDJMix.Domain/Mixing/MixDecisionBand.cs`
```csharp
namespace AutoDJMix.Domain.Mixing;

public enum MixDecisionBand
{
    Avoid = 0,
    Risky = 1,
    Good = 2,
    Perfect = 3
}

public static class MixDecisionMatrix
{
    public static MixDecisionBand Classify(double finalScore)
    {
        if (finalScore >= 85) return MixDecisionBand.Perfect;
        if (finalScore >= 70) return MixDecisionBand.Good;
        if (finalScore >= 55) return MixDecisionBand.Risky;
        return MixDecisionBand.Avoid;
    }
}
```

- [ ] **Step 3: Add hard override types**

Create: `src/AutoDJMix.Domain/Mixing/HardOverride.cs`
```csharp
namespace AutoDJMix.Domain.Mixing;

public enum HardOverrideType
{
    ForceTransitionLoopBlendOnly = 0,
    ForceTransitionBreakRebuildOnly = 1,
    ForceEqBassNeverOverlap = 2
}
```

- [ ] **Step 4: Run Domain tests**

Run:
```bash
dotnet test tests/AutoDJMix.Domain.Tests/AutoDJMix.Domain.Tests.csproj -c Release
```
Expected: PASS

## Task 4: Add SQLite persistence (Library table) + first migration

**Files:**
- Create: `src/AutoDJMix.Persistence/AutoDjMixDbContext.cs`
- Create: `src/AutoDJMix.Persistence/Entities/TrackEntity.cs`
- Create: `src/AutoDJMix.Persistence/Migrations/*` (generated)
- Test: `tests/AutoDJMix.Persistence.Tests/SqliteMigrationTests.cs`

- [ ] **Step 1: Add EF Core design package (for migrations)**

Modify: `src/AutoDJMix.Persistence/AutoDJMix.Persistence.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\AutoDJMix.Domain\AutoDJMix.Domain.csproj" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Create TrackEntity + DbContext**

Create: `src/AutoDJMix.Persistence/Entities/TrackEntity.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace AutoDJMix.Persistence.Entities;

public sealed class TrackEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string SourcePath { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public long DurationMs { get; set; }

    [Required]
    public string ContentHashSha256 { get; set; } = string.Empty;
}
```

Create: `src/AutoDJMix.Persistence/AutoDjMixDbContext.cs`
```csharp
using AutoDJMix.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoDJMix.Persistence;

public sealed class AutoDjMixDbContext : DbContext
{
    public AutoDjMixDbContext(DbContextOptions<AutoDjMixDbContext> options) : base(options)
    {
    }

    public DbSet<TrackEntity> Tracks => Set<TrackEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackEntity>(b =>
        {
            b.HasIndex(x => x.SourcePath).IsUnique();
            b.HasIndex(x => x.ContentHashSha256);
        });
    }
}
```

- [ ] **Step 3: Generate first migration**

Run:
```bash
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 8.0.5
dotnet tool restore
dotnet ef migrations add InitialCreate --project src/AutoDJMix.Persistence/AutoDJMix.Persistence.csproj --startup-project src/AutoDJMix.Migrator/AutoDJMix.Migrator.csproj
```

- [ ] **Step 4: Add test to apply migrations on in-memory sqlite**

Create: `tests/AutoDJMix.Persistence.Tests/SqliteMigrationTests.cs`
```csharp
using AutoDJMix.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoDJMix.Persistence.Tests;

public sealed class SqliteMigrationTests
{
    [Fact]
    public void Can_apply_migrations()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AutoDjMixDbContext>()
            .UseSqlite(connection)
            .Options;

        using var db = new AutoDjMixDbContext(options);
        db.Database.Migrate();

        Assert.True(db.Database.CanConnect());
    }
}
```

- [ ] **Step 5: Run tests**

Run:
```bash
dotnet test tests/AutoDJMix.Persistence.Tests/AutoDJMix.Persistence.Tests.csproj -c Release
```
Expected: PASS

- [ ] **Step 6: Build the full solution**

Run:
```bash
dotnet build AutoDJMix.sln -c Release
```
Expected: SUCCESS

---

## Self-Review (performed)

- Spec coverage: This plan covers the requested foundation: solution structure, core Domain primitives (Track/Role/Camelot/BeatGrid) plus MixEngine score formula + hard override types, and SQLite persistence with initial migration for library/track storage.
- Placeholder scan: No TBD/TODO steps; all file paths and command lines are explicit.
- Type consistency: Namespaces and types are consistent across tasks and tests.
