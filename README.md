# Sharp.Diagnostics.Logging

Micro-framework to improve the ergonomics of logging with the .NET
`TraceSource` API.

## Status

[![Build](https://github.com/sharpjs/Sharp.Diagnostics.Logging/workflows/Build/badge.svg)](https://github.com/sharpjs/Sharp.Diagnostics.Logging/actions)
[![NuGet](https://img.shields.io/nuget/v/Sharp.Diagnostics.Logging.svg)](https://www.nuget.org/packages/Sharp.Diagnostics.Logging)
[![NuGet](https://img.shields.io/nuget/dt/Sharp.Diagnostics.Logging.svg)](https://www.nuget.org/packages/Sharp.Diagnostics.Logging)

- **Stable:**      Used in production for years with no reported bugs.
- **Tested:**      100% coverage by automated tests.
- **Legacy:**      No further development except for occasional maintenance.
- **Documented:**  IntelliSense on everything.

The replacement for the legacy `TraceSource` API is
[`Microsoft.Extensions.Logging`](https://www.nuget.org/packages/Microsoft.Extensions.Logging).

## Installation

A [NuGet package](https://www.nuget.org/packages/Sharp.Diagnostics.Logging) is available.

## Documentation

Documented via IntelliSense.  No external documentation.

## Building From Source

Requirements:
- Visual Studio 2022 or later (if using Visual Studio).
- Appropriate .NET SDKs — see the target framework(s) specified in each `.csproj` file.
  - Download [.NET SDKs](https://dotnet.microsoft.com/download/dotnet)
  - Download [.NET Framework Developer Packs](https://dotnet.microsoft.com/download/dotnet-framework)

```powershell
# The default: build and run tests
.\Make.ps1 [-Test] [-Configuration <String>]

# Just build; don't run tests
.\Make.ps1 -Build [-Configuration <String>]

# Build and run tests w/coverage
.\Make.ps1 -Coverage [-Configuration <String>]
```

<!--
  Copyright 2022 Jeffrey Sharp

  Permission to use, copy, modify, and distribute this software for any
  purpose with or without fee is hereby granted, provided that the above
  copyright notice and this permission notice appear in all copies.

  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
  WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
  MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
  ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
  WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
  ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
  OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
-->
