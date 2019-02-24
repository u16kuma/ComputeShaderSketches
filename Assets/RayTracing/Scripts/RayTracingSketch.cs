using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayTracingSketch : Sketch
{
	[SerializeField]
	private Rotate cameraRotateSpeed;

	[SerializeField]
	private Slider cameraRotateSpeedSlider;

	private void Start()
	{
		cameraRotateSpeedSlider.value = cameraRotateSpeed.speed;
		cameraRotateSpeedSlider.onValueChanged.AddListener((value) =>
		{
			cameraRotateSpeed.speed = value;
		});
	}
}
