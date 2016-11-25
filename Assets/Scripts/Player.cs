using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player {
	private AudioSource[] sources;
	private List<AudioSource> toUnpause;
	private int indexNext = 0;

	public Player (AudioSource[] sources){
		this.sources = sources;
		toUnpause = new List<AudioSource> ();
	}

	public void PlayNote (string instrument, int number){
		AudioClip clip = null;

		switch (instrument) {
			case "pdru": clip = Resources.Load ("Sounds/Drum/drum" + number, typeof(AudioClip)) as AudioClip; break;
			case "pgui": clip = Resources.Load ("Sounds/Guit/guit" + number, typeof(AudioClip)) as AudioClip; break;
			case "pbas": clip = Resources.Load ("Sounds/Bass/bass" + number, typeof(AudioClip)) as AudioClip; break;
			case "ppia": clip = Resources.Load ("Sounds/Piano/piano" + number, typeof(AudioClip)) as AudioClip; break;
			case "psin": clip = Resources.Load ("Sounds/Synth/synth" + number, typeof(AudioClip)) as AudioClip; break;
		}
		if (clip != null)
			sources[indexNext++ % sources.Length].PlayOneShot (clip);
	}

	public void StopMusic (){
		foreach (AudioSource source in sources) {
			if (source.isPlaying) {
				source.Stop ();
			}
		}
		toUnpause.Clear ();
	}
	public void PauseMusic (){
		foreach (AudioSource source in sources) {
			if (source.isPlaying) {
				toUnpause.Add (source);
				source.Pause ();
			}
		}
	}
	public void ResumeMusic (){
		foreach (AudioSource source in toUnpause) {
			source.UnPause ();
		}
		toUnpause.Clear ();
	}
}
