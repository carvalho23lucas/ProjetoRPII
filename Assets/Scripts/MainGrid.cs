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
	private string selectedCommand;

	void Start()
	{
		assembler = new Assembler (width, height);
		GridLayoutGroup gridlg = gameObject.GetComponent<GridLayoutGroup> ();
		gridlg.constraintCount = width;

		BuildSideMenu ();
		BuildGrid ();
	}

	void BuildSideMenu(){
		Component[] Buttons = SideMenu.GetComponentsInChildren<Button> ();
		foreach (Component button in Buttons){
			string command = button.transform.GetComponent<Metadata>().command;
			(button as Button).onClick.AddListener (() => { 
				selectedCommand = command;
				if (command != ""){
					switch(command.Split(' ')[0]){
						case "iif": CheckValidPlaces(3, 3); break;
						case "for": CheckValidPlaces(3, 2); break;
						default: CheckValidPlaces(1, 1); break;
					}
				}
				else BuildGrid();
			});
		}
	}
	void CheckValidPlaces(int w, int h){
		ClearValidPlaces ();
		for (int x = 0; x < grid.GetLength (0) - h + 1; x++) {
			for (int y = 0; y < grid.GetLength (1) - w; y++) {
				if (grid [x, y].GetComponent<Metadata> ().command != "")
					continue;
				if (y > 0){
					if(grid [x, y - 1].GetComponent<Metadata> ().command == "")
						continue;
					if (new[]{ "iff", "for", "ielse", "iend", "fend" }.Contains (grid [x, y - 1].GetComponent<Metadata> ().command.Split(' ')[0]))
						continue;
					if (selectedCommand != "")
						if (new[]{ 'i', 'f' }.Contains (selectedCommand [0]))
							continue;
					else {
						string test = grid [x, y - 1].GetComponent<Metadata> ().command;
						test = test + ".";
					}
				}
				if (y == 0 && x > 0 && grid [x - 1, y].GetComponent<Metadata> ().command == "")
					continue;

				grid [x, y].GetComponent<Metadata> ().isvalid = true;
				grid [x, y].GetComponent<RawImage> ().texture = Resources.Load ("Valid") as Texture2D;
			}
		}
	}
	void ClearValidPlaces(){
		for (int x = 0; x < grid.GetLength (0); x++) {
			for (int y = 0; y < grid.GetLength (1); y++) {
				grid [x, y].GetComponent<Metadata> ().isvalid = false;
				if (grid [x, y].GetComponent<RawImage> ().texture.name == "Valid")
					grid [x, y].GetComponent<RawImage> ().texture = Resources.Load ("EmptyCell") as Texture2D;
			}
		}
	}

	void BuildGrid(){
		foreach (Transform child in gameObject.transform) {
			GameObject.Destroy(child.gameObject);
		}

		grid = new GameObject[assembler.grid.Count, assembler.grid[0].Length];
		for (int x = 0; x < assembler.grid.Count; x++) {
			for (int y = 0; y < assembler.grid[0].Length; y++) {
				createObject (x, y);
			}
		}
	}
	void createObject(int x, int y){
		GameObject cell = Instantiate (assembler.getGameObject(x, y));

		cell.GetComponent<Button>().onClick.AddListener (() => {
			Metadata meta = cell.transform.GetComponent<Metadata> ();
			if (selectedCommand != null && (meta.isvalid || selectedCommand == "")){
				assembler.setObject(selectedCommand, meta.x, meta.y);
				selectedCommand = null;
				BuildGrid();
			}
		});

		cell.transform.SetParent(gameObject.transform, false);
		grid [x, y] = cell;
	}
}
