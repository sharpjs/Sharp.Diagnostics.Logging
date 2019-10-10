# Sharp.Diagnostics.Logging

Micro-framework for logging via the .NET `TraceSource` API.

## Status

- 100% test coverage
- Version 0.0.0 used in production for several years with no reported bugs.
- Version 0.1.0 released recently, adding support for correlation via
  [`System.Diagnostics.Activity`](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.activity).
- Version 1.0.0 in development.

## Building From Source

Requirements:
- .NET Framework SDK: 4.5, 4.8
- .NET Core SDK: 3.0

```powershell
# The default: build and run tests
.\Make.ps1 [-Test] [-Configuration <String>]

# Just build; don't run tests
.\Make.ps1 -Build [-Configuration <String>]

# Build and run tests w/coverage
.\Make.ps1 -Coverage [-Configuration <String>]
```
