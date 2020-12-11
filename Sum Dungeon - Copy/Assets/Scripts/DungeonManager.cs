using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour {

	public GameObject livesDisplay, potionsDisplay, dungeonLevelDisplay;

	private DungeonGenerator dungeonGen;
	private DisplayDungeon displayDungeon;
	private Text lives, potions, dungeonLevel;

	//Finds the other classes on the level and manages when a new dungeon is generated.
	void Start () {
		dungeonGen = GameObject.FindObjectOfType<DungeonGenerator> ();
		displayDungeon = GameObject.FindObjectOfType<DisplayDungeon> ();
		if (PersistentGameData.dungeonMapSave.Length == 1) {
			dungeonGen.CreateNewDungeon ();
			displayDungeon.DisplayTileGrid (true);
		} else {
			displayDungeon.DisplayTileGrid (false);
		}
		//Displays the number of lives and potions the palyer has.
		lives = livesDisplay.GetComponent<Text>();
		potions = potionsDisplay.GetComponent<Text>();
		dungeonLevel = dungeonLevelDisplay.GetComponent<Text>();
		dungeonLevel.text = PersistentGameData.dungeonLevel.ToString();
		lives.text = PersistentGameData.playerLivesSave.ToString();
		potions.text = PersistentGameData.potions.ToString();
	}

	//When the player presses the heal button, one potion is used to give the player full health.
	public void Heal () {
		if (PersistentGameData.potions > 0) {
			PersistentGameData.potions -= 1;
			PersistentGameData.playerLivesSave = 3;
			lives.text = PersistentGameData.playerLivesSave.ToString();
			potions.text = PersistentGameData.potions.ToString();
		}
	}
}
