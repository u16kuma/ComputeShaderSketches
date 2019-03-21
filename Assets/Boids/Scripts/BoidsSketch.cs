using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BoidsSketch : Sketch
{
	[SerializeField]
	private Slider rotateSpeedSlider;

	[SerializeField]
	private Slider objectNumSlider;

	[SerializeField]
	private Rotate rotate;

	[SerializeField]
	private BoidsMain boidsMain;

	private void Start()
	{
		rotateSpeedSlider.onValueChanged.AddListener(value =>
		{
			rotate.speed = value;
		});
		rotate.speed = rotateSpeedSlider.value;

		objectNumSlider.onValueChanged.AddListener(value =>
		{
			boidsMain.MaxObjectNum = (int)value;
		});
		boidsMain.MaxObjectNum = (int)objectNumSlider.value;
	}
}
