using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketches : MonoBehaviour
{
	[SerializeField]
	private List<Sketch> sketchPrefabList;

	private void Start()
	{
		// とりあえずすべてインスタンス化
		foreach (var sketchPrefab in sketchPrefabList)
		{
			var sketch = Instantiate(sketchPrefab);
		}
	}
}
