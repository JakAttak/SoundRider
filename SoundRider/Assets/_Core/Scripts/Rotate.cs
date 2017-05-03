using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	[SerializeField] Vector3 speed;

	void Update () {
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + speed * Time.deltaTime);
	}
}
