using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveGameSystem {

	//Takes a saveGame object and saves it to the file name - 'name'.
	public static bool SaveGame(SaveGame saveGame, string name) {
		//Create a Binary formatter.
		BinaryFormatter formatter = new BinaryFormatter();
		//Create the file stream to where the game will be saved if the file does not already exist
		//a new file will be created otherwise it will overwrite the previous file.
		using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create)) {
			try {
				//Serialise the saveGame object using the binary formatter.
				formatter.Serialize(stream, saveGame);
			}
			catch (Exception) {
				//Returns false if the saveGame object cannot be saved for any reason.
				return false;
			}
		}
		//Returns true if the saveGame object is saved.
		return true;
	}

	//Deserializes the saveGame object with the file name - 'name'.
	public static SaveGame LoadGame(string name) {
		if (!DoesSaveGameExist(name)) {
			//If there is no save with the name - 'name', load nothing.
			return null;
		}
		//Create a binary formatter.
		BinaryFormatter formatter = new BinaryFormatter();
		//Open the file stream to the saveGame file.
		using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open)) {
			try {
				//Deserialize the file using the binary formatter and return it as a saveGame object.
				return formatter.Deserialize(stream) as SaveGame;
			}
			catch (Exception) {
				//Returns false if the saveGame object cannot be loaded for any reason.
				return null;
			}
		}
	}

	//Deletes a saveGame file with the file name - 'name'.
	public static bool DeleteSaveGame(string name) {
		try {
			//Deletes the file.
			File.Delete(GetSavePath(name));
		}
		catch (Exception) {
			//Returns false if the file cannot be deleted for any reason.
			return false;
		}
		//Returns true if the file is deleted.
		return true;
	}

	//Tries to find the save file with the name - 'name'. 
	public static bool DoesSaveGameExist(string name) {
		//Returns true if the file exists otherwise returns false.
		return File.Exists(GetSavePath(name));
	}

	//Finds the file path to the file with the name - 'name'.
	private static string GetSavePath(string name) {
		//Returns a string containing the complete path to the file.
		return Path.Combine(Application.persistentDataPath, name + ".sav");
	}

}
