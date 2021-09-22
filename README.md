# Lean Testing in C# with LeanTest.Net

LeanTest.Net is a shared library which is used to simplify the process of creating and maintaining 
simple, consistent and lean developer tests across projects.

You can find a blog post which describes a core principle behind Lean Testing at [medium.com](https://javascript.plainenglish.io/why-dont-you-take-given-in-bdd-seriously-f168da29f1c).

You can find the documentation in [GitHub Pages](https://belgaard.github.io/Leantest/).

[![GitHub forks](https://img.shields.io/github/forks/belgaard/leantest?style=social)](https://github.com/belgaard/Leantest)

## Deployment
Pre-release nuGet packages from the latest AppVeyor build [![Build status](https://ci.appveyor.com/api/projects/status/gd05aw9aslc3kgbq/branch/master?svg=true)](https://ci.appveyor.com/project/belgaard/leantest/branch/master) (from the latest push to master on GitHub) are available on myGet.org,
 - https://www.myget.org/F/belgaard-ci/api/v3/index.json

LeanTest.Net is built on at least .Net Standard 2.0, in order to avoid the many supporting dependencies needed by 1.0.
Older versions from v0.13.* were built on .Net Standard 1.0 
in order to make it possible to use LeanTest.Net on all platforms supported by .Net Standard.

We use semantic versioning, so you can count on minor version upgrades being backwards compatible.
However, note that older versions that start with _0._, are considered as pre-releases. 
The consequence of this is that there may be small breaking changes 
even though only the minor part of the version was bumped up. 

Officially released builds are available on nuGet.org,
 - Core: [![nuGet Core](https://img.shields.io/nuget/v/LeanTest.Core.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Core)
 - JSon: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.JSon.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.JSon) 
 - DI.DotNetCore: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.DI.DotNetCore.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.DI.DotNetCore) 
 - MsTest: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.MsTest.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.MsTest) 
 - Xunit: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.Xunit.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Xunit) 
 - AspNetCore: [![nuGet Mock](https://img.shields.io/nuget/v/LeanTest.AspNetCore.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.AspNetCore)
 
## Support
 [![Join the chat at https://gitter.im/Leantestnet/Lobby](https://badges.gitter.im/Leantestnet/Lobby.svg)](https://gitter.im/Leantestnet/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## References
An early version of LeanTest, as well as the concept of developer testing, 
was used extensively while developing Saxo Bank's social web site, 
[TradingFloor.com](https://web.archive.org/web/20170223064452/https://www.tradingfloor.com/). 
Around 850+ developer tests (today 1000+ tests) covered most business functionality, running on a typical developer PC in around one minute. 
Major refactorings of the code were done with these tests as a safety net.

LeanTest is currently used internally in Saxo Bank's REST based [OpenAPI](https://developer.saxo).
