using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public float autoLoadLevelAfter;
	public int currentLoadedSceneIndex;

	private ScreenFader fader;
	private string levelName;

	//When the Awake procedure runs, levelIndex will be set to the current 
	//scene’s index from the build settings.
	void Awake () {
		currentLoadedSceneIndex = SceneManager.GetActiveScene().buildIndex;
		fader = GameObject.FindObjectOfType<ScreenFader>();
	}

	//If the autoLoadlevelAfter is not zero, the class will load the next level in the amount of seconds from
	//autoLoadLevelAfter.
	void Start () {
		if (autoLoadLevelAfter > 0) {
			Invoke("LoadNextLevel", autoLoadLevelAfter);
		}
	}

	//Loads the level with the name given to it.
	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);
		SceneManager.LoadScene(name);
	}

	//Reloads the current level.
	public void ReLoadLevel () {
		SceneManager.LoadScene(currentLoadedSceneIndex);
	}

	//The game stops running and closes.
	public void QuitRequest(){
		Debug.Log ("Quit requested");
		Application.Quit ();
	}

	//Loads the next level in the build settings order.
	public void LoadNextLevel () {
		SceneManager.LoadScene(currentLoadedSceneIndex+1);
	}

	//Tells the fader on the level to fade out and loads the next level in the build settings order after 1 second.
	public void LoadNextAfterFade () {
		fader.FadeOut();
		Invoke("LoadNextLevel", 1f);
	}

	//Tells the fader on the level to fade out and calls the LoadLevelFadeName procedure after 1 second.
	public void LoadAfterFade (string name) {
		fader.FadeOut();
		levelName = name;
		Invoke("LoadLevelFadeName", 1f);
	}

	//Called by the LoadAfterFade procedure to load the level with the name assigned to the variable levelName from
	// LoadAfterFade. This task is split into two prodecures because 'Invoke' can't call a procedure with parameters.
	void LoadLevelFadeName () {
		Debug.Log ("New Level load: " + levelName);
		SceneManager.LoadScene(levelName);
	}
}
