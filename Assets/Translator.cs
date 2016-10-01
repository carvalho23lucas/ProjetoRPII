using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Translator {
	Dictionary<int, string[]> grid = new Dictionary<int, string[]>();

	public void AddToList(string item, int rowPos, int colPos){
		if (grid [rowPos] == null)
			grid [rowPos] = new string[10];
		grid [rowPos] [colPos] = item;
	}
}
