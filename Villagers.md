# Villagers

Defend the Villagers from an enslaught of evil.

A tower defence game with a town economy producing those defensive buildings/towers.

Environment is a 2D, Flat Grid.

### Buildings

- Town Hall
Allows the mayor to fast forward to night fall by winding the clock forward
Game over if the hall is destroyed.
The mayor spawns here.
Villagers gather here if they have nothing to do.

- House (max 6 = max 12 villagers)
Creates 2 villagers per house. Villagers go home here at nightfall.

- Stone/Wood Wall
Defensive shenanigans that baddies can smash.

- Gate
Closes automatically at night.

- Stockpile
Stores a pile of items. One kind of item per stockpile square. Items are wood, stone, ore, weapons, etc.
	- Wood is displayed as planks of wood, stacked. Max 12 wood (2 trees)
	- Ore/Stone is displayed as a wooden box containing rocks. Max 6 stone (2 rocks)
	- Weapons are displayed in weapon racks. Max 6 weapons.
	- Iron is displayed as ingots, stacked.

- Blacksmith
2 Stone => Sword

- Fletcher
2 Wood => Bow

- Guard Post (costs 3 swords, some wood)
Villagers assign themselves to this post, max 3 villagers per post

- Archery Tower (costs 3 bows, some stone)
Villagers assign themselves to this post, max 3 villagers per post

### Villagers

- Job types
	- Idle
	- Woodcutter
	- Mason
	- Blacksmith
	- Swordsman
	- Archer

### Scenery
- Tree
Spawns gremlins. Therefore, gremlins attack from all directions >:D
Contains 6 wood.

- Rocks
Produces a finite supply of stone.
Contains 3 stone.

- Cliff
An unassailable cliff of impassibility.

### Scripting
- Multiple levels, duh.
- Have multiple waves of enemies on different maps.
- THIS IS GOING TO WORK BAHAHAHA I WILL MAKE A COMPLETE GAME.

### Mayor

- Move with WASD
- Place buildings nearby yourself.
- Direct the villagers

Day/Night
- Day lasts 5 minutes, and can be skipped by the mayor by using the town hall.
- Night lasts until all the baddies have been eliminated.

Map Tile Types

0 Ground
1 Cliff
2 Water
3 Tree
4 Town Hall
5 Bridge
6 Rock
