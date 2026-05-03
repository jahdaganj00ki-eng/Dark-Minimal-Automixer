# AutoDJMix

Foundation scaffold for a Windows (.NET 8 / WPF) application that will import tracks, analyze them, plan a DJ set using `mixEngine.json`, and render an MP3 export.

## Build (Windows)

Prereqs:
- Visual Studio 2022 (or newer)
- .NET SDK 8.x

Commands:
```powershell
dotnet restore .\AutoDJMix.sln
dotnet build .\AutoDJMix.sln -c Release
dotnet test .\AutoDJMix.sln -c Release
```

## GitHub Actions (Windows CI)

Workflow: `.github/workflows/ci.yml`

Push to `main` (or open a PR) to run build + tests on `windows-latest`.

