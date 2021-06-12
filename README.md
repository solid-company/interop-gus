`SolidCompany.Interop.Gus` is a .NET Core/.NET StandardNET Core/.NET Standard interop package for Statistics Poland (pol. Główny Urząd Statystyczny, GUS) API. Enables downoading legal entity data using Polish TAX Id (NIP).

## Get Packages

You can get `SolidCompany.Interop.Gus` package by [downloading it from NuGet feed](https://www.nuget.org/packages/SolidCompany.Interop.Gus).

## Getting Started

`SolidCompany.Interop.Gus` requires a key that you can obtain from GUS. The whole instruction is available [here](https://api.stat.gov.pl/Home/RegonApi).

When you receive your key then you are free to use the client:

```C#
await using var client = new GusBirClient("your_key", GusEnironment.Production);

var entity = await client.FindByNipAsync("5261040828");
```

## Dependency Injections

If you'd like to use `GusBirClient` with dependency injection then you can install a companion package [SolidCompany.Interop.Gus.DependencyInjection](https://www.nuget.org/packages/SolidCompany.Interop.Gus.DependencyInjection).

The only thing you need to do then is to add the following line of code to your service configuration:

```C#
services.AddGusClient(c =>
{
    c.Environment = GusEnironment.Production;
    c.Key = "your_key";
});
```

You'll be able to access `GusBirClient` through `IGusBirClient` interface.