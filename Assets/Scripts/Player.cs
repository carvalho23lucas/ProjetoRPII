using UnityEngine;
using System.Collections;

public class Player {
	private AudioSource source;
	public Player (AudioSource source){
		this.source = source;
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
			source.PlayOneShot (clip);
	}
}
