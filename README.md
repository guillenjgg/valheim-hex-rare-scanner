# HexRareScanner

Automatically adds minimap pins and plays a sound when selected rare creatures spawn. I hunt sea serpents, and this mod helps track them.

## Features

* Automatically adds pins to the minimap when tracked creatures spawn.
* Optional sound notification when a tracked creature is detected.
* Pins are automatically removed when the creature dies.
* Tracked creature pins can be manually removed by right-clicking them on the map.
* Independently configure which creatures are tracked.
* Tracked creature settings are organized by biome.

## Tracked Creatures

Current supported creatures include:

### Ocean

* Sea Serpent
* Bonemaw Serpent

### Meadows

* 2-star Deer
* White Deer
* 2-star Boar

### Black Forest

* Troll
* Black Forest Bear

### Swamp

* Abomination
* Writhan

### Mountains

* Stone Golem
* 2-star Wolf

### Plains

* Vile Bear

### Mistlands

* Seeker Soldier
* 1-star or higher Seeker
* Gjall

### Ashlands

* 2-star Asksvin
* Fallen Valkyrie
* Morgen

### Deep North

* Barka
* Shadow Person
* Deep North Skeleton
* 2-star Frozen Greydwarf
* Gammeltroll
* Moose
* Fallen Warrior
* Eyeless One
* Elaking

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

`BepInEx/config/com.hex.rarescanner.cfg`

Tracked creatures can be enabled or disabled individually through the BepInEx configuration settings.

Manual map pin removal and tracked creature sounds can also be enabled or disabled through configuration.

## Links

* [Discord Support](https://discord.gg/wU2FXD94v4)
* [GitHub](https://github.com/guillenjgg/valheim-hex-rare-scanner)