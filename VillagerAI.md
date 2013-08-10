# Villager AI Modes

Some example trees:

- ProductionVillager (priority selector)
	* Nightfall event (sequence selector)
		* Is nightfall? (condition)
		* MoveToBuilding[Home] (action)
	* GatherResource (sequence selector)
		* Need to gather resource? (condition)
		* Move to nearest resource (action)
		* Gather resource (action)
	* ProduceResource (sequence selector)
		* Need to produce resource? (condition)
		* Move to production building (action)
		* Produce resource (action)
	* PlaceResource (sequence selector)
		* Need to place resource? (condition)
		* Move to nearest stockpile (action)
		* Place resource on stockpile (action)
	* Idle (sequence selector)
		* Move to production building (action)

- IdleVillager (priority selector)
	* Flee
		* Is enemy near by? (condition)
		* Run away until far enough away (action)
	* Nightfall event (sequence selector)
		* Is it nightfall? (condition)
		* Move to home building (action)
	* ConstructBuilding (sequence selector)
		* Need to construct building? (condition)
		* Move to nearest building (action)
		* Construct building (action)
	* Idle (sequence selector)
		* Move to campfire (action)

* Soldier (priority selector)
	* Injured (sequence selector)
		* Have you got low health? (condition)
		* Run away until far enough away (action)
	* AttackEnemy (sequence selector)
		* Is enemy near by? (condition)
		* Select random nearest enemy (action)
		* Attack enemy (action)
	* MoveToEnemy (sequence selector)
		* Is enemy far away? (condition)
		* Move within range of nearest enemy (action)
	* Patrol (loop selector)
		* Patrol Flag #1 (action)
		* Patrol Flag #2 (action)
		* Patrol Flag #3 (action)

* Gremlin (priority selector)
	* Injured (sequence selector)
		* Have you got low health as well as friendlies? (condition)
		* Run away for a while (action)
	* AttackEnemy (sequence selector)
		* Is enemy near by? (condition)
		* Select random nearest enemy (action)
		* Attack enemy (action)
	* AttackBuilding (sequence selector)
		* SelectRandomNearbyBuilding
		* PathToSelection


### Behaviour Tree Nodes

- RootNode
	* GetState()
		* Running
		* Failed
		* Success
	* Reset()

- PrioritySelector
	* First node down gets priority
	* Each node is evaluated on every AI tick

- SequenceSelector
	* Runs first child node onwards until it returns Running or completes
	* Stores which node last returned Running
	* Reset resets this state

- LoopSelector
	* Similar to sequence selector except can't be reset and never returns Success

- Actions
	* GatherResource(resourcetype)
	* ProduceResource(resourcetype)
	* PlaceResource(resourcetype)
	* Flee(enemytype)
	
	* SelectNearest(tag,nearestToRandomlySelect)
	* SelectAssigned(tag) #Picks your assigned house or production building or target etc.

	* FleeSelection(currentselection)
	* DamageSelection(currentselection)
	* BuildSelection(currentselection)
	* PathToSelection(currentselection)

- Conditions
	* IsFarFrom(tag,distance)
	* IsNearTo(tag,distance)

	* IsCharacterNear(charactertype, distance)
	* IsCharacterFar(charactertype, distance)
	* IsNighttime
	* IsHealthLessThan(percent)
	* IsSelectionLessThan(distance)
	* IsSelectionMoreThan(distance)
	* HaveResource(resourcetype)
	* CanGatherSelection

### Character Knowledge Graph
- Nearby characters list (health, type bit field)

### Character Steering Actions
- Path follow (path)
- Flee (nearby bit field type)
- Pursuit (single target)
