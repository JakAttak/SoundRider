using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private float lane;
	private int max_lane = 3;

	private Rigidbody rb;

	private bool jump;
	public int jumps;
	private int jumps_allowed = 2;
	private float jump_force = 5f;

	private float start_height;

	private Tween slider = new Tween(0.125f, 2, 2);

	private GameManager game; 

	void Start () {
		game = GameManager.getActive();

		rb = GetComponent<Rigidbody>();
		start_height = transform.position.y;
	}
	
	void Update () {
		if (Input.GetKeyDown("space")) {
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
    	lane = slider.getValue(true);

    	slider.start(lane, lane + dir);
    }

    void FixedUpdate() {
    	if (transform.position.y <= start_height + 0.01f && rb.velocity.y < 0) {
    		jumps = 0;
    	}

    	if (jump && jumps < jumps_allowed) {
            rb.velocity += new Vector3(0f, jump_force, 0f);
            jump = false;
            jumps += 1;
    	}

    	if (rb.velocity.y < 0 && rb.velocity.y > Physics.gravity.y * 1.25f) {
    		rb.velocity -= new Vector3(0f, 1f, 0f);
    	}

    	lane = slider.getValue();
		transform.position = new Vector3(-1.5f + lane, transform.position.y, transform.position.z); // lock to our lane

		float mult = game.getSpeed();

		if (Input.GetKey("up")) {
			mult *= 2.5f;
		} else if (Input.GetKey("down")) {
			mult *= -2.5f;
		}
		
		rb.MovePosition(transform.position + Vector3.forward * mult * Time.deltaTime);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Coin") {
			game.collectCoin();
			Destroy(col.gameObject);
		}
	}

}
