using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private int score = 0;
	private int coins = 0;
	private float distance = 0;
	private float difficulty = 0.75f;

	[SerializeField] TextMesh scoreText;
	[SerializeField] TextMesh songName;

	private bool lost = false;

	[SerializeField] float gameSpeed = 5f;

	private GameObject player;
	private float lastPlayerZ;

	void Start () {
		songName.text = "Level: " + currentSongName();
	}
	
	void Update () {
		if (!lost) {
			trackPlayer();
			checkLoss();
		}
	}

	void checkLoss() {
		if (player.transform.position.y < -1f) {
			setLost(true);
		}
	}

	// Scoring:
		// Factors: coins, distance, difficulty
	void trackPlayer() {
		moveDistance(getPlayerZ() - lastPlayerZ);
		lastPlayerZ = getPlayerZ();
	}

	public void collectCoin() {
		if (lost)
			return;

		coins += 1;
		updateScore();
	}

	private void moveDistance(float d) {
		if (lost)
			return;

		distance += Mathf.Max(d, 0);
		updateScore();
	}

	void updateScore() {
		score = (int) ((coins + distance) * difficulty);

		if (score > PlayerPrefs.GetInt(currentSongName() + "_highscore")) {
			PlayerPrefs.SetInt(currentSongName() + "_highscore", score);
		}

		updateScoreText();
	}

	void updateScoreText() {
		if (lost) {
			scoreText.text = "Game Over.\nFinal score: " + score + "\nBest: " + PlayerPrefs.GetInt(currentSongName() + "_highscore");
		} else {
			scoreText.text = "Coins: " + coins + "c\nDistance: " + distance.ToString("F1") + "m\nScore: " + score;
		}
	}


	// Getters and setters
	public void setPlayer(GameObject p) {
		player = p;
		lastPlayerZ = player.transform.position.z;
	}

	public bool getLost() {
		return lost;
	}

	public void setLost(bool l) {
		lost = l;
		updateScoreText();
	}

	public void setDifficulty(float d) {
		difficulty = d;
	}

	public float getDifficulty() {
		return difficulty;
	}

	public float getSpeed() {
		return gameSpeed;
	}

	public float getPlayerZ() {
		return player.transform.position.z;
	}

	public static string currentSongName() {
		return ((AudioSource) GameObject.FindObjectOfType(typeof(AudioSource))).clip.name;
	}

	public static GameManager getActive() {
		return (GameManager) GameObject.FindObjectOfType(typeof(GameManager));
	}
}
