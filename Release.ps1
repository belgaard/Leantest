<#
.SYNOPSIS
Bumps the LeanTest package version and tags the release commit.

.DESCRIPTION
Updates the `version:` line in appveyor.yml, commits it to master as "Bumped to <version>",
and creates a matching git tag. See RELEASE.md for the full release process this script is part of.

.PARAMETER Version
The new package version, e.g. "4.15.0" (Major.Minor.Patch).

.PARAMETER Push
Push the commit and tag to origin immediately. Without this switch, the script stops after
committing and tagging locally so you can review before pushing.

.EXAMPLE
./Release.ps1 -Version 4.15.0
./Release.ps1 -Version 4.15.0 -Push
#>
param(
    [Parameter(Mandatory)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,

    [switch]$Push
)

$ErrorActionPreference = 'Stop'

$branch = git rev-parse --abbrev-ref HEAD
if ($branch -ne 'master') {
    throw "Must be on master (currently on '$branch')."
}

if (git status --porcelain) {
    throw "Working tree is not clean. Commit or stash changes first."
}

git fetch origin master --tags
$behind = git rev-list --count HEAD..origin/master
if ($behind -gt 0) {
    throw "Local master is behind origin/master by $behind commit(s). Pull first."
}

$appveyorPath = Join-Path $PSScriptRoot 'appveyor.yml'
$content = Get-Content $appveyorPath -Raw
$updated = $content -replace '(?m)^version: \d+\.\d+\.\d+\.\{build\}', "version: $Version.{build}"
if ($updated -eq $content) {
    throw "Could not find a 'version: X.Y.Z.{build}' line to update in appveyor.yml."
}
Set-Content -Path $appveyorPath -Value $updated -NoNewline

git add appveyor.yml
git commit -m "Bumped to $Version"
git tag "v$Version"

Write-Host "Committed and tagged v$Version locally."
if ($Push) {
    git push origin master --tags
    Write-Host "Pushed. AppVeyor will publish the master build to MyGet, then the tag build to nuget.org."
} else {
    Write-Host "Review with 'git show' / 'git log -1', then push with:"
    Write-Host "  git push origin master --tags"
}
