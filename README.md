# Readme

## Deployment
The latest AppVeyor build [![Build status](https://ci.appveyor.com/api/projects/status/gd05aw9aslc3kgbq/branch/master?svg=true)](https://ci.appveyor.com/project/belgaard/leantest/branch/master) (from the latest push to master on GitHub) is available on myGet.org,
 - https://www.myget.org/F/belgaard-ci/api/v3/index.json

Note that v0.13.* LeanTest is built on .Net Standard 1.0. By using the very lowest common denominator it is possible to use LeanTest on all platforms dupported by .Net Standard.

The latest version will be built on at least .Net Standard 2.0, in order to avoid the many supporting dependencies needed by 1.0.

Note that the versions built so far start with _0._, which means that I consider the code to be in a pre-release state. The consequence of this is that there may be small breaking changes even though only the minor part of the version is bumped up. As soon as the major part of the version is bumped to _1._ I will start using semantic versioning, so you can count on minor version upgrades being backwards compatible.

Officially released builds are available on nuGet.org,
 - Core: [![nuGet Core](https://img.shields.io/nuget/v/LeanTest.Core.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Core)
 - Mock: [![nuGet Mock](https://img.shields.io/nuget/v/LeanTest.Mock.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Mock)
 - JSon: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.JSon.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.JSon) 
 
## Support
 [![Join the chat at https://gitter.im/Leantestnet/Lobby](https://badges.gitter.im/Leantestnet/Lobby.svg)](https://gitter.im/Leantestnet/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## References
An early version of LeanTest, as well as the concept of developer testing, was used extensively while developing Saxo Bank's social web site, [TradingFloor.com](https://www.tradingfloor.com/). Around 850+ developer tests (today 1000+ tests) covered most business functionality, running on a typical developer PC in around one minute. Major refactorings of the code were done with these tests as a safety net.

LeanTest is currently used internally in Saxo Bank's REST based [Open API](https://developer.saxo).
