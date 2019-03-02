using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sketches : MonoBehaviour
{
	[SerializeField]
	private List<Sketch> sketchPrefabList;

	[SerializeField]
	private Button sketchButton;

	[SerializeField]
	private Transform sketchButtonParent;

	[SerializeField]
	private Button backButton;

	[SerializeField]
	private Canvas sketchButtonCanvas;

	private void Start()
	{
		Sketch sketch = null;
		backButton.onClick.AddListener(() =>
		{
			if (sketch != null)
			{
				sketchButtonCanvas.gameObject.SetActive(true);
				backButton.gameObject.SetActive(false);

				Destroy(sketch.gameObject);
				sketch = null;
			}
		});

		sketchButtonCanvas.gameObject.SetActive(true);
		backButton.gameObject.SetActive(false);

		foreach (var sketchPrefab in sketchPrefabList)
		{
			var button = Instantiate(sketchButton);
			button.gameObject.SetActive(true);
			button.name = sketchPrefab.name;
			button.transform.SetParent(sketchButtonParent, false);
			button.GetComponentInChildren<Text>().text = sketchPrefab.name;
			button.onClick.AddListener(() =>
			{
				sketchButtonCanvas.gameObject.SetActive(false);
				backButton.gameObject.SetActive(true);

				sketch = Instantiate(sketchPrefab);
			});
		}
	}
}
