﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float speed;

	private void Update()
	{
		var euler = transform.localEulerAngles;
		euler.y += Time.deltaTime * speed;
		transform.localEulerAngles = euler;
	}
}
