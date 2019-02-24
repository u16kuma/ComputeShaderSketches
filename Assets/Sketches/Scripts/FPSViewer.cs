using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSViewer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	int frameCount;
	float prevTime;

	void Start()
	{
		frameCount = 0;
		prevTime = 0.0f;
	}

	void Update()
	{
		++frameCount;
		float time = Time.realtimeSinceStartup - prevTime;

		if (time >= 0.5f)
		{
			text.text = string.Format("{0:0.00} fps", frameCount / time);

			frameCount = 0;
			prevTime = Time.realtimeSinceStartup;
		}
	}
}
