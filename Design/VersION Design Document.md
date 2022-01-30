# VersION - What it is

VersION should help fix those pesky problems that have been bothering developers for a long time. Ever since the .NET ecosystem moved from direct assembly references towards NuGet packages, development became easier while also becoming harder.

Packages made it easy to add external libraries, keep them updated and so on. But at the same time it made development harder by having to manage packages and their versions, keeping them up-to-date and not being able to easily debug changes in libraries that can only be tested good in a real application. There are lots of tools that fix some of these problems but somehow not a single on was capable of doing it all at once with ease. And while NuGet itself improved over the time from an annoyingly instable package manager to a relatively useful and stable tool, it never really fixed the problems it caused by itself.

# VersION - What it does

VersION tries to fix the issues that lots of developers have to face. The goal is to become the swiss army knife for every developer in the .NET environment. Ultimately it should be capable of doing at least these things:

* Apply customizable, but plug'n'play ready versioning for NuGet packages at various stages and scenarios
  * Automatically during CI/CD builds
  * Manually through Visual Studio or the CLI
  * Automatically based on Git (Tag-based or via Conventional Changelog)
* Allow repacking NuGet packages
  * Upgrade preview packages (i.e. Alpha to Beta, Beta to RC, RC to Stable) or directly to stable packages
* Generate Changelogs
  * Using different formatters (i.e. Markdown, HTML or other custom formats)
  * Using different, configurable sources (i.e. Git history using Conventional Changelog)
  * Allowing to integrate custom steps into the generation pipeline
  * Publishing mechanisms (i.e. upload somewhere)
* Publishing Packages in one Go
  * Generate new Release on GitHub with generated changelog and artifacts
  * Support other services
* Extensive plugin support for all tasks