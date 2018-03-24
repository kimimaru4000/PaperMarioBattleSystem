# PaperMarioBattleSystem
An open source recreation of the turn-based battle system from the first two Paper Mario games. The goal is to create a modular, flexible battle system that is easily customizable and can be added to game projects with minimal effort.

## Features
* Turn-based Battles
* Status Effects
* Action Commands
* Items
* Badges
* Battle Menus
* Move Sequences (what happens when you perform a move)

Note that visual features (smoothness of movement, UI, etc.) are not being prioritized at the moment. The current focus is implementing the core systems found in the Paper Mario games in a flexible and extensible manner.

You can find an ~6 minute video showcasing many of the battle system's features [here](https://1drv.ms/v/s!AoVfzuXWGWSwi6FUJxn_8mUD5d3QSw). 

## Getting Started
You will need MonoGame 3.7 and Visual Studio 2017, but MonoDevelop, Xamarin Studio, and earlier versions of Visual Studio may work as well. The project targets DesktopGL and .NET 4.7.

Builds for Windows, OSX, and Linux can be made with the Ruge Deploy Tool (https://github.com/MetaSmug/MonoGame.Ruge.DeployTool) using the DeployToolSettings.dt config file in the repository. You may need to adjust the output paths.

## Contributing
Feel free to submit a pull request with details on your changes. Make sure your code has been tested and is well-commented.

## Author(s)
* **Thomas Deeb (aka Kimimaru)**

## License
This project is licensed under the MIT License - see the LICENSE.md file for details.

## Acknowledgements
* Jdaster64 (evasion Badge stacking, many other in-depth mechanics on the Paper Mario games) - https://supermariofiles.wordpress.com/
* The Spriter's Resource (Paper Mario sprites) - https://www.spriters-resource.com/