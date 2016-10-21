using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Translator {
	public List<string[]> grid;
	public string selectedCommand;

	public Translator(int width, int height){
		grid = new List<string[]>();
		for (int x = 0; x < height; x++)
			addNewLine (width);
	}

	public void addNewLine(int width){
		string[] vect = new string[width];
		for (int x = 0; x < width; x++)
			vect[x] = "";
		grid.Add (vect);
	}

	public void setCommand(string command, int x, int y){
		while (grid.Count <= x) addNewLine (grid [0].Length);
		grid[x][y] = command;
	}

	public void setObject(string command, int x, int y){
		switch (command.Split (' ') [0]) {
			case "for":
				setCommand (command.Split(' ')[0], x, y);
				setCommand (command.Split(' ')[1], x, y + 1);
				setCommand (command.Split(' ')[2], x, y + 2);
				setCommand ("fend", x + 1, y);
				setCommand ("fzzz", x + 1, y + 1);
				setCommand ("fzzz", x + 1, y + 2);
				break;
			case "iff":
				setCommand (command.Split(' ')[0], x, y);
				setCommand (command.Split(' ')[1], x, y + 1);
				setCommand (command.Split(' ')[2], x, y + 2);
				setCommand ("iels", x + 1, y);
				setCommand ("izzz", x + 1, y + 1);
				setCommand ("izzz", x + 1, y + 2);
				setCommand ("iend", x + 2, y);
				setCommand ("izzz", x + 2, y + 1);
				setCommand ("izzz", x + 2, y + 2);
				break;
			default: 
				setCommand (command, x, y);
				break;
		}
	}

	public GameObject getObject(int x, int y){
		GameObject cell;
		string command = grid [x] [y];

		switch (command.Split (' ')[0]) {
			case "var": cell = LoadPrefab ("Var"); break;

			case "for": cell = LoadPrefab ("For"); break;
			case "fvar": cell = LoadPrefab ("For"); break;
			case "frto": cell = LoadPrefab ("For"); break;
			case "fend": cell = LoadPrefab ("For"); break;
			case "fzzz": cell = LoadPrefab ("For"); break;
			case "fspc": cell = LoadPrefab ("For"); break;

			case "iff": cell = LoadPrefab ("If"); break;
			case "ivar": cell = LoadPrefab ("If"); break;
			case "ifdo": cell = LoadPrefab ("If"); break;
			case "iels": cell = LoadPrefab ("If"); break;
			case "iend": cell = LoadPrefab ("If"); break;
			case "izzz": cell = LoadPrefab ("If"); break;
			case "ispc": cell = LoadPrefab ("If"); break;

			case "pdru": cell = LoadPrefab ("Play"); break;
			case "pgui": cell = LoadPrefab ("Play"); break;
			case "pbas": cell = LoadPrefab ("Play"); break;
			case "ppia": cell = LoadPrefab ("Play"); break;
			case "psin": cell = LoadPrefab ("Play"); break;
				
			default: cell = LoadPrefab("EmptyCell"); break;
		}

		cell.transform.GetComponent<Metadata> ().setPosition (x, y);

		return cell;
	}

	private GameObject LoadPrefab(string name){
		return AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/" + name + ".prefab", typeof(GameObject)) as GameObject;
	}
}
