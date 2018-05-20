# PaperMarioBattleSystem
An open source recreation of the turn-based battle system from the first two Paper Mario games. The goal is to create a modular, flexible battle system that is easily customizable and can be added to game projects with minimal effort.

## Features
* Turn-based Battles
* Status Effects
* Action Commands
* Items
* Badges
* Battle Menus
* Dialogue Bubbles
* Move Sequences (what happens when you perform a move)

Note that visual features (smoothness of movement, UI, etc.) are not being prioritized at the moment. The current focus is implementing the core systems found in the Paper Mario games in a flexible and extensible manner.

You can find an ~6 minute video showcasing many of the battle system's features [here](https://1drv.ms/v/s!AoVfzuXWGWSwi6FUJxn_8mUD5d3QSw).
You can also find a short video displaying the dialogue system [here](https://1drv.ms/v/s!AoVfzuXWGWSwi6Ix05m1gYyaE7-vQw).

## Getting Started
You will need at least MonoGame 3.7.0.1129 and Visual Studio 2017, but MonoDevelop, Xamarin Studio, and earlier versions of Visual Studio may work as well. The project targets DesktopGL and .NET 4.7.1, and it uses the latest C# version.

## Builds
Cross-platform builds can be made using Mono's [mkbundle](http://www.mono-project.com/docs/tools+libraries/tools/mkbundle/). You can find a great tutorial for setting up and using this tool on Windows [here](https://dotnetcoretutorials.com/2018/03/22/bundling-mono-with-a-net-executable-using-mkbundle-on-windows/). Something overlooked in the tutorial is that you will need to extract the runtime you want to build for. To do so, rename the runtime file's extension to ".zip" or something similar and then extract it.

For non-Windows builds to run, you may need to comment out code involving the System.Windows.Forms namespace. The only code in the project using System.Windows.Forms is the debug functions that take screenshots and dump logs, so it doesn't affect the battle system itself.

## Contributing
Feel free to submit a pull request with details on your changes. Please make sure that your code has been tested and is well-commented. Also feel free to open an issue if you encounter a bug or behavior that seems like a bug.

## Author(s)
* **Thomas Deeb (aka Kimimaru)**

## License
This project is licensed under the MIT License - see the LICENSE.md file for details.

## Acknowledgements
* Jdaster64 (evasion Badge stacking, many other in-depth mechanics on the Paper Mario games) - https://supermariofiles.wordpress.com/
* The Spriter's Resource (Paper Mario sprites) - https://www.spriters-resource.com/