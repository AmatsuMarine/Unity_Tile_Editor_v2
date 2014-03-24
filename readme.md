Unity Tile Editor v2
---------------------

Tiles are all individual GameObjects with variables for their 4 neighbors (above, below, right, and left).  Cliffs are created to link neighboring Tiles.

Each Tile has its own Mesh Collider and cliffs such that the higher tile detects a collision with the cliff.

Cliff materials require a shader that uses the secondary UV set.

For an example, see the included Unity asset