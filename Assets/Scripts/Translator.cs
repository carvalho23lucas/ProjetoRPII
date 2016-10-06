using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public static class Translator {
	public static Dictionary<int, string[]> grid = new Dictionary<int, string[]>();
	public static int idbutton;

	public static void AddToList(string item, int rowPos, int colPos){
		if (grid[rowPos] == null)
			grid[rowPos] = new string[10];
		grid[rowPos][colPos] = item;
	}
}
