using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCreator : MonoBehaviour {

	private static int NORM = 1;
	private static int TOPONLY = 2;
	private static int LENIENT = 3;
	private static int SIDEWAYS = 4;

	[SerializeField] int MODE = NORM;

	[SerializeField] bool BUILD_CLEANLY = false;

	[SerializeField] float camera_speed = 20f;

	[SerializeField] float cube_thickness = 0.5f;

	private Color[] colors = new Color[12];

	private float[] spectrum_averages = new float[12];
	private float[] spectrum_highs = new float[12];
	private int num_of_averages = 0;

	private float nextGeneration = -100f;

	private int max_coin_streak = 10;
	private int min_coin_streak = 3;
	private int coin_streak_left = 0;
	private int coin_streak_i;
	[SerializeField] GameObject coin_prefab;

	[SerializeField] PlayerController player;

	// Use this for initialization
	void Start() {
		for (int i = 0; i < colors.Length; i++) {
			float c = (1.0f / 12.0f) * i;
			colors[i] = new Color(c, c, c);
		}

		colors[0] = (Color) new Color32(207,240,158,255);
		colors[1] = (Color) new Color32(168,219,168,255);
		colors[2] = (Color) new Color32(121,189,154,255);
		colors[3] = (Color) new Color32(59,134,134,255);
		colors[4] = (Color) new Color32(11,72,107,255);
		colors[5] = (Color) new Color32(196,77,88,255);
		colors[6] = (Color) new Color32(255,107,107,255);
		colors[7] = (Color) new Color32(199,244,100,255);
		colors[8] = (Color) new Color32(78,205,196,255);
		colors[9] = (Color) new Color32(85,98,112,255);
		colors[10] = (Color) new Color32(213,222,217,255);
		colors[11] = (Color) new Color32(203,232,107,255);

		AudioProcessor processor = FindObjectOfType<AudioProcessor> ();
		processor.onSpectrum.AddListener (onSpectrum);

		nextGeneration = player.getZPosition();
		NewCoinStreak();

		player.setRunSpeed(camera_speed);
	}
	
	// Update is called once per frame
	void Update() {
		transform.position = transform.position + new Vector3(0, 0, camera_speed * Time.deltaTime);
	}

	void NewCoinStreak()
	{
		coin_streak_left = min_coin_streak + (int) (Random.value * (max_coin_streak - min_coin_streak));
		coin_streak_i = (int) (Random.value * 12);
	}

	void generatePlatform(float x, Color col, bool coin, float width = 1.0f, float height = 1.0f) {
 		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
 		float z = nextGeneration;
 		if (!BUILD_CLEANLY) {
 			z = transform.position.z;
 		}
 		cube.transform.position = new Vector3(x, 0, z);
 		cube.transform.localScale = new Vector3 (width, height, cube_thickness);
 		cube.GetComponent<Renderer>().material.color = col;
 		
 		if (coin) {
 			GameObject new_coin = Instantiate(coin_prefab, cube.transform);
 			new_coin.transform.localScale *= 0.5f;
 			new_coin.transform.position = new_coin.transform.position + new Vector3(0f, 0.5f + new_coin.transform.localScale.z, 0f);
 		}
 	}

 	void generatePlatforms(float[] spectrum) {
 		int n = 1;
 		if (BUILD_CLEANLY) {
 			n = (int) ((transform.position.z - nextGeneration) / cube_thickness); // we can need to fill multiple frames worth if the camera is moving too fast and we are using the build_cleanly setting
 		}
 		if (!BUILD_CLEANLY || transform.position.z > nextGeneration) {
 			for (int j = 0; j < n; j++) {
		 		for (int i = 0; i < spectrum.Length; ++i) {
					if (compareSpectrum(spectrum[i], i)) {
						generatePlatform(i - 5.5f, colors[i], (i == coin_streak_i));
					}
					float nx = -8.5f - i * 0.25f;
					if (i >= spectrum.Length / 2) {
						nx = 8.5f + (i - (spectrum.Length / 2)) * 0.25f;
					}
					generatePlatform(nx, colors[colors.Length - 1 - i], false, 0.25f, (spectrum[i] / spectrum_highs[i]) * 12.5f);
				}

				nextGeneration += cube_thickness;

				coin_streak_left -= 1;
				if (coin_streak_left < 1) {
					NewCoinStreak();
				}
			}
		}
 	}

 	//This event will be called every frame while music is playing
	void onSpectrum (float[] spectrum) {
		updateAverages(spectrum);

		generatePlatforms(spectrum);
	}

	void updateAverages(float[] spectrum) {
		for (int i = 0; i < spectrum.Length; i++) {
			if (spectrum[i] > spectrum_highs[i]) {
				spectrum_highs[i] = spectrum[i];
			}

			spectrum_averages[i] = ((spectrum_averages[i] * num_of_averages) + spectrum[i]) / (num_of_averages + 1);

		}
		num_of_averages += 1;
	}

	bool compareSpectrum(float check, int i) {
		if (MODE == TOPONLY) {
			return ((check - spectrum_averages[i]) >= (spectrum_highs[i] - check));
		} else if (MODE == LENIENT) {
			return ((check / spectrum_averages[i]) >= 0.75f);
		} else if (MODE == SIDEWAYS) {
			return ((check / spectrum_averages[1]) >= 0.75f);
		}

		return (check >= spectrum_averages[i]);
	}
}
