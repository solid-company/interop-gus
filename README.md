`SolidCompany.Interop.Gus` is a .NET Core/.NET StandardNET Core/.NET Standard interop package for Statistics Poland (pol. G³ówny Urz¹d Statystyczny, GUS) API. Enables downoading legal entity data
    using Polish TAX Id (NIP).

## Get Packages

You can get `SolidCompany.Interop.Gus` package by [downloading it from NuGet feed](https://www.nuget.org/packages/SolidCompany.Interop.Gus).

## Getting Started

`SolidCompany.Interop.Gus` requires a key that you can obtain from GUS. The whole instruction is available [here](https://api.stat.gov.pl/Home/RegonApi).

When you receive your key then you are free to use the client:

```C#
await using var client = new GusBirClient("your_key", GusEnironment.Production);

var entity = await client.FindByNipAsync("5261040828");
```