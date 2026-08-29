# Release process

Approach from [Publishing your first NuGet package with AppVeyor and MyGet](https://andrewlock.net/publishing-your-first-nuget-package-with-appveyor-and-myget/).

Every push to `master` builds and publishes a pre-release package to the MyGet CI feed
(`belgaard-ci`) automatically — no action needed for that. A build only publishes to nuget.org
when it was triggered by a git tag on `master` (`appveyor_repo_tag: true` in `appveyor.yml`).
The package version comes from the `version:` line in `appveyor.yml`, flowed into every
`.csproj` via `$(appveyor_build_version)`.

To cut a release once CI is green on `master`:

```powershell
./Release.ps1 -Version 4.15.0
# review the commit and tag, then:
git push origin master --tags
```

Or do both in one step with `-Push`.

Bump the minor version for anything that changes the supported target frameworks or is
otherwise more than a fix (e.g. a .NET upgrade); patch otherwise. This mirrors past releases —
e.g. the .NET 5 → 8 upgrade bumped 4.13 → 4.14, the .NET 8 → 10 upgrade bumped 4.14 → 4.15.

Commit message convention: `Bumped to X.Y.Z`, tag `vX.Y.Z` on the same commit.
