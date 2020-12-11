using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PersistentGameData : MonoBehaviour {

	public static int dungeonLevel, rangeMin, rangeMax, maxMoves, playerLivesSave, potions;
	public static string playerName;
	public static Vector3 playerPosSave, exitPosSave;
	public static List<char> operatorCodesArray;
	public static int[,] dungeonMapSave;
	public static bool saveTheGame;

	//When this object is instantiated, this class will load the saved data from the file or 
	//create a set of new data if there is no file and this is a new game.
	void Awake () {
		saveTheGame = true;
		bool dataLoaded = LoadGameData();
		if (!dataLoaded) {
			dungeonMapSave = new int[1,1];
			operatorCodesArray = new List<char>();
			playerPosSave = new Vector3(0,0,0);
			exitPosSave = new Vector3(0,0,0);
			dungeonLevel = 1;
			playerLivesSave = 3;
			potions = 3;
			rangeMin = 0;
			rangeMax = 0;
			maxMoves = 0;
			playerName = "";
		}
	}

	//Before this object is destroyed, save the game data to the file.
	void OnDisable () {
		if (saveTheGame == true) {
			SaveGameData();
		}
	}

	//This method is run before a new scene is loaded to save the data of the previous scene.
	void SaveGameData () {
		GameData SavedGameData = new GameData();
		SavedGameData.dungeonLevel = dungeonLevel;
		SavedGameData.rangeMax = rangeMax;
		SavedGameData.rangeMin = rangeMin;
		SavedGameData.maxMoves = maxMoves;
		SavedGameData.playerLivesSave = playerLivesSave;
		SavedGameData.playerName = playerName;
		SavedGameData.potions = potions;
		SavedGameData.operatorCodesArray = operatorCodesArray;
		SavedGameData.playerPosSaveX = playerPosSave.x;
		SavedGameData.playerPosSaveY = playerPosSave.y;
		SavedGameData.playerPosSaveZ = playerPosSave.z;
		SavedGameData.exitPosSaveX = exitPosSave.x;
		SavedGameData.exitPosSaveY = exitPosSave.y;
		SavedGameData.exitPosSaveZ = exitPosSave.z;
		SavedGameData.dungeonMapSave = dungeonMapSave;
		SaveGameSystem.SaveGame(SavedGameData, "SavedGameData");
		Debug.Log("Game data saved.");
	}

	//This method uses the SaveGameSystem to load the data from the file and returns true if it finds the file.
	bool LoadGameData () {
		if (SaveGameSystem.DoesSaveGameExist ("SavedGameData")) {
			GameData SavedGameData = SaveGameSystem.LoadGame("SavedGameData") as GameData;
			dungeonLevel = SavedGameData.dungeonLevel;
			rangeMax = SavedGameData.rangeMax;
			rangeMin = SavedGameData.rangeMin;
			maxMoves = SavedGameData.maxMoves;
			playerLivesSave = SavedGameData.playerLivesSave;
			playerName = SavedGameData.playerName;
			potions = SavedGameData.potions;
			operatorCodesArray = SavedGameData.operatorCodesArray;
			dungeonMapSave = SavedGameData.dungeonMapSave;

			//The vector of the player's and exit's position is created out of the induvidual coordinates for each axis. 
			playerPosSave = new Vector3(SavedGameData.playerPosSaveX, SavedGameData.playerPosSaveY, SavedGameData.playerPosSaveZ);
			exitPosSave = new Vector3(SavedGameData.exitPosSaveX, SavedGameData.exitPosSaveY, SavedGameData.exitPosSaveZ);
			Debug.Log("Game data loaded.");
			return true;
		}
		return false;
	}
}

//This is a separate private class that inherits from SaveGame and is used by PersistentGameData to store the game data
//in a serialized permentant file.
[Serializable]
class GameData : SaveGame {

	public int dungeonLevel, rangeMin, rangeMax, maxMoves, playerLivesSave, potions;
	public string playerName;
	public float playerPosSaveX, playerPosSaveY, playerPosSaveZ;
	public float exitPosSaveX, exitPosSaveY, exitPosSaveZ;
	public List<char> operatorCodesArray;
	public int[,] dungeonMapSave;

	//The vector of the player's and exit's position is saved separately as three floats of each separate coordinate 
	//because Vector3 is not a standard data type.
}
