﻿using UnityEngine;
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
					switch(command[0]){
						case 'i': CheckValidPlaces(3, 5, 'i', 'f', 'v', 'p' ); break;
						case 'f': CheckValidPlaces(3, 3, 'i', 'f', 'v', 'p'); break;
						case 'v': CheckValidPlaces(1, 1, 'i', 'f', 'p'); break;
						case 'p': CheckValidPlaces(1, 1, 'i', 'f', 'v'); break;
					}
				}
				else
					ClearValidPlaces ();
			});
		}
	}
	void CheckValidPlaces(int pieceWidth, int pieceHeight, params char[] invalidPrevious){
		ClearValidPlaces ();
		for (int i = 0; i < grid.GetLength (0) - pieceHeight + 1; i++) {
			for (int j = 0; j < grid.GetLength (1) - pieceWidth; j++) {
				if (grid [i, j].GetComponent<Metadata> ().command != "")
					continue;
				if (i > 0) {
					if (grid [i - 1, 0].GetComponent<Metadata> ().command == "")
						continue;
				}
				if (j > 0){ 
					if (grid [i, j - 1].GetComponent<Metadata> ().command == "")
						continue;
					if (invalidPrevious.Contains (grid [i, j - 1].GetComponent<Metadata> ().command[0]))
						continue;
				}

				grid [i, j].GetComponent<Metadata> ().isvalid = true;
				grid [i, j].GetComponent<RawImage> ().texture = Resources.Load ("Valid") as Texture2D;
			}
		}
	}
	void ClearValidPlaces(){
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				grid [i, j].GetComponent<Metadata> ().isvalid = false;
				if (grid [i, j].GetComponent<RawImage> ().texture.name == "Valid")
					grid [i, j].GetComponent<RawImage> ().texture = Resources.Load ("EmptyCell") as Texture2D;
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
			if (selectedCommand != null){
				if (selectedCommand == "" && meta.command != ""){
					assembler.deleteObject(meta.x, meta.y);
					BuildGrid();
				}
				else if (meta.isvalid){
					assembler.setObject(selectedCommand, meta.x, meta.y);
					selectedCommand = null;
					BuildGrid();
				}
			}
		});

		cell.transform.SetParent(gameObject.transform, false);
		grid [x, y] = cell;
	}
}
