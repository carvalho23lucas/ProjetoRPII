using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MainGrid : MonoBehaviour {
	public int width, height, topstart;
	[SerializeField] Transform ScrollContent;
	[SerializeField] GameObject Cell;

	GameObject[,] grid;
	[SerializeField] GameObject SideMenu;

	void OnGUI()
	{
		
	}

	void Start()
	{
		Component[] Buttons = SideMenu.GetComponentsInChildren<Button> ();
		foreach (Component button in Buttons){
			int buttonid = int.Parse((button as Button).name);
			(button as Button).onClick.AddListener (() => { 
				Translator.idbutton = buttonid;
			});
		}

		grid = new GameObject[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GameObject cell = Instantiate (Cell);

				cell.GetComponent<Button>().onClick.AddListener (() => {
					if (Translator.idbutton == 2)
						cell.GetComponent<RawImage>().texture = Resources.Load("PlayNote") as Texture;
					else
						cell.GetComponent<RawImage>().texture = Resources.Load("EmptyCell") as Texture;
				});
				cell.transform.position = new Vector2 (5 * x, 5 * y);
				cell.transform.SetParent(ScrollContent, false);

				grid [x, y] = cell;
			}
		}
	}

	void Update()
	{
		
	}
}
