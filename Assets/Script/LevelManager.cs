using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	// state
	public enum State { Running, Win, Lose }
	public static State currentState = State.Running;

	public void LoadLevel(string level) {
		SceneManager.LoadScene (level);
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene ().name == "Game") {
			if (currentState == State.Win) {
				SceneManager.LoadScene ("Win");
			} else if (currentState == State.Lose) {
				SceneManager.LoadScene ("Lose");
			}
		} else if (SceneManager.GetActiveScene ().name == "Menu") {
			currentState = State.Running;
		}
	}
}
