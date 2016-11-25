using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable]
public class Interpreter
{
	private Assembler assembler;
	private Player player;
	private bool isPlaying = false;
	private bool isPaused = false;
	private bool isStopped = false;
	public int lineNumber = 0;

	public Interpreter (Assembler assembler, Player player)
	{
		this.assembler = assembler;
		this.player = player;
	}

	public IEnumerator StartExecution ()
	{
		if (isPlaying)
			yield break;
		if (isPaused && !isStopped) {
			player.ResumeMusic ();
			isPaused = false;
			yield break;
		}
		player.StopMusic ();
		isPaused = isStopped = false;
		isPlaying = true;
		while (true) {
			if (lineNumber < 0 || lineNumber >= assembler.grid.Count)
				break;

			if (ExecuteCell (assembler.grid [lineNumber++], 0)) {
				yield return WaitForSecondsOrStop (8);
			}

			while (isPaused)
				if (isStopped)
					yield break;
				else
					yield return null;
			if (isStopped) 
				yield break;
		}
		lineNumber = 0;
	}
	public void InterruptExecution ()
	{
		isPlaying = false;
		isPaused = true;
		player.PauseMusic ();
	}
	public void StopExecution ()
	{
		lineNumber = 0;
		isPlaying = false;
		isPaused = false;
		isStopped = true;
		player.StopMusic ();
	}

	private bool ExecuteCell (string[] line, int i)
	{
		if (line [i] == "")
			return false;

		string[] parameters;
		switch (line [i] [0]) {
			case 'b':
				return false;
			case 'm':
				return ExecuteCell (line, i + 1);
			case 'v':
				parameters = line [i].Split (' ') [1].Split (';');
				setVariable (parameters [0], ParseIntOrDefault (parameters [1]));
				ExecuteCell (line, i + 1);
				return false;
			case 'p':
				player.PlayNote (line [i].Split (' ') [0], getVariable (line [i].Split (' ') [1]));
				ExecuteCell (line, i + 1);
				return true;
			case 'i': 
				if (line [i] == "iff") {
					parameters = line [i + 1].Split (' ') [1].Split (';');
					if (!compare (parameters [0], ParseIntOrDefault (parameters [1]), ParseIntOrDefault (parameters [2]))) {
						lineNumber = findElse (lineNumber, i);
					}
				}
				return false;
			case 'f':
				if (line [i] == "for") {
					parameters = (line [i + 1].Split (' ') [1].Split (';')).ToList ().Concat (new[]{ line [i + 2].Split (' ') [1] }).ToArray ();
					if (line [i + 1].Split (' ') [0] == "fvar") {
						line [i + 1] = line [i + 1].Split (' ') [0] + "* " + parameters [0] + ";" + parameters [1];
						setVariable (parameters [0], ParseIntOrDefault (parameters [1]));
					} else {
						setVariable (parameters [0], getVariable (parameters [0]) + 1);
					}

					if (!compare (parameters [0], 4, ParseIntOrDefault (parameters [2]))) {
						line [i + 1] = line [i + 1].Split (' ') [0].Replace ("fvar*", "fvar") + " " + parameters [0] + ";" + parameters [1];
						removeVariable (lineNumber);
						lineNumber = findForEnd (lineNumber, i) + 1;
					}
				} else if (line [i] == "fend") {
					int forStart = findFor (lineNumber, i);
					clearVariables (forStart + 1, lineNumber);
					lineNumber = forStart;
				}
				return false;
			default: return false;
		}
	}

	private int ParseIntOrDefault (string value)
	{
		int result;
		return int.TryParse (value, out result) ? result : 0;
	}
	private IEnumerator WaitForSecondsOrStop(float seconds)
	{
		while (seconds > 0.0 && !isStopped)
		{
			if (!isPaused)
					seconds -= Time.deltaTime;
			yield return null;
		}
	}

	private Dictionary<string, string> variables = new Dictionary<string, string> ();
	private int getVariable (string varName)
	{
		if (variables.ContainsKey (varName))
			return ParseIntOrDefault(variables [varName].Split (',') [1]);
		else
			return 0;
	}
	private void setVariable (string varName, int value)
	{
		if (variables.ContainsKey (varName))
			variables [varName] = variables [varName].Split (',') [0] + "," + value;
		else
			variables.Add (varName, lineNumber + "," + value);
	}
	private void removeVariable (int line)
	{
		clearVariables (line - 1, line + 1);
	}
	private void clearVariables (int startLine, int endLine)
	{
		List<string> keys = variables.Keys.ToList ();
		foreach (string varName in keys) {
			int varScope = ParseIntOrDefault (variables [varName].Split (',') [0]);
			if (varScope > startLine && varScope < endLine)
				variables.Remove (varName);
		}
	}

	private bool compare (string varName, int idCond, int value)
	{
		int varValue = getVariable (varName);
		switch (idCond) {
			case 0: return varValue == value;
			case 1: return varValue != value;
			case 2: return varValue < value;
			case 3: return varValue > value;
			case 4: return varValue <= value;
			case 5: return varValue >= value;
			default: return false;
		}
	}

	private int findElse (int currLine, int currColumn)
	{
		while (assembler.grid [currLine] [currColumn] != "else")
			currLine++;
		return currLine;
	}
	private int findFor (int currLine, int currColumn)
	{
		while (assembler.grid [currLine] [currColumn] != "for")
			currLine--;
		return currLine;
	}
	private int findForEnd (int currLine, int currColumn)
	{
		while (assembler.grid [currLine] [currColumn] != "fend")
			currLine++;
		return currLine;
	}
}