# PaperMarioBattleSystem
<img src="https://tdeeb.github.io/PaperMarioBattleSystem/images/PMBS%20Multibounce.gif" alt="Mario performing Multibounce." width="400" height="300"/>

Paper Mario Battle System (PMBS for short) is an open source recreation of the turn-based battle system from the first two Paper Mario games. The goal is to create a modular, flexible battle system that is customizable and can be used in game projects. The inspiration for this project came from the lack of open source battle systems that closely resemble Paper Mario.

More information can be found on the [wiki](https://github.com/tdeeb/PaperMarioBattleSystem/wiki), including potential improvements, documentation, and examples for using PMBS.

## Features
* Turn-based Battles
* All Status Effects from PM and TTYD
* Action Commands
* Items
* Badges
* Battle Menus
* Dialogue Bubbles with Control Codes
* Move Sequences
* Accurate and modular Damage Formula
* And more!

PMBS contains most of the core features from the battle system of Paper Mario and Paper Mario: The Thousand-Year Door, and it supports all interactions between characters found in the original games. It's extensible; you can create new interactions not seen in the original games, including, but not limited to, an attack that forces Mario to swap out his Partner or a ranged attack that suffers from Payback damage. This makes it a viable tool for Paper Mario enthusiasts to test out builds and new abilities.

## Installation
1. Clone the repository
2. Install [MonoGame](https://github.com/MonoGame/MonoGame) 3.7.0.1129 or later.
3. Install Visual Studio 2017, though earlier versions may work as well. On non-Windows platforms, install JetBrains Rider, MonoDevelop, or Xamarin Studio. On OSX you can also use Visual Studio for Mac. The project targets DesktopGL and .NET 4.7.1, and it uses the latest minor C# version (7.3).
4. Install NuGet or open the Package Manager Console in Visual Studio.
5. Run `nuget restore -PackagesDirectory PaperMarioBattleSystem` in a terminal or the Package Manager Console to restore all NuGet packages in the project. [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) 1.8.4.0 is currently used to help parse Control Codes for the dialogue system.

## Builds
Cross-platform builds can be made using Mono's [mkbundle](http://www.mono-project.com/docs/tools+libraries/tools/mkbundle/). You can find a great tutorial for setting up and using the tool on Windows [here](https://dotnetcoretutorials.com/2018/03/22/bundling-mono-with-a-net-executable-using-mkbundle-on-windows/). Something overlooked in the tutorial is that you will need to extract the runtime you want to build for. To do so, rename the runtime file's extension to ".zip" then extract it.

For native non-Windows builds to run, you may need to comment out code involving the System.Windows.Forms namespace. The only code in the project using System.Windows.Forms is the debug functions that take screenshots and dump logs, so it doesn't affect the battle system itself.

## Contributing
Contributions are encouraged and appreciated! Feel free to file an [issue](https://github.com/tdeeb/PaperMarioBattleSystem/issues) for any bugs, missing features, or suggestions for improving the battle system. [Pull requests](https://github.com/tdeeb/PaperMarioBattleSystem/pulls) are highly encouraged.

## Author(s)
* **Thomas Deeb (aka Kimimaru)**

## License
This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/tdeeb/PaperMarioBattleSystem/blob/master/LICENSE) file for details.

## Acknowledgements
* Jdaster64 (evasion Badge stacking, many other in-depth mechanics on the Paper Mario games) - https://supermariofiles.wordpress.com/
* Super Mario Wiki (PM and TTYD bestiaries, other miscellaneous information) - https://www.mariowiki.com/Main_Page
* The Spriter's Resource (Paper Mario sprites) - https://www.spriters-resource.com/
