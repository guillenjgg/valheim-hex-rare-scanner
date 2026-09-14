# HexRareScanner

Automatically adds minimap pins and plays a sound when selected rare creatures spawn. I hunt sea serpents and this mod helps track them.

## Features

* Automatically adds pins to the minimap and world map when tracked creatures spawn
* Optional sound notification when a tracked creature is detected
* Pins are automatically removed when the creature dies
* Supports rare creature tracking, including selected Deep North creatures
* Supports star-level filtering for creatures such as 2-star Wolves, Boars, Deer, Asksvin, and Frozen Greydwarfs
* Configurable through BepInEx configuration settings

## Tracked Creatures

Current supported creatures include:

* Sea Serpent
* Bonemaw Serpent
* Troll
* Black Forest Bear
* Vile Bear
* Abomination
* Stone Golem
* Morgen
* 2-star Wolf
* 2-star Boar
* 2-star Deer
* 2-star Asksvin
* Fallen Valkyrie
* Writhan
* Barka
* Shadow Person
* Deep North Skeleton
* 2-star Frozen Greydwarf

## Writhan

Writhan is an extremely rare Swamp spawn.

Spawn requirements include:

* Swamp biome
* Median biome area
* 5,000 to 8,000 meters from the world center
* Altitude between -2 and 10 meters
* Terrain slope between 0 and 35 degrees
* Can spawn during the day or night
* 5% spawn chance
* Spawn interval of 8,000 seconds
* Maximum nearby active Writhans: 1
* Can spawn at levels 1 through 3
* Cannot spawn inside a player base
* No boss, global key, environment, or event requirement

## Configuration

Configuration file:

```text
BepInEx/config/com.hex.rarescanner.cfg
```

## Links

* [Discord Support](https://discord.gg/wU2FXD94v4)
* [GitHub](https://github.com/guillenjgg/valheim-hex-rare-scanner)
