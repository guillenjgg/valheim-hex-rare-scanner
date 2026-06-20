Automatically adds minimap pins and plays a sound when selected rare creatures spawn. I hunt sea serpents and this mod helps track them.

## Features

- Automatically adds pins to the minimap and world map when tracked creatures spawn
- Optional sound notification when a tracked creature is detected
- Pins are automatically removed when the creature dies
- Supports rare creature tracking (such as 2-star Wolves and 2-star Boars)
- Configurable through BepInEx configuration settings

## Tracked Creatures

Current supported creatures include:

- Sea Serpent
- Bonemaw Serpent
- Troll
- Black Forest Bear
- Vile Bear
- Abomination
- Stone Golem
- Morgen
- 2-star Wolf
- 2-star Boar
- 2-star Deer
- 2-star Asksvin
- Fallen Valkyrie

## Configuration

Configuration file:

```text
BepInEx/config/com.hex.rarescanner.cfg
```

### General

| Setting | Description |
|----------|-------------|
| IsModEnabled | Enable or disable the mod |
| PlayTrackedCreatureSound | Play a sound when a tracked creature is detected |

### Tracking

Enable or disable tracking for individual creatures:

- Track Sea Serpents
- Track Bonemaw Serpents
- Track Trolls
- Track Black Forest Bears
- Track Vile Bears
- Track Abominations
- Track Stone Golems
- Track Morgens
- Track 2-star Wolves
- Track 2-star Boars
- Track 2-star Deer
- Track 2-star Asksvin
- Track FallenValkyrie

## Installation

1. Install BepInEx
2. Place `HexRareScanner.dll` into:

```text
BepInEx/plugins/
```

3. Launch the game

## Multiplayer

This mod has only been tested in local single-player worlds.

Multiplayer compatibility is currently unknown and has not been tested on dedicated servers or multiplayer sessions.

## Notes

- Creature pins are temporary and do not persist between game sessions
- Pins are removed automatically when the tracked creature dies
- Existing player-created map pins are never modified
- Multiple tracked creatures can be active simultaneously
- Each tracked creature receives its own unique map pin
- Sound notifications can be disabled in the configuration

## Feedback

Found a bug, have a suggestion, or want to request a creature to track?

Report bugs or provide feedback on Discord:

https://discord.gg/wU2FXD94v4

## Source Code

GitHub Repository:

https://github.com/guillenjgg/valheim-hex-rare-scanner
