using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class MainGrid : MonoBehaviour {
	public int width, height;
	[SerializeField] public GameObject SideMenu;

	private GameObject[,] grid;
	private Translator translator;

	void Start()
	{
		translator = new Translator (width, height);
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
				translator.selectedCommand = command;
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
		for (int x = 0; x < grid.GetLength (0) - h + 1; x++) {
			for (int y = 0; y < grid.GetLength (1) - w; y++) {
				if (grid [x, y].GetComponent<Metadata> ().command != "")
					continue;
				if (y > 0 && grid [x, y - 1].GetComponent<Metadata> ().command == "")
					continue;
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
				if (grid[x, y].GetComponent<RawImage>().texture.name == "Valid")
					grid [x, y].GetComponent<RawImage> ().texture = Resources.Load ("EmptyCell") as Texture2D;
			}
		}
	}

	void BuildGrid(){
		foreach (Transform child in gameObject.transform) {
			GameObject.Destroy(child.gameObject);
		}

		grid = new GameObject[height, width];
		for (int x = 0; x < height; x++) {
			for (int y = 0; y < width; y++) {
				createObject (x, y);
			}
		}
	}
	void createObject(int x, int y){
		GameObject cell = Instantiate (translator.getObject(x, y));

		cell.GetComponent<Button>().onClick.AddListener (() => {
			Metadata meta = cell.transform.GetComponent<Metadata> ();
			if (translator.selectedCommand != null && (meta.isvalid || translator.selectedCommand == "")){
				translator.setObject(translator.selectedCommand, meta.x, meta.y);
				translator.selectedCommand = null;
				BuildGrid();
			}
		});

		cell.transform.SetParent(gameObject.transform, false);
		grid [x, y] = cell;
	}
}
