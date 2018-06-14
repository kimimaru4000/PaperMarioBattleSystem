# PaperMarioBattleSystem
![Mario successfully performing a Hammer Action Command against a Goomba](https://1drv.ms/u/s!AoVfzuXWGWSwi6MeVjvbHC3kpY1CCg)

PaperMarioBattleSystem (PMBS for short) is an open source recreation of the turn-based battle system from the first two Paper Mario games. The goal is to create a modular, flexible battle system that is customizable and can be used in game projects. The inspiration for this project came from the lack of open source battle systems that closely resemble Paper Mario.

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

More information can be found on the [wiki](https://github.com/tdeeb/PaperMarioBattleSystem/wiki), including potential improvements, documentation, and examples for using PMBS.

You can also find a short video displaying the dialogue system [here](https://1drv.ms/v/s!AoVfzuXWGWSwi6Ix05m1gYyaE7-vQw).

## Getting Started
You will need at least MonoGame 3.7.0.1129 and Visual Studio 2017, but MonoDevelop, Xamarin Studio, and earlier versions of Visual Studio may work as well. The project targets DesktopGL and .NET 4.7.1, and it uses the latest C# version.

## Builds
Cross-platform builds can be made using Mono's [mkbundle](http://www.mono-project.com/docs/tools+libraries/tools/mkbundle/). You can find a great tutorial for setting up and using this tool on Windows [here](https://dotnetcoretutorials.com/2018/03/22/bundling-mono-with-a-net-executable-using-mkbundle-on-windows/). Something overlooked in the tutorial is that you will need to extract the runtime you want to build for. To do so, rename the runtime file's extension to ".zip" or something similar and then extract it.

For native non-Windows builds to run, you may need to comment out code involving the System.Windows.Forms namespace. The only code in the project using System.Windows.Forms is the debug functions that take screenshots and dump logs, so it doesn't affect the battle system itself.

## Dependencies
* [MonoGame 3.7+](https://github.com/MonoGame/MonoGame) - rendering, input, and more.
* [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) - for parsing Control Codes in Dialogue Bubbles.

## Contributing
Contributions are encouraged and appreciated! Feel free to file an [issue](https://github.com/tdeeb/PaperMarioBattleSystem/issues) for any bugs, missing features, or suggestions for improving the battle system. [Pull requests](https://github.com/tdeeb/PaperMarioBattleSystem/pulls) are also being accepted.

## Author(s)
* **Thomas Deeb (aka Kimimaru)**

## License
This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/tdeeb/PaperMarioBattleSystem/blob/master/LICENSE) file for details.

## Acknowledgements
* Jdaster64 (evasion Badge stacking, many other in-depth mechanics on the Paper Mario games) - https://supermariofiles.wordpress.com/
* Super Mario Wiki (PM and TTYD bestiaries, other miscellaneous information) - https://www.mariowiki.com/Main_Page
* The Spriter's Resource (Paper Mario sprites) - https://www.spriters-resource.com/