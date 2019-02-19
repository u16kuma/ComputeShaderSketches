using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTracingMain : MonoBehaviour
{
	public ComputeShader rayTracingShader;
	public Texture skyboxTexture;
	public Light directionalLight;
	private RenderTexture target;
	private new Camera camera;
	private uint currentSample = 0;
	private Material addMaterial;

	private void Awake()
	{
		camera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (transform.hasChanged)
		{
			currentSample = 0;
			transform.hasChanged = false;
		}
	}

	private void SetShaderParameters()
	{
		rayTracingShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
		rayTracingShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
		rayTracingShader.SetTexture(0, "_SkyboxTexture", skyboxTexture);
		rayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));

		Vector3 lVec = directionalLight.transform.forward;
		rayTracingShader.SetVector("_DirectionalLight", new Vector4(lVec.x, lVec.y, lVec.z, directionalLight.intensity));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Render(destination);
	}

	private void Render(RenderTexture destination)
	{
		InitRenderTexture();

		SetShaderParameters();

		rayTracingShader.SetTexture(0, "Result", target);
		int threadGroupX = Mathf.CeilToInt(Screen.width / 8.0f);
		int threadGroupY = Mathf.CeilToInt(Screen.height / 8.0f);
		rayTracingShader.Dispatch(0, threadGroupX, threadGroupY, 1);

		if (addMaterial == null)
		{
			addMaterial = new Material(Shader.Find("Hidden/AddShader"));
		}
		addMaterial.SetFloat("_Sample", currentSample);
		Graphics.Blit(target, destination, addMaterial);
		currentSample++;
	}

	private void InitRenderTexture()
	{
		if (target == null ||
			target.width != Screen.width ||
			target.height != Screen.height)
		{
			if (target != null)
			{
				target.Release();
			}

			target = new RenderTexture(
				width: Screen.width,
				height: Screen.height,
				depth: 0,
				format: RenderTextureFormat.ARGBFloat,
				readWrite: RenderTextureReadWrite.Linear);
			target.enableRandomWrite = true;
			target.Create();
		}
	}
}
