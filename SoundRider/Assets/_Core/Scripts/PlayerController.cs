using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float lane;
	private int max_lane = 3;
	private float run_speed = 10f;

	private Rigidbody rb;

	private bool jump;
	private int jumps;
	[SerializeField] int jumps_allowed = 2;
	private float jump_force = 5f;

	private float start_height;
	private int dead_height = -1;

	private int score = 0;

	[SerializeField] TextMesh scoreText;

	private Tween slider = new Tween(0.125f, 2, 2);

	void Start () {
		rb = GetComponent<Rigidbody>();
		start_height = transform.position.y;
	}
	
	void Update () {
		if (Input.GetKeyDown("space") && jumps < jumps_allowed) {
			jump = true;
		}

    	if (Input.GetKeyDown("left") && lane > 0) {
    		slide(-1);
    	}

    	if (Input.GetKeyDown("right") && lane < max_lane) {
    		slide(1);
    	}
    }

    void slide(int dir) {
    	if (!slider.isFinished()) {
    		slider.finish();
    		lane = slider.getValue();
    	}

    	slider.start(lane, lane + dir);
    }

    void FixedUpdate() {
    	if (jump) {
            rb.velocity += new Vector3(0f, jump_force, 0f);
            jump = false;
            jumps += 1;
    	}

    	if (rb.velocity.y < 0 && rb.velocity.y > Physics.gravity.y * 10f) {
    		rb.velocity -= new Vector3(0f, 1f, 0f);
    	}

    	if (transform.position.y <= start_height) {
    		jumps = 0;
    	}

    	if (transform.position.y <= dead_height) {
    		scoreText.text = "You lost.\nFinal score: " + score + "\nBest: " + PlayerPrefs.GetInt(currentSongName() + "_highscore");
    	}

    	lane = slider.getValue();
		transform.position = new Vector3(-1.5f + lane, transform.position.y, transform.position.z); // lock to our lane

		float mult = run_speed;
		if (Input.GetKey("up")) {
			mult *= 2.5f;
		} else if (Input.GetKey("down")) {
			mult *= -2.5f;
		}
		
		rb.MovePosition(transform.position + Vector3.forward * mult * Time.deltaTime);
	}

	public void setRunSpeed(float r) {
		run_speed = r;
	}

	public float getZPosition() {
		return transform.position.z;
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Coin") {
			getPoint();
			Destroy(col.gameObject);
		}
	}

	void getPoint() {
		score += 1;
		scoreText.text = "Score: " + score;

		if (score > PlayerPrefs.GetInt(currentSongName() + "_highscore")) {
			PlayerPrefs.SetInt(currentSongName() + "_highscore", score);
		}
	}

	string currentSongName() {
		return ((AudioSource) GameObject.FindObjectOfType(typeof(AudioSource))).clip.name;
	}


}
