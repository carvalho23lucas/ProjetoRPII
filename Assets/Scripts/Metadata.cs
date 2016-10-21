using UnityEngine;
using System.Collections;

public class Metadata : MonoBehaviour {
	public int x;
	public int y;
	public string command;
	public bool ignore;
	public bool isvalid;

	public void setPosition(int x, int y){
		this.x = x;
		this.y = y;
	}
}
