using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayer : MonoBehaviour {

	[SerializeField] SteamVR_TrackedController _controller_left;
	[SerializeField] SteamVR_TrackedController _controller_right;
	private PrimitiveType _currentPrimitiveType = PrimitiveType.Sphere;

	private void OnEnable()
	{
		_controller_left.TriggerClicked += HandleTriggerClicked;
		_controller_left.PadClicked += HandlePadClicked;
	}

	private void OnDisable()
	{
		_controller_left.TriggerClicked -= HandleTriggerClicked;
		_controller_left.PadClicked -= HandlePadClicked;
	}

	#region Primitive Spawning
	private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{
		SpawnCurrentPrimitiveAtController();
	}

	private void SpawnCurrentPrimitiveAtController()
	{
		var spawnedPrimitive = GameObject.CreatePrimitive(_currentPrimitiveType);
		spawnedPrimitive.transform.position = transform.position;
		spawnedPrimitive.transform.rotation = transform.rotation;

		spawnedPrimitive.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		if (_currentPrimitiveType == PrimitiveType.Plane)
			spawnedPrimitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	}
	#endregion

	#region Primitive Selection
	private void HandlePadClicked(object sender, ClickedEventArgs e)
	{
		if (e.padY < 0)
			SelectPreviousPrimitive();
		else
			SelectNextPrimitive();
	}

	private void SelectNextPrimitive()
	{
		_currentPrimitiveType++;
		if (_currentPrimitiveType > PrimitiveType.Quad)
			_currentPrimitiveType = PrimitiveType.Sphere;
	}

	private void SelectPreviousPrimitive()
	{
		_currentPrimitiveType--;
		if (_currentPrimitiveType < PrimitiveType.Sphere)
			_currentPrimitiveType = PrimitiveType.Quad;
	}
	#endregion
}