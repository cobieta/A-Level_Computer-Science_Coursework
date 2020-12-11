using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGame : MonoBehaviour {

	private LevelManager lvlManager;

	//Find the level manager
	void Start () {
		lvlManager = GameObject.FindObjectOfType<LevelManager>();
	}

	//Called if the user selects continue game and only loads the game if there is a file saved. 
	public void ContinueGameSelected () {
		if (SaveGameSystem.DoesSaveGameExist ("SavedGameData")) {
			lvlManager.LoadAfterFade("04Dungeon");
		}
	}
}
