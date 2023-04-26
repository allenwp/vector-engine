# Vector Engine
A 3D video game engine for vector displays

# Setup

ASIO is non-redistributable. asio.h can be found in the ASIO SDK here: https://www.steinberg.net/en/company/developers.html
Place it in this path: /VectorEngine/BlueWave.Interop.Asio/asio.h

# Configuration

Change the following line in `Program.cs` of the Host or ConsoleHost project to change what game is loaded:
`public static readonly Type GameConfigType = typeof(Flight.GameConfig);`

When building a binary, you'll want to have the asset path changed to be relative to the binary. Do this by changing this line in `GameCOnfig.cs` of the game project:

`public static string GetAssetsPath() => @"../../../VectorEngine.Calibration/Assets";`

# [GUI] Host Usage

The engine is designed to be a very primitive ECS with automatic serialization and deserialization of the entire .NET object graph, similar to the way Unity behaves with automatic serialization and deserialization.

Ctrl + E will toggle the editor. Editor is disabled by default for release builds.

Other keyboard shortcuts include Ctrl + S to save the scene and Ctrl + D to duplicate the current selection when applicable.