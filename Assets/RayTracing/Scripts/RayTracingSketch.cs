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

	[SerializeField]
	private RayTracingMain main;

	[SerializeField]
	private Button setupButton;

	[SerializeField]
	private Button switchButton;

	[SerializeField]
	private GameObject menuRoot;

	[SerializeField]
	private Slider textureSizeSlider;

	[SerializeField]
	private Slider frameIntervalSlider;

	[SerializeField]
	private Slider sphereNumSlider;

	[SerializeField]
	private Slider bounceNumSlider;

	private void Start()
	{
		cameraRotateSpeedSlider.onValueChanged.AddListener((value) =>
		{
			cameraRotateSpeed.speed = value;
		});
		cameraRotateSpeedSlider.value = cameraRotateSpeedSlider.value;
		cameraRotateSpeed.speed = cameraRotateSpeedSlider.value;

		setupButton.onClick.AddListener(() =>
		{
			main.Setup();
		});

		switchButton.onClick.AddListener(() =>
		{
			menuRoot.SetActive(!menuRoot.activeSelf);
		});
		
		textureSizeSlider.onValueChanged.AddListener((value) =>
		{
			main.SetRenderTextureScale(value / 10.0f);
		});
		textureSizeSlider.value = textureSizeSlider.value;

		frameIntervalSlider.onValueChanged.AddListener((value) =>
		{
			main.SetFrameInterval((int)value);
		});
		main.SetFrameInterval((int)frameIntervalSlider.value);

		sphereNumSlider.onValueChanged.AddListener((value) =>
		{
			main.SetSphereNum((int)value);
		});
		main.SetSphereNum((int)sphereNumSlider.value);

		bounceNumSlider.onValueChanged.AddListener((value) =>
		{
			main.SetBounceNum((int)value);
		});
		main.SetBounceNum((int)bounceNumSlider.value);
	}
}
