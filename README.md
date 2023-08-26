## Silver Fishstick
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) [![Telegram](https://img.shields.io/badge/Telegram-2CA5E0?style=for-the-badge&logo=telegram&logoColor=white)](https://t.me/trollicus)

Simple yet effective Linkvertise bypass for any link!

Built with Educational Purposes.


## Demonstration

[](https://github.com/Trollicus/silver-fishstick/assets/40140975/66c0a474-5aa4-4afd-b1ea-17ee40e08612)


## Getting Started

Instructions about setting up the project.

### Requirements

* .NET 7

### Build & Restore

Restore the nugget packages

```
dotnet restore
```

Via opening CMD in the project directory simply type

```
dotnet build
```


### Set-Up

Before usage you should:

In `Program.cs`:

```csharp 
Linkvertise linkvertise = new Linkvertise();
await linkvertise.Bypass(new Uri("link_here"));; 
```
You must add your link then run where it says "link_here"


