#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BatchBuild
{
	public static void Build()
	{
		EditorApplication.Exit(0);
	}
}
#endif