using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogErrorDisplay : MonoBehaviour {

	private Text errorDisplay;

	//Called by the CreateNewGame class to initialise the text component of the game object and pass 
	//a message to the dialog box to display. 
	public void DisplayError (string errorMessage) {
		errorDisplay = GetComponent<Text>();
		errorDisplay.text = errorMessage;
	}
}
