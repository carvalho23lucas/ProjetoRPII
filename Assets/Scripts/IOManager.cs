using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IOManager
{
	public List<string[]> LoadFile ()
	{
		List<string[]> grid = new List<string[]> ();
		string path = EditorUtility.OpenFilePanel ("Carregar arquivo de configuração", "", "bitb");
		if (path.Length != 0) {
			try {
				string line;
				using (StreamReader reader = new StreamReader (path, Encoding.Default)) {
					while ((line = reader.ReadLine ()) != null) {
						grid.Add (line.Split (','));
					}
				}
			} catch (System.Exception) { }
		}
		grid.Add (new string[grid [0].Length]);
		return grid;
	}
	public void SaveFile (List<string[]> grid)
	{
		string path = EditorUtility.SaveFilePanel ("Salvar arquivo de configuração", "", "BitBlocks.bitb", "bitb");
		if (path.Length != 0) {
			try {
				using (StreamWriter writer = new StreamWriter (path, false, Encoding.Default)) {
					foreach (string[] line in grid){
						writer.WriteLine (string.Join(",", line));
					}
				}
			} catch (System.Exception) { }
		}
	}
}