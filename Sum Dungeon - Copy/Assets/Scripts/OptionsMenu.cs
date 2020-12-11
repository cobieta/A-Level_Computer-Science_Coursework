using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	public GameObject livesDisplay, potionsDisplay, monsterHealthDisplay, startingNumberDisplay, movesLeftDisplay;
	public GameObject menu1, menu2, menu3, buttonGrid;
	public GameObject operatorButtonPrefab;

	private LevelManager lvlManager;
	private Text lives, potions, monsterHealth, startingNumber, movesLeft, messageDisplay;
	private GenerateMathQuestion genMathQ;
	private List<int[]> answerOperations; 
	private List<int> playerAnswers;
	private int[] mathsQuestion;
	private int movesLeftValue, state; 
	private bool giveRewards;

	//Initialise all game objects to display data to the user and generate a maths question. 
	void Start () {
		lvlManager = GameObject.FindObjectOfType<LevelManager>();
		genMathQ = GetComponent<GenerateMathQuestion>();
		messageDisplay = menu3.GetComponentInChildren<Text>();
		lives = livesDisplay.GetComponent<Text>();
		potions = potionsDisplay.GetComponent<Text>();
		monsterHealth = monsterHealthDisplay.GetComponent<Text>();
		startingNumber = startingNumberDisplay.GetComponent<Text>();
		movesLeft = movesLeftDisplay.GetComponent<Text>();
		lives.text = PersistentGameData.playerLivesSave.ToString();
		potions.text = PersistentGameData.potions.ToString();
		mathsQuestion = genMathQ.GetMathsQuestion();
		monsterHealth.text = mathsQuestion[2].ToString();
		startingNumber.text = mathsQuestion[1].ToString();
		movesLeftValue = mathsQuestion[0];
		movesLeft.text = movesLeftValue.ToString();
		answerOperations = genMathQ.GetAnswerSequence();
		answerOperations.Reverse();
		playerAnswers = new List<int>();
		CreateButtons();
		giveRewards = true;
	}

	//Check if the player has answered the question or died. 
	void Update () {
		if (movesLeftValue == 0) {
			bool correct = true;
			for (int i = 0; i < playerAnswers.Count; i++) {
				if (playerAnswers[i] != i) {
					correct = false;
				}
			}
			if (correct) {
				//Player has won battle.
				menu3.SetActive(true);
				messageDisplay.text = "You have won! You received a potion as a reward.";
				state = 1;
				Invoke("DealWithOutcome", 2);
			} else {
				//Player was not correct.
				PersistentGameData.playerLivesSave -= 1;
				lives.text = PersistentGameData.playerLivesSave.ToString();
				movesLeftValue = mathsQuestion[0];
				menu3.SetActive(true);
				messageDisplay.text = "You were not correct. The monster attacked and you lost a life.";
				state = 2;
				Invoke("DealWithOutcome", 2);
			}
		} else if (PersistentGameData.playerLivesSave == 0) {
			//Player is dead.
			if (PersistentGameData.dungeonLevel > PersistentHighScores.maxFloorsCleared) {
				PersistentHighScores.maxFloorsCleared = PersistentGameData.dungeonLevel;
			}
			menu3.SetActive(true);
			messageDisplay.text = "You are dead. Game Over.";
			state = 3;
			Invoke("DealWithOutcome", 2);
		}
	}

	//The outcome of the player answering a question or dying. 
	void DealWithOutcome () {
		switch (state) {
			case 1:
				if (giveRewards == true) {
					PersistentGameData.potions += 1;
					PersistentHighScores.enemiesDefeated += 1;
					giveRewards = false;
				}
				lvlManager.LoadAfterFade("04Dungeon");
				break;
			case 2:
				playerAnswers = new List<int>();
				menu3.SetActive(false);
				menu2.SetActive(false);
				menu1.SetActive(true);
				break;
			case 3:
				SaveGameSystem.DeleteSaveGame("SavedGameData");
				PersistentGameData.saveTheGame = false;
				lvlManager.LoadAfterFade("01StartMenu");
				break;
		}
	}

	//Creates the list of buttons for the user to enter their answer. 
	void CreateButtons () {
		int buttonNumber = answerOperations.Count;
		List<int> numbers = new List<int>();
		playerAnswers = new List<int>();
		for (int j = 0; j < buttonNumber; j++) {
			numbers.Add(j);
		}
		for (int i = 0; i < buttonNumber; i++) {
			GameObject operationButton = Object.Instantiate(operatorButtonPrefab, buttonGrid.transform) as GameObject;
			Text operationButtonText = operationButton.GetComponentInChildren<Text>();
			Button operationButtonFunction = operationButton.GetComponent<Button>();
			int randomPosition = Random.Range(0, numbers.Count);
			int thisPosition = numbers[randomPosition];
			numbers.RemoveAt(randomPosition);
			string buttonText = System.Convert.ToChar(answerOperations[thisPosition][0]).ToString();
			buttonText = System.String.Concat(buttonText, answerOperations[thisPosition][1].ToString());
			operationButtonText.text = buttonText;
			operationButtonFunction.onClick.AddListener(delegate {WhenButtonClicked(thisPosition); });
		}
	}

	//All buttons have this function to record the player's answer. 
	void WhenButtonClicked (int buttonNumber) {
		movesLeftValue -= 1;
		playerAnswers.Add(buttonNumber);
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

	//Called by the fight button to switch the visible menus.
	public void Fight () {
		menu2.SetActive(true);
		menu1.SetActive(false);
	}
}
