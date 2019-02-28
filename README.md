# LeanTest
A shared library used to simplify the process of creating and maintaining simple and consistent developer tests across projects.

## Deployment
The latest AppVeyor build [![Build status](https://ci.appveyor.com/api/projects/status/gd05aw9aslc3kgbq/branch/master?svg=true)](https://ci.appveyor.com/project/belgaard/leantest/branch/master) (from the latest push to master on GitHub) is available on myGet.org,
 - https://www.myget.org/F/belgaard-ci/api/v3/index.json

Note that v0.13.* LeanTest is built on .Net Standard 1.0. By using the very lowest common denominator it is possible to use LeanTest on all platforms dupported by .Net Standard.

The latest version will be built on at least .Net Standard 2.0, in order to avoid the many supporting dependencies needed by 1.0.

Officially released builds are available on nuGet.org,
 - Core: [![nuGet Core](https://img.shields.io/nuget/v/LeanTest.Core.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Core)
 - Mock: [![nuGet Mock](https://img.shields.io/nuget/v/LeanTest.Mock.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Mock)
 - JSon: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.JSon.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.JSon) 
 
 ## Support
 [![Join the chat at https://gitter.im/Leantestnet/Lobby](https://badges.gitter.im/Leantestnet/Lobby.svg)](https://gitter.im/Leantestnet/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## The Concept
The concept of _developer testing_ is not very well understood. This is my fault, but watch [my blog](https://blog.elgaard.com), I intend to eventually explain the concept in great detail.

In short, developer testing is about developers writing tests while developing code. And I mean tests that testers would call _real_ tests, not simply unit tests. Tests which are simple to write initially, then simple to maintain going forward. Tests which cover actual functionality which is recognisable by and valuable to the business.

The way we achieve all this is by _maximizing code under test_ but _minimizing data_.

Maximizing code under test means not mocking away logic unless we really have to. And we only really have to mock logic away if we cannot control it deterministically (or if it is really slow to execute). In practice, this usually means that truly external dependencies must be mocked and nothing more. And we have a single mocking strategy for an entire test suite, having slightly different mocking per test case is a no-no.

Minimizing data means ensuring that exactly the data needed for a given test to run (yes, we do this _per-test_ unlike the way we do mocking) is provided for the test. With naming we try to express exactly what characteristica of the data will make the test run.

Note that the versions built so far start with _0._, which means that I consider the code to be in a pre-release state. The consequence of this is that there may be small breaking changes even though only the minor part of the version is bumped up. As soon as the major part of the version is bumped to _1._ I will start using semantic versioning, so you can count on minor version upgrades being backwards compatible.

## An Example
Note that the test below declares all _data_ that it depends on in a `WithData` method. This is quite central to lean tests - the target under test is always _empty_ with respect to data before each test, while any data that each individual test depends on is explicitly declared. In this case, the data is fed into a simple mock of an external service. In other cases the data could be fed into an (initially empty) database or other means for keeping state.

````csharp
        [TestMethod]
        public void GetAgeMustReturn10WhenKeyMatchesNewedUpData()
        {
            _contextBuilder
                .WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
                .Build();

            int actual = _target.GetAge("FourtyTwo");

            Assert.AreEqual(10, actual);
        }
````

## References
An early version of LeanTest, as well as the concept of developer testing, was used extensively while developing Saxo Bank's social web site, [TradingFloor.com](https://www.tradingfloor.com/). Around 850+ developer tests (today 1000+ tests) covered most business functionality, running on a typical developer PC in around one minute. Major refactorings of the code were done with these tests as a safety net.

LeanTest is currently used internally in Saxo Bank's REST based [Open API](https://developer.saxo). I intend to also use LeanTest in my experimental [C# Open API](https://github.com/belgaard/TopOA) which wraps the official REST Open API.
