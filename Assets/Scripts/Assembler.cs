using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable]
public class Assembler {
	public List<string[]> grid;
	private int varCount = 0;
	private int fvarCount = 0;
	private static bool createdVarOnce = false;

	public Assembler(int width, int height){
		grid = new List<string[]>();
		for (int x = 0; x < height; x++)
			addNewLine (width);
  	}
	private void addNewLine(int width){
		string[] vect = new string[width];
		for (int x = 0; x < width; x++)
			vect[x] = "";
		vect [width - 1] = "bar";
		grid.Add (vect);
  	}

	public void insertLine(int i){
		string[] vect = new string[grid [0].Length];
		for (int x = 0; x < grid [0].Length; x++)
			vect[x] = continueLoopsOrEmpty(i, x);
		vect [grid [0].Length - 1] = "bar";
		grid.Insert (i, vect);
	}
	public void removeLine(int i){
		for (int j = 0; j < grid [0].Length; j++) {
			if (grid [i] [j] != "" && grid [i] [j] [0] != 'm') {
				deleteObject (i, j, false);
			}
		}
		grid.Remove (grid [i]);
	}
	private int useOrCreateVarLine(int i, string varName, string playNumber){
		int emptyPlace = 0;
		int varCount = 0;
		if (i == 0) insertLine (i++);
		for (int j = 0; j < grid[i].Length; j++){
			if (grid [i - 1] [j] != "") {
				if (new[]{ 'm', 'f', 'i' }.Contains (grid [i - 1] [j] [0]))
					if (!grid [i - 1] [j].EndsWith ("end"))
						emptyPlace++;
				if (new[]{ 'v' }.Contains (grid [i - 1] [j] [0]))
					varCount++;
				if (new[]{ 'f', 'i', 'p' }.Contains (grid [i - 1] [j] [0]) || j == grid [i - 1].Length - 1) {
					insertLine (i++);
					varCount = 0;
					break;
				}
			} else break;
		}
		grid [i - 1] [emptyPlace + varCount] = "var " + varName + ";" + playNumber;
		createdVarOnce = true;
		return i;
	}
	private string continueLoopsOrEmpty(int i, int x){
		if (i == 0)
			return "";
		if (grid [i - 1] [x] == "")
			return "";
		if (grid [i - 1] [x] [0] != 'm'){
			if (grid [i - 1] [x] == "for")
				return "midfr";
			else if (new[]{ "iff", "iels" }.Contains (grid [i - 1] [x]))
				return "midif";
			else
				return "";
		}
		return grid [i - 1] [x];
	}

	public void setObject(string command, int x, int y){
		if (command == "") {
			setCommand (command, x, y);
			return;
		}
		switch (command [0]) {
			case 'f':
				string fvarName = ("fvar" + fvarCount++).Replace ("fvar0", "fvar");
				setCommand ("for", x, y); setCommand ("fvar " + fvarName + ";1", x, y + 1); setCommand ("frto 4", x, y + 2);
				setCommand ("midfr", x + 1, y);
				setCommand ("fend", x + 2, y); setCommand ("fzzz", x + 2, y + 1); setCommand ("fzzz", x + 2, y + 2);
				break;
			case 'i':
				setCommand ("iff", x, y); setCommand ("ivar var;0;1", x, y + 1); setCommand ("ifdo", x, y + 2);
				setCommand ("midif", x + 1, y);
				setCommand ("iels", x + 2, y); setCommand ("izzz", x + 2, y + 1); setCommand ("izzz", x + 2, y + 2);
				setCommand ("midif", x + 3, y);
				setCommand ("iend", x + 4, y); setCommand ("izzz", x + 4, y + 1); setCommand ("izzz", x + 4, y + 2);
				break;
			case 'p':
				string varName = !createdVarOnce ? ("var" + varCount++).Replace ("var0", "var") : "var";
				if (!createdVarOnce) x = useOrCreateVarLine (x, varName, command.Split (' ') [1]);
				setCommand (command.Split (' ') [0] + " " + varName, x, y);
				break;
			default: 
				setCommand (command, x, y);
				break;
		}
	}
	private void setCommand(string command, int x, int y){
		while (grid.Count <= x) addNewLine (grid [0].Length);
		grid[x][y] = command;
	}

	public void deleteObject (int x, int y, bool removeEmptyAuto = true){
		string command = grid [x] [y];
		if (command != "") {
			string[] trueOrigin = findOrigin (x, y).Split(',');
			x = int.Parse (trueOrigin [0]);
			y = int.Parse (trueOrigin [1]);

			switch (grid [x] [y] [0]) {
				case 'i':
				case 'f':
					deleteRecursive (x, y);
					break;
				default:
					tiltLine (x, y);
					break;
			}

			if (removeEmptyAuto)
				removeEmptyLines ();
		}
	}
	private string findOrigin (int x, int y){
		if (new[]{'i', 'f', 'm'}.Contains(grid [x] [y] [0])) {
			if (new[]{ "for", "iff" }.Contains (grid [x] [y]))
				return x + "," + y;
			else if (!new[]{ "midif", "midfr", "iels", "iend", "fend" }.Contains (grid [x] [y]))
				return findOrigin (x, y - 1);
			else
				return findOrigin (x - 1, y);
		} else
			return x + "," + y;
	}
	private void deleteRecursive (int x, int y){
		if (new[]{ "fend", "iend" }.Contains (grid [x] [y])) {
			kill3 (x, y);
		} else if (new[]{ "midfr", "midif" }.Contains (grid [x] [y])) {
			tiltLine (x, y);
			deleteRecursive (x + 1, y);
		} else if (new[]{ "for", "iff", "iels" }.Contains (grid [x] [y])) {
			kill3 (x, y);
			deleteRecursive (x + 1, y);
		}
	}
	private void kill3 (int x, int y){
		grid [x] [y] = grid [x] [y + 1] = grid [x] [y + 2] = "";
	}
	private void tiltLine(int x, int y){
		for (int i = y; i < grid [x].Length - 2; i++) {
			grid [x] [i] = grid [x] [i + 1];
		}
		grid [x] [grid [x].Length - 2] = "";
	}
	private void removeEmptyLines(){
		int lastValid = 0;
		for (int i = 0; i < grid.Count; i++)
			if (grid [i] [0] != "")
				lastValid = i;

		for (int i = 0; i < lastValid; i++) {
			if (grid [i] [0] == "") {
				grid.Remove (grid [i--]);
				addNewLine (grid [0].Length);
				lastValid--;
			}
		}
	}

	public GameObject getGameObject(int x, int y){
		GameObject cell;
		string command = grid [x] [y];

		switch (command.Split (' ')[0]) {
			case "var": cell = LoadPrefab ("Var"); break;

			case "for": cell = LoadPrefab ("For/For"); break;
			case "fvar": cell = LoadPrefab ("For/ForVar"); break;
			case "frto": cell = LoadPrefab ("For/ForTo"); break;
			case "fend": cell = LoadPrefab ("For/ForEnd"); break;
			case "fzzz": cell = LoadPrefab ("For/ForEmpty"); break;
			case "midfr": cell = LoadPrefab ("For/ForMid"); break;

			case "iff": cell = LoadPrefab ("If/If"); break;
			case "ivar": cell = LoadPrefab ("If/IfCond"); break;
			case "ifdo": cell = LoadPrefab ("If/IfDo"); break;
			case "iels": cell = LoadPrefab ("If/IfElse"); break;
			case "iend": cell = LoadPrefab ("If/IfEnd"); break;
			case "izzz": cell = LoadPrefab ("If/IfEmpty"); break;
			case "midif": cell = LoadPrefab ("If/IfMid"); break;

			case "pdru": cell = LoadPrefab ("Play/PlayDrum"); break;
			case "pgui": cell = LoadPrefab ("Play/PlayGuit"); break;
			case "pbas": cell = LoadPrefab ("Play/PlayBass"); break;
			case "ppia": cell = LoadPrefab ("Play/PlayPiano"); break;
			case "psin": cell = LoadPrefab ("Play/PlaySint"); break;
				
			case "bar": cell = LoadPrefab ("AddRemove"); break;

			default: cell = LoadPrefab("EmptyCell"); break;
		}

		cell.transform.GetComponent<Metadata> ().setPosition (x, y);

		return cell;
	}
	private GameObject LoadPrefab(string name){
		return AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/" + name + ".prefab", typeof(GameObject)) as GameObject;
	}
}
