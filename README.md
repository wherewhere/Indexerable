# Indexerable

[![Issues](https://img.shields.io/github/issues/wherewhere/Indexerable.svg?label=Issues&style=flat-square)](https://github.com/wherewhere/Indexerable/issues)
[![Stars](https://img.shields.io/github/stars/wherewhere/Indexerable.svg?label=Stars&style=flat-square)](https://github.com/wherewhere/Indexerable/stargazers)
[![NuGet](https://img.shields.io/nuget/dt/Indexerable.svg?logo=NuGet&style=flat-square)](https://www.nuget.org/packages/Indexerable)

Indexerable provides LINQ-style operations for `IReadOnlyList<T>`. Its list-oriented operators let you compose queries while retaining indexed access, without first materializing each intermediate result.

## Support Platform
- .NET Framework 4.5
- .NET Framework 4.7
- .NET Framework 4.7.1
- .NET Standard 1.0
- .NET Standard 1.2
- .NET Standard 2.0
- .NET Standard 2.1
- .NET Core 5.0
- .NET Core App 2.0
- .NET Core App 2.1
- .NET Core App 3.0
- .NET 6.0
- .NET 7.0
- .NET 8.0
- .NET 9.0

## Install

```shell
dotnet add package Indexerable
```

Then import the extension-method namespace:

```csharp
using System;
using System.Collections.Generic;
using Indexer.Linq;
```

## Quick start

```csharp
using Indexer.Linq;

IReadOnlyList<int> values = Indexerable.Range(1, 5);
IReadOnlyList<int> result = values
    .Select((value, index) => value * (index + 1))
    .Skip(1)
    .Take(3);

Console.WriteLine(result.Count);                // 3
Console.WriteLine(result[1]);                   // 9
Console.WriteLine(string.Join(", ", result));   // 4, 9, 16
```

## Available operations

All extension methods operate on `IReadOnlyList<T>` unless noted.

| Category | Methods |
| --- | --- |
| Create sequences | `Range`, `Repeat`, `Empty`, `InfiniteSequence` |
| Adapt lists | `AsReadOnlyList`, `ToReadOnlyList`, `Cast` |
| Transform and combine | `Select`, `Append`, `Prepend`, `Concat`, `Zip`, `Reverse`, `Index` |
| Select a portion | `Skip`, `SkipLast`, `Take`, `TakeLast` |
| Search | `IndexOf(value)`, `IndexOf(predicate)` |
| Copy elements | `CopyTo` |

`ToReadOnlyList` also has an overload for `string`, which exposes its characters as a read-only list. `InfiniteSequence<T>` is available on .NET 7 and later targets.

## How queries behave

- Sequence-producing operations return `IReadOnlyList<T>`, with `Count`, indexed access, and `foreach`. Search operations such as `IndexOf` return an index, while `CopyTo` copies elements into an array.
- Most operators return lightweight list views. Elements are generally computed when accessed or enumerated rather than materialized into a new collection. For example, `Select` invokes its selector each time an item is accessed; projected values are not cached.
- Views over an existing list retain that source rather than taking a snapshot, so changes to a mutable source may be visible through the view.
- Most operators require an `IReadOnlyList<T>`. Convert an `IList<T>` with `ToReadOnlyList` when needed; it wraps the source rather than copying it.
- `InfiniteSequence` is unbounded. Apply `Take` before enumerating it fully.
