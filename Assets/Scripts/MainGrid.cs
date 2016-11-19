using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Linq;

public class MainGrid : MonoBehaviour {
	public int width, height;
	[SerializeField] public GameObject SideMenu;

	private GameObject[,] grid;
	private Assembler assembler;
	private string selectedCommand = "";
	private static long lastClick = -1;
	private static int lastXClick = -1, lastYClick = -1;

	void Start()
	{
		assembler = new Assembler (width, height);
		GridLayoutGroup gridlg = gameObject.GetComponent<GridLayoutGroup> ();
		gridlg.constraintCount = width;

		BuildSideMenu ();
		BuildGrid ();
	}

	private void BuildSideMenu(){
		Component[] Buttons = SideMenu.GetComponentsInChildren<Button> ();
		foreach (Component button in Buttons){
			string command = button.transform.GetComponent<Metadata>().command;
			(button as Button).onClick.AddListener (() => { 
				selectedCommand = command;
				if (command != ""){
					switch(command[0]){
						case 'i': CheckValidPlaces(3, 5, 'i', 'f', 'v', 'p' ); break;
						case 'f': CheckValidPlaces(3, 3, 'i', 'f', 'v', 'p'); break;
						case 'v': CheckValidPlaces(1, 1, 'i', 'f', 'p'); break;
						case 'p': CheckValidPlaces(1, 1, 'i', 'f', 'v'); break;
					}
				}
				else
					ClearValidPlaces ();
				//TODO play a preview of the note here!
			});
		}
	}
	private void CheckValidPlaces(int pieceWidth, int pieceHeight, params char[] invalidPrevious){
		ClearValidPlaces ();
		for (int i = 0; i < grid.GetLength (0) - pieceHeight + 1; i++) {
			for (int j = 0; j < grid.GetLength (1) - pieceWidth; j++) {
				if (grid [i, j].GetComponent<Metadata> ().command != "")
					continue;
				if (i > 0) {
					if (grid [i - 1, 0].GetComponent<Metadata> ().command == "")
						continue;
				}
				if (j > 0) {
					if (grid [i, j - 1].GetComponent<Metadata> ().command == "")
						continue;
					if (invalidPrevious.Contains (grid [i, j - 1].GetComponent<Metadata> ().command[0]))
						continue;
				}
				if (!ValidRectangle (i, j, pieceWidth, pieceHeight))
					continue;
				
				grid [i, j].GetComponent<Metadata> ().isvalid = true;
				grid [i, j].GetComponent<RawImage> ().color = Color.gray;
			}
		}
	}
	private bool ValidRectangle(int x, int y, int pieceWidth, int pieceHeight){
		for (int i = x; i < x + pieceHeight; i++)
			for (int j = y; j < y + pieceWidth; j++)
				if (grid [i, j].GetComponent<Metadata> ().command != "")
					return false;
		return true;
	}
	private void ClearValidPlaces(){
		for (int i = 0; i < grid.GetLength (0); i++)
			for (int j = 0; j < grid.GetLength (1); j++)
				if (grid [i, j].GetComponent<Metadata> ().isvalid) {
					grid [i, j].GetComponent<RawImage> ().color = Color.white;
					grid [i, j].GetComponent<Metadata> ().isvalid = false;
				}
	}

	private void BuildGrid(){
		ClearGrid ();
		grid = new GameObject[assembler.grid.Count, assembler.grid [0].Length];
		for (int x = 0; x < assembler.grid.Count; x++) {
			for (int y = 0; y < assembler.grid [0].Length; y++) {
				createObject (x, y);
			}
		}
	}
	private void ClearGrid(){
		foreach (Transform child in gameObject.transform)
			GameObject.Destroy(child.gameObject);
	}
	private void createObject(int x, int y){
		GameObject cell = Instantiate (assembler.getGameObject(x, y));
		populateFields (cell, x, y);
		if (assembler.grid [x] [y] == "bar") {
			Component[] Buttons = cell.GetComponentsInChildren<Button> ();
			foreach (Component Button in Buttons) {
				string command = Button.transform.GetComponent<Metadata> ().command;
				Button.transform.GetComponent<Button> ().onClick.AddListener (() => {
					if (command == "badd")
						assembler.insertLine (x);
					else if (command == "brem")
						assembler.removeLine (x);
					BuildGrid ();
				});
			}
		} else {
			cell.GetComponent<Button> ().onClick.AddListener (() => {
				Metadata meta = cell.transform.GetComponent<Metadata> ();

				long currentClick = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
				if (currentClick - lastClick < 300 && lastXClick == meta.x && lastYClick == meta.y) {
					selectedCommand = "";
					lastClick = lastXClick = lastYClick = -1;
				} else {
					lastClick = currentClick;
					lastXClick = meta.x;
					lastYClick = meta.y;
				}

				if (selectedCommand != null) {
					if (selectedCommand == "" && meta.command != "") {
						assembler.deleteObject (meta.x, meta.y);
						selectedCommand = null;
						BuildGrid ();
					} else if (meta.isvalid) {
						assembler.setObject (selectedCommand, meta.x, meta.y);
						selectedCommand = null;
						BuildGrid ();
					}
				}
			});
			Dropdown[] Dropdowns = cell.GetComponentsInChildren<Dropdown> ();
			InputField[] InputFields = cell.GetComponentsInChildren<InputField> ();
			foreach (Component DropDown in Dropdowns) {

			}
			foreach (Component InputField in InputFields) {

			}
		}

		cell.transform.SetParent(gameObject.transform, false);
		grid [x, y] = cell;
	}
	private void populateFields(GameObject cell, int x, int y)
	{
		string command = assembler.grid [x] [y];
		Dropdown[] Dropdowns = cell.GetComponentsInChildren<Dropdown> ();
		InputField[] InputFields = cell.GetComponentsInChildren<InputField> ();

		switch (command.Split (' ')[0]) {
			case "var": break;

			case "fvar": break;
			case "frto": break;
			
			case "ivar": break;

			case "pdru": 
			case "pgui": 
			case "pbas": 
			case "ppia": 
			case "psin": break;
		}
	}
}
