using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween {

	private float start_time;
	private float total_time;

	private float start_value;
	private float finish_value;

	public Tween(float tt, float sv, float fv) {
		start_value = sv;
		finish_value = fv;
		total_time = tt;
	}

	public Tween(float tt) {
		total_time = tt;
	}

	public void start() {
		start_time = Time.time;
	}

	public void start(float sv, float fv) {
		start_value = sv;
		finish_value = fv;
		start();
	}

	public void setStartValue(float v) {
		start_value = v;
	}

	public void setFinishValue(float v) {
		finish_value = v;
	}

	public void setDuration(float d) {
		total_time = d;
	}

	public float getValue() {
		if (start_time == null) {
			return start_value;
		}

		return Mathf.Lerp(start_value, finish_value, Mathf.Min(elapsedTime() / total_time, 1f));
	}

	public bool isFinished() {
		return (start_time == null || elapsedTime() >= total_time);
	}

	public void finish() {
		start_time = Time.time - total_time;
	}

	private float elapsedTime() {
		return Time.time - start_time;
	}
}
