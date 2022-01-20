# Sharp.Diagnostics.Logging

Micro-framework to improve the ergonomics of logging with the .NET
`TraceSource` API.

## Status

- 100% test coverage.
- Used in production for several years with no reported bugs.
- No further development except for occasional maintenance.
  The replacement for the legacy `TraceSource` API is
  [`Microsoft.Extensions.Logging`](https://www.nuget.org/packages/Microsoft.Extensions.Logging/).

## Installation

A [NuGet package](https://www.nuget.org/packages/Sharp.Diagnostics.Logging/) is available.

## Documentation

Documented via IntelliSense.  No external documentation.

## Building From Source

Requirements:
- .NET Framework SDK: 4.6.1, 4.8
- .NET Core SDK: 3.1

```powershell
# The default: build and run tests
.\Make.ps1 [-Test] [-Configuration <String>]

# Just build; don't run tests
.\Make.ps1 -Build [-Configuration <String>]

# Build and run tests w/coverage
.\Make.ps1 -Coverage [-Configuration <String>]
```
