using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MainGrid : MonoBehaviour {
	public GameObject plane;
	public int width = 10, height = 10;

	void Awake()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GameObject gridPlane = Instantiate (plane) as GameObject;
				gridPlane.transform.position = new Vector2 (gridPlane.transform.position.x + x, gridPlane.transform.position.y + y);
			}
		}
	}

	void OnGUI(){

	}

	void Start()
	{
	
	}

	void Update()
	{
	
	}
}
