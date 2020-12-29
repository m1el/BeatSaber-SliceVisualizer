# Beat Saber Slice Visualizer mod

This plugin shows how your slice is offset from center.
Using that information, you can train your accuracy.

## Installation

Copy SliceVizualizer.dll to `Beat Saber\Plugins` directory.

## Configuration

Edit `Beat Saber\UserData\SliceVizualizer.json`.

## Setting up Development

Please read the https://github.com/Zingabopp/BeatSaberModdingTools/wiki
for more information on plugin development for BeatSaber.

In case you have troubles with setting up paths or references, please read:
https://github.com/Zingabopp/BeatSaberModdingTools/wiki/Resolving-References

To develop this plugin, you'll need:

- Visual Studio 2019
- Beat Saber installed
- Recommended to install Visual Studio extension `BeatSaberModdingTools.vsix` from https://github.com/Zingabopp/BeatSaberModdingTools/releases

Next:

- Clone this repo `git clone git@github.com:m1el/BeatSaber-SliceVisualizer.git`
- Copy `SliceVisualizer.csproj.user.example` to `SliceVisualizer.csproj.user`
- Edit `SliceVisualizer.csproj.user` to set your Beat Saber directory
- Build Solution

## LICENSE

The MIT License
