using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchTemplate : MonoBehaviour
{
	public ComputeShader computeShader;
	
	private void Start()
	{
		computeShader.Dispatch(0, 1, 1, 1);
	}
}
