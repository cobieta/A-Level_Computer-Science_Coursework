using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisplayDungeon : MonoBehaviour {

	public Tile wall, floor, exit;
	public GameObject ExitMapObject, player;
	public Vector3Int startingPosition;

	private Tilemap tileMap, exitMap;

	//Finds the Tilemap components of the game object this class is attached to and the other tile map game objects.
	void Start () {
		tileMap = GetComponent<Tilemap>();
		exitMap = ExitMapObject.GetComponent<Tilemap>();
	}

	//Loops through all tiles and sets the correct tile at the correct position on the tile map. 
	public void DisplayTileGrid (bool isNewDungeon) {
		float totalElements = (float)PersistentGameData.dungeonMapSave.Length;
		int gridLength = Mathf.RoundToInt(Mathf.Sqrt(totalElements)) - 1;
		int[,] dungeonGrid = PersistentGameData.dungeonMapSave;
		int floorArea = gridLength * gridLength;
		Vector3Int[] floorTileCoords = new Vector3Int[floorArea];
		int newTile = 0;
		Vector3Int currentCoords = new Vector3Int(0,0,0);
		for (int y = 0; y <= gridLength; y++) {
			for (int x = 0; x <= gridLength; x++) {
				currentCoords.x = x;
				currentCoords.y = y;
				if (dungeonGrid[x,y] == 0) {
					tileMap.SetTile(currentCoords, wall);
				} else {
					tileMap.SetTile(currentCoords, floor);
					floorTileCoords[newTile] = currentCoords;
					newTile++;
				}
			}
		}
		//Picks the position of the dungeon exit and the player's starting position if a new dungeon is generated,
		//otherwise the exit is placed at the coordinates loaded from memory.
		if (isNewDungeon == true) {
			int exitPosition;
			exitPosition = Random.Range(0, newTile);
			exitMap.SetTile(floorTileCoords[exitPosition], exit);
			PersistentGameData.exitPosSave = (Vector3)floorTileCoords[exitPosition];
			int entrancePosition;
			do {
				entrancePosition = Random.Range(0, newTile);
				startingPosition = floorTileCoords[entrancePosition];
				player.transform.position = startingPosition;
				PersistentGameData.playerPosSave = startingPosition;
			} while (exitPosition == entrancePosition);
			PlayerMovement movement = player.GetComponent<PlayerMovement>();
			movement.GetNewDungeon();
		} else {
			Vector3Int exitCoords;
			exitCoords = Vector3Int.RoundToInt(PersistentGameData.exitPosSave);
			exitMap.SetTile(exitCoords, exit);
		}
	}

}
