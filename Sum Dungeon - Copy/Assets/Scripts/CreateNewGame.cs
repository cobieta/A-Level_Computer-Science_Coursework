using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewGame : MonoBehaviour {

	public InputField playerName, maxRange, minRange;
	public GameObject dialogBox, errorMessage;

	private bool validSelection;
	private Toggle[] myToggles;
	private DialogErrorDisplay errorDisplay;
	private LevelManager lvlManager;
	//TODO private GenerateDungeon genDungeon

	//Finds all the classes it needs data from, including the PersistentGameData,
	//all the toggles, the level manager and the dungeon generator.
	void Start () {
		myToggles = GameObject.FindObjectsOfType<Toggle>();
		lvlManager = GameObject.FindObjectOfType<LevelManager>();
	}

	//Called when the player presses the Start Game button and reads what the
	//player has entered as well as creating new data to be saved.
	public void CreateGame () {
		bool correctInput = ValidateInputs();
		GetOperatorCodes();
		if (validSelection == false) {
			OpenDialogBox("Please pick at least one operator to use.");
		} else if (correctInput == true) {
			PersistentGameData.maxMoves = CalculateMaxMoves (PersistentGameData.dungeonLevel);
			PersistentGameData.dungeonMapSave = new int[1,1];
			PersistentGameData.playerPosSave = new Vector3(0,0,0);
			PersistentGameData.exitPosSave = new Vector3(0,0,0);
			PersistentGameData.dungeonLevel = 1;
			PersistentGameData.playerLivesSave = 3;
			PersistentGameData.potions = 3;
			lvlManager.LoadNextAfterFade();
		}
	}

	//Checks all the user inputs to make sure they are valid.
	bool ValidateInputs () {
		//If the player hasn't entered a name.
		if (playerName.text.Length == 0) {
			OpenDialogBox ("You forgot to enter a name.");
		} else {
			PersistentGameData.playerName = playerName.text;
			//Parsing the text to convert it into integers.
			int workingMin;
			int workingMax;
			//Convert the text into integers.
			bool minResult = int.TryParse (minRange.text, out workingMin);
			bool maxResult = int.TryParse (maxRange.text, out workingMax);
			//If the player hasn't entered a range.
			if (minResult == false) {
				OpenDialogBox ("You forgot to enter a minimum value.");
			} else if (maxResult == false) {
				OpenDialogBox ("You forgot to enter a maximum value.");
			} else {
				//If the minimum value is greater than the maximum values.
				if (workingMax < workingMin) {
					OpenDialogBox ("Your minimum value is greater than your maximum value, please swap the values.");
				} else {
					//If the minimum value is 0 or negative.
					if (workingMin < 1) {
						OpenDialogBox("Your minimum value must be greater than zero.");
					} else {
						//If all conditions are met, the values are saved.
						PersistentGameData.rangeMin = workingMin;
						PersistentGameData.rangeMax = workingMax;
						return true;
					}
				}
			}
		}
		return false;
	}

	//Displays the dialog box with the error message given to it.
	void OpenDialogBox (string message) {
		dialogBox.SetActive(true);
		errorDisplay = errorMessage.GetComponent<DialogErrorDisplay>();
		errorDisplay.DisplayError(message);
	}

	//Called when the user presses the CLOSE button on the dialog box to stop displaying the box. 
	public void CloseDialogBox () {
		dialogBox.SetActive(false);
	}

	//A formula I came up with myself to increase the maximum moves for a maths problem as the player progresses.
	int CalculateMaxMoves (int currentLevel) {
		float cLevel = (float)currentLevel;
		float fMoves = (4f * Mathf.Sqrt (cLevel)) / 1.5f;
		int moves = Mathf.RoundToInt(fMoves);
		return moves;
	}

	//The operatorCodesArray stores the different operations the player wants to use, 1 is addition, 2 is 
	//subtraction, 3 is multiplication and 4 is division.The different toggles corespond to different 
	//operators. 
	void GetOperatorCodes () {
		PersistentGameData.operatorCodesArray = new List<char>();
		validSelection = false;
		for (int i = 0; i <= 3; i++) {
			validSelection = true;
			if (myToggles[i].isOn == true) {
				switch (i) {
					case 1:
						PersistentGameData.operatorCodesArray.Add('+');
						break;
					case 2:
						PersistentGameData.operatorCodesArray.Add('-');
						break;
					case 3:
						PersistentGameData.operatorCodesArray.Add('×');
						break;
					default:
						PersistentGameData.operatorCodesArray.Add('÷');
						break;
				}
			}
		}
	}
}
