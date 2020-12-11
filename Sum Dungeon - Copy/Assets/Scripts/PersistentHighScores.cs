using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentHighScores : MonoBehaviour {

	public static int maxFloorsCleared, enemiesDefeated, bossesDefeated;

	//When this object is instantiated, this class will load the saved data from the file or create a set of new data
	//if there is no file and this is a new game.
	void Awake () {
		bool dataLoaded = LoadHighScoreData();
		if (!dataLoaded) {
			maxFloorsCleared = 0;
			enemiesDefeated = 0; 
			bossesDefeated = 0;
		}
	}

	//Before this object is destroyed, save the game data to the file.
	void OnDisable () {
		SaveHighScoreData();
	}

	//This method is run before a new scene is loaded to save the data of the previous scene.
	void SaveHighScoreData () {
		HighScoreData SavedHighScores = new HighScoreData();
		SavedHighScores.maxFloorsCleared = maxFloorsCleared;
		SavedHighScores.enemiesDefeated = enemiesDefeated;
		SavedHighScores.bossesDefeated = bossesDefeated;
		SaveGameSystem.SaveGame(SavedHighScores, "SavedHighScores");
		Debug.Log("High scores saved.");
	}

	//This method uses the SaveGameSystem to load the high score data from the file and returns true if it finds the file.
	bool LoadHighScoreData () {
		if (SaveGameSystem.DoesSaveGameExist ("SavedHighScores")) {
			HighScoreData SavedHighScores = SaveGameSystem.LoadGame("SavedHighScores") as HighScoreData;
			maxFloorsCleared = SavedHighScores.maxFloorsCleared;
			enemiesDefeated = SavedHighScores.enemiesDefeated;
			bossesDefeated = SavedHighScores.bossesDefeated;
			Debug.Log("High score data loaded.");
			return true;
		}
		return false;
	}
}

//This is a separate private class that inherits from SaveGame and is used by PersistentHighScores to store the high score
//data in a serialized permentant file.
[Serializable]
class HighScoreData : SaveGame {

	public int maxFloorsCleared, enemiesDefeated, bossesDefeated; 
}
