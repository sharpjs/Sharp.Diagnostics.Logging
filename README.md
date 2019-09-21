# Sharp.Diagnostics.Logging

Micro-framework for logging via the .NET `TraceSource` API.

## Building From Source

Requirements:
- .NET Framework SDKs: 4.5, 4.6, 4.8
- .NET Core 2.2 SDK

```powershell
# The default: build and run tests
.\Make.ps1 [-Test] [-Configuration <String>]

# Just build; don't run tests
.\Make.ps1 -Build [-Configuration <String>]

# Build and run tests w/coverage
.\Make.ps1 -Coverage [-Configuration <String>]
```

## Status

- Version 0.0.0 used in production for several years.  *No reported bugs.*
- Version 1.0.0 in development.
- Fully documented via IntelliSense, but no external documentation.
