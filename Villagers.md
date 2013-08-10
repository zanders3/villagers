# Villagers

Defend the Villagers from an enslaught of evil!

A tower defence game with a town economy producing those towers/units

Environment is a 2D, Flat Grid.

### Buildings

- Town Hall
Allows the mayor to fast forward to night fall by winding the clock forward
Game over if the hall is destroyed.
The mayor spawns here.
Villagers gather here if they have nothing to do.

- House (max 6 = max 12 villagers)
Creates 2 villagers per house. Villagers go home here at nightfall.

- Stone/Wood Wall/Gate
Defensive shenanigans that baddies can smash.

- Stockpile
Stores a pile of items. One kind of item per stockpile square. Items are wood, stone, ore, weapons, etc.
	- Wood is displayed as planks of wood, stacked. Max 12 wood (2 trees)
	- Ore/Stone is displayed as a wooden box containing rocks. Max 6 stone (2 rocks)
	- Weapons are displayed in weapon racks. Max 6 weapons.
	- Iron is displayed as ingots, stacked.

- Poleturner
2 Wood => Pike

- Fletcher
2 Wood => Bow

- Tower
Archer villagers try to assign themselves to towers. Failing that they follow the mayor.

- Guard Post
Soldiers try to assign themselves to guard posts. Failing that they follow the mayor.

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

- On iPad: Zelda Phantom Hourglass controls!!

- WASD Keys move mayor around. The camera follows the mayor at a fixed distance and angle.
- Mouse clicks activate things:
	* Clicking on a resource makes a following villager gather that resource.
	* Clicking on a building with villagers following:
		+ if (damaged or not built)
			+ villager is a builder TODOOO
		+ else if (production building)
			+ villager is a producer
		+ else if (house with villagers inside)
			+ villager comes out of house and follows mayor TODOOO
	* Clicking on a resource on the stockpile with villagers following:
		+ Melee Weapon
			+ Villager becomes a soldier
		+ Ranged Weapon
			+ Villager becomes an archer
	* Clicking and dragging on the ground paints villager circular selection.
		+ Dismissing villagers causes them to return to their previous task.
- Building Construction a button on screen.
	* Opens up and allows you to pick a building. Unavailable buildings are grayed out.
	* Build button turns into a cancel button.
	* Unavailable villagers automatically tap the building out of the ground. 
	* It takes the buildings HP number of taps to construct.
	* Under construction blueprints are just a flat transparent blue box.

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

## Extension Ideas:

### Buildings

=== Next Level Technology ===
Gated by the foundry technology, since you need it to produce steel.

Additional Resource: Ore

- Foundry
Ore => Steel

- Blacksmith
Steel => Sword

- Armoury
Steel => Armour

- Steel Wall/Gate

- Improved Fletcher
Steel => Crossbow

- Cannon Tower
Contains a small cannon which requires 2 villagers to operate.

