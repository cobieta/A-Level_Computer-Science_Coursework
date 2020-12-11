using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScores : MonoBehaviour {

	private GameObject[] textBoxes;
	private Text[] myText;

	//Finds the text boxes that display the high scores and finds the text components of them as well as the saved high
	//scores.
	void Start () {
		//Only the text boxes that are being used to display the high scores have been given the 'DisplayBox' tag.
		textBoxes = GameObject.FindGameObjectsWithTag ("DisplayBox");
		myText = new Text[textBoxes.Length];
		//Loops through each textBox gameObject to find their text component.
		for (int i = 0; i <= (textBoxes.Length - 1); i++) {
			myText [i] = textBoxes [i].GetComponent<Text> ();
		}
		//Loads all the high scores after the game objects used to display them have been found.
		LoadHighScores ();
	}

	void LoadHighScores () {
		//Each saved high score gets loaded into it's corresponding text box.
		myText[0].text = PersistentHighScores.bossesDefeated.ToString();
		myText[1].text = PersistentHighScores.enemiesDefeated.ToString();
		myText[2].text = PersistentHighScores.maxFloorsCleared.ToString();
		//When a new high score needs to be added, just use the same format as the lines above but increase the 
		//number of myText to access the new display box and reference the specific high score you want to load
		//from the savedHighScores class.
	}
}
