using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

	public int testLevelValue = 1;

	private int[,] tileArray;
	private int gridLength, newRegionNumber;
	private List<Vector2Int> frontierTilesList;
	private List<int[]> connectors;

	//This method is called whenever a new dungeon needs to be created.
	public void CreateNewDungeon () {
		newRegionNumber = 1;
		CreateGrid(PersistentGameData.dungeonLevel); 
		PlaceRooms(PersistentGameData.dungeonLevel);
		frontierTilesList = new List<Vector2Int>();
		GenerateCorridors();
		connectors = new List<int[]>();
		ConnectAllRegions();
		RemoveDeadEnds();
		PrintDungeon();
		PersistentGameData.dungeonMapSave = tileArray;
	}

	//This method is just used for debugging to check the dungeon looks right.
	void PrintDungeon () {
		string printedGrid = "Grid:";
		for (int y = gridLength; y >= 0; y--) {
			string printedGridLine = "| ";
			for (int x = 0; x <= gridLength; x++) {
				printedGridLine = System.String.Concat(printedGridLine, tileArray[x,y].ToString(), " | ");
				if (x == gridLength) {
					printedGrid = System.String.Concat(printedGrid, "\n", printedGridLine);
				}
			}
		}
		Debug.Log(printedGrid);
	}

	//Creates a square grid of wall tiles. 
	void CreateGrid (int sideLength) {
		//The size of the grid increases as the player gets further, it must also always be an odd number.
		sideLength = (2 * sideLength) + 13;
		tileArray = new int[sideLength, sideLength];
		gridLength = sideLength - 1;
		for (int y = 0; y <= gridLength; y++) {
			for (int x = 0; x <= gridLength; x++) {
				tileArray[x,y] = 0;
			}
		}
	}

	//Attemps to place a number of rooms randomly on the grid, the greater the level of the dungeon the more 
	//rooms it will try to place
	void PlaceRooms (int roomAttempts) {
		roomAttempts = roomAttempts + 2;
		int oddCoords = (gridLength / 2) - 2;
		for (int i = 1; i <= roomAttempts; i++) {
			//Find a random coordinate to place the room at.
			int xPos = GetRandomOddNumber(oddCoords);
			int yPos = GetRandomOddNumber(oddCoords);
			//Generate a random size for the room.
			int maxRoomSize = Mathf.RoundToInt(oddCoords/2);
			int xLength = GetRandomOddNumber(maxRoomSize);
			int yLength = GetRandomOddNumber(maxRoomSize);
			if (xLength == 1) {
				xLength = 3;
			}
			if (yLength == 1) {
				yLength = 3;
			}
			//Check the room won overlap with any other rooms and place the room on the grid.
			bool placeRoom = true;
			for (int y = yPos; y < (yPos + yLength); y++) {
				if (y > (gridLength-1)) {
					placeRoom = false;
					break;
				}
				for (int x = xPos; x < (xPos + xLength); x++) {
					if (x > (gridLength-1) || tileArray[x,y] != 0) {
						placeRoom = false;
						break;
					}
				}
			}
			//Place the room if it didn't overlap.
			if (placeRoom == true) {
				for (int y = yPos; y < (yPos + yLength); y++) {
					for (int x = xPos; x < (xPos + xLength); x++) {
						tileArray[x,y] = newRegionNumber;
					}
				}
				//Increase the number of the region so the next region has a different number.
				newRegionNumber++;
			}
		}
	}

	//Generates a random odd number between 1 and a maximum value.
	int GetRandomOddNumber (int maxRange) {
		int ranOddNum = Random.Range(0, maxRange+1);
		ranOddNum = (2*ranOddNum) + 1;
		return ranOddNum;
	}

	//Generates corridors that fill the spaces between the rooms using a maze generator based on Prim's algorithm.
	void GenerateCorridors () {
		//Find a closed tile to start the maze generator at.
		Vector2Int startPoint = AreAllOddTilesOpen();
		//Check all the odd tiles are open to finish running the maze generator.
		do { //Maze generator.
			//Open the tile with the starting tile coordinates.
			tileArray[startPoint.x,startPoint.y] = newRegionNumber;
			//Find frontier tiles.
			FindFrontierTiles(startPoint.x, startPoint.y);
			//While the list of frontier cells is not empty
			while (frontierTilesList.Count != 0) {
				//Pick a random frontier tile from the list of frontier tiles.
				int randomFrontier = Random.Range(0, frontierTilesList.Count);
				Vector2Int frontierTile = frontierTilesList[randomFrontier];
				//Find the neighbors of the fontier cell 
				Vector2Int[] neighbours = GetCardinalTileCoords(frontierTile.x, frontierTile.y);
				List<Vector2Int> connectNeighbours = new List<Vector2Int>();
				for (int i = 0; i <= 3; i++) {
					if (neighbours[i] != Vector2Int.zero && tileArray[neighbours[i].x, neighbours[i].y] == newRegionNumber) {
						connectNeighbours.Add(neighbours[i]);
					}
				}
				//Pick a random neighbour and connect it to the frontier tile.
				int randomNeighbour = Random.Range(0, connectNeighbours.Count);
				Vector2Int pickedNeighbour = connectNeighbours[randomNeighbour];
				tileArray[frontierTile.x, frontierTile.y] = newRegionNumber;
				tileArray[pickedNeighbour.x, pickedNeighbour.y] = newRegionNumber;
				int connectingX = (frontierTile.x + pickedNeighbour.x) / 2;
				int connectingY = (frontierTile.y + pickedNeighbour.y) / 2;
				tileArray[connectingX, connectingY] = newRegionNumber;
				//Remove the frontier tile from the list of frontier tile.
				frontierTilesList.RemoveAt(randomFrontier);
				//Find the frontier tiles of the current frontier tile.
				FindFrontierTiles(frontierTile.x, frontierTile.y);
			}
			newRegionNumber++;
			startPoint = AreAllOddTilesOpen();
		} while (startPoint != Vector2Int.zero);
	}

	//Loops through all tiles with odd coordinates and returns the coordinate of the first tile that is still closed,
	//Otherwise it returns the coordinates of the origin.
	Vector2Int AreAllOddTilesOpen () {
		for (int y = 1; y < gridLength; y += 2) {
			for (int x = 1; x < gridLength; x += 2) {
				if (tileArray[x,y] == 0) {
					Vector2Int emptyTile = new Vector2Int(x, y);
					return emptyTile;
				}
			}
		}
		return Vector2Int.zero;
	}

	//Adds the frontier tiles around a tile with the coordinates given to the frontier tiles list if it is not
	//already in the list.
	void FindFrontierTiles (int xCoord, int yCoord) {
		Vector2Int[] NESWtiles = GetCardinalTileCoords(xCoord, yCoord);
		for (int i = 0; i <= 3; i++) {
			if (NESWtiles[i] != Vector2Int.zero && tileArray[NESWtiles[i].x, NESWtiles[i].y] == 0) {
				if (frontierTilesList.Contains(NESWtiles[i]) == false) {
					frontierTilesList.Add(NESWtiles[i]);
				}
			}
		}
	}

	//Get the coordinates of each tile two away from the origional coordinate in every cardinal direction.
	Vector2Int[] GetCardinalTileCoords (int xCoord, int yCoord) {
		Vector2Int[] NESWArray = new Vector2Int[4];
		NESWArray [0] = new Vector2Int (xCoord, yCoord + 2);
		NESWArray [1] = new Vector2Int (xCoord + 2, yCoord);
		NESWArray [2] = new Vector2Int (xCoord, yCoord - 2);
		NESWArray [3] = new Vector2Int (xCoord - 2, yCoord);
		//Get rid of coordinates that are outside the grid.
		for (int i = 0; i <= 3; i++) {
			if (NESWArray [i].x > gridLength || NESWArray [i].x < 0) {
				NESWArray [i] = Vector2Int.zero;
			} else if (NESWArray [i].y > gridLength || NESWArray [i].y < 0) {
				NESWArray[i] = Vector2Int.zero;
			}
		}
		return NESWArray;
	}

	//Connect all rooms and corridors together to form one region.
	void ConnectAllRegions () {
		FindAllConnectors();
		//Connect the regions until the list of connecting tiles is empty.
		while (connectors.Count != 0) {
			//Get a random connector tile and connect the two regions it connects.
			int[] randomConnector = connectors[Random.Range(0, connectors.Count)];
			int startingRegion = randomConnector[2];
			int connectingRegion = randomConnector[3];
			//Open the connector.
			tileArray[randomConnector[0],randomConnector[1]] = startingRegion;
			//Convert the region number of the newly connected region to the number of the starting region.
			for (int y = 1; y < gridLength; y++) {
				for (int x = 1; x < gridLength; x++) {
					if (tileArray[x,y] == connectingRegion) {
						tileArray[x,y] = startingRegion;
					}
				}
			}
			//Remove all extraneous connectors between the two regions that are now connected but give them
			//some chance of opening up.
			for (int j = 0; j < connectors.Count; j++) {
				if (connectors[j][2] == startingRegion && connectors[j][3] == connectingRegion) {
					OpenUpChance(connectors[j][0], connectors[j][1], startingRegion);
					connectors.Remove(connectors[j]);
				} else if (connectors[j][2] == connectingRegion && connectors[j][3] == startingRegion) {
					OpenUpChance(connectors[j][0], connectors[j][1], startingRegion);
					connectors.Remove(connectors[j]);
				}
			}
		}
	}

	//Finds tiles that could connect two different regions.
	void FindAllConnectors () {
		//Iterate through all tiles.
		for (int y = 1; y < gridLength; y++) {
			for (int x = 1; x < gridLength; x++) {
				//Only check blocked tiles.
				if (tileArray[x,y] == 0) {
					//Check the tiles north and south for different regions.
					if (tileArray[x,y+1] != 0 && tileArray[x,y-1] != 0 && tileArray[x,y+1] != tileArray[x,y-1]) {
						connectors.Add(new int[4] {x, y, tileArray[x,y+1], tileArray[x,y-1]});
					//Check the tiles east and west for different regions.
					} else if (tileArray[x+1,y] != 0 && tileArray[x-1,y] != 0 && tileArray[x+1,y] != tileArray[x-1,y]) {
						connectors.Add(new int[4] {x, y, tileArray[x+1,y], tileArray[x-1,y]});
					}
				}
			}
		}
	}

	//Give a tile a 1 in 10 chance of opening up.
	void OpenUpChance (int xCoords, int yCoords, int connectToRegion) {
		int chance = Random.Range(1, 11);
		if (chance == 1) {
			tileArray[xCoords, yCoords] = connectToRegion;
		}
	}

	//The method that is called from the main CreateNewDungeon method and starts off the recursive
	//removal of dead ends. 
	void RemoveDeadEnds () {
		Vector2Int deadEnd = FindDeadEnds();
		tileArray[deadEnd.x, deadEnd.y] = 0;
		deadEnd = CountWalls(deadEnd.x, deadEnd.y);
		RecursiveRemove(deadEnd);
	}

	//Recursivley call this method until there are no more deadEnds.
	void RecursiveRemove (Vector2Int currentTile) {
		Vector2Int nextDeadEnd = CountWalls(currentTile.x, currentTile.y);
		if (nextDeadEnd != Vector2Int.zero) {
			//Close the dead end.
			tileArray[currentTile.x, currentTile.y] = 0;
			RecursiveRemove(nextDeadEnd);
		} else {
			Vector2Int newDeadEnd = FindDeadEnds();
			if (newDeadEnd != Vector2Int.zero) {
				RecursiveRemove(newDeadEnd);
			}
		}
	}

	//Find a new dead end and return its coordinates, otherwise return (0,0).
	Vector2Int FindDeadEnds () {
		for (int y = gridLength-1; y >= 1; y--) {
			for (int x = 1; x < gridLength; x++) {
				if (tileArray[x,y] != 0) {
					if (CountWalls(x,y) != Vector2Int.zero) {
						Vector2Int newDeadEnd = new Vector2Int(x,y);
						return newDeadEnd;
					}
				}
			}
		}
		return Vector2Int.zero;
	}

	//If the given tile is a dead end, return the coordinates of the tile connecting it to the 
	//rest of the dungeon, otherwise return the coordinates (0,0).
	Vector2Int CountWalls (int xCoord, int yCoord) {
		int wallCount = 0;
		Vector2Int openSide = new Vector2Int(0,0);
		Vector2Int[] NESWArray = new Vector2Int[4];
		NESWArray [0] = new Vector2Int (xCoord, yCoord + 1);
		NESWArray [1] = new Vector2Int (xCoord + 1, yCoord);
		NESWArray [2] = new Vector2Int (xCoord, yCoord - 1);
		NESWArray [3] = new Vector2Int (xCoord - 1, yCoord);
		foreach (Vector2Int v in NESWArray) {
			if (tileArray[v.x,v.y] == 0) {
				wallCount++;
			} else {
				openSide = v;
			}
		}
		if (wallCount == 3) {
			return openSide;
		} else {
			return Vector2Int.zero;
		}
	}
}
