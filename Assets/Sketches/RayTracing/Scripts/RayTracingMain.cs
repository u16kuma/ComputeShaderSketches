using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere
{
	public Vector3 position;
	public float radius;
	public Vector3 albedo;
	public Vector3 specular;
}

public class RayTracingMain : MonoBehaviour
{
	public ComputeShader rayTracingShader;
	public Texture skyboxTexture;
	public Light directionalLight;
	private RenderTexture target;
	private new Camera camera;
	private uint currentSample = 0;
	private Material addMaterial;
	public Vector2 sphereRadius = new Vector2(3.0f, 8.0f);
	public uint spheresMax = 100;
	public float SpherePlacementRadius = 100.0f;
	private ComputeBuffer sphereBuffer;
	private float renderTextureScale = 0.2f;

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

	public void Setup()
	{
		if (sphereBuffer != null)
		{
			sphereBuffer.Release();
		}

		currentSample = 0;
		SetUpScene();
	}

	private void OnEnable()
	{
		currentSample = 0;
		SetUpScene();
	}

	private void OnDisable()
	{
		if (sphereBuffer != null)
		{
			sphereBuffer.Release();
		}
	}

	private void SetUpScene()
	{
		List<Sphere> spheres = new List<Sphere>();

		for (int i = 0; i < spheresMax; i++)
		{
			Sphere sphere = new Sphere();

			sphere.radius = Mathf.Lerp(sphereRadius.x, sphereRadius.y, Random.Range(0.0f, 1.0f));
			Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
			sphere.position = new Vector3(randomPos.x, sphere.radius, randomPos.y);

			foreach (Sphere other in spheres)
			{
				float minDist = sphere.radius + other.radius;
				if (Vector3.SqrMagnitude(sphere.position - other.position) < minDist * minDist)
				{
					goto SkipSphere;
				}
			}

			Color color = Random.ColorHSV();
			bool metal = Random.value < 0.5f;
			sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
			sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;

			spheres.Add(sphere);

		SkipSphere:
			continue;
		}

		sphereBuffer = new ComputeBuffer(spheres.Count, 40);
		sphereBuffer.SetData(spheres);
	}

	private void SetShaderParameters()
	{
		rayTracingShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
		rayTracingShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
		rayTracingShader.SetTexture(0, "_SkyboxTexture", skyboxTexture);
		//rayTracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));

		Vector3 lVec = directionalLight.transform.forward;
		rayTracingShader.SetVector("_DirectionalLight", new Vector4(lVec.x, lVec.y, lVec.z, directionalLight.intensity));

		rayTracingShader.SetBuffer(0, "_Spheres", sphereBuffer);
	}
	
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Time.frameCount % 3 == 0)
		{
			Render(destination);
		}
	}

	private void Render(RenderTexture destination)
	{
		InitRenderTexture();

		SetShaderParameters();

		rayTracingShader.SetTexture(0, "Result", target);
		int threadGroupX = Mathf.CeilToInt((int)(Screen.width * renderTextureScale) / 8.0f);
		int threadGroupY = Mathf.CeilToInt((int)(Screen.height * renderTextureScale) / 8.0f);
		rayTracingShader.Dispatch(0, threadGroupX, threadGroupY, 1);
		
		if (addMaterial == null)
		{
			addMaterial = new Material(Shader.Find("Hidden/AddShader"));
		}
		addMaterial.SetFloat("_Sample", currentSample);
		//Graphics.Blit(target, destination, addMaterial);
		Graphics.Blit(target, destination);
		currentSample++;
	}

	private void InitRenderTexture()
	{
		if (target == null ||
			target.width != (int)(Screen.width * renderTextureScale) ||
			target.height != (int)(Screen.height * renderTextureScale))
		{
			if (target != null)
			{
				target.Release();
			}

			target = new RenderTexture(
				width: (int)(Screen.width * renderTextureScale),
				height: (int)(Screen.height * renderTextureScale),
				depth: 0,
				format: RenderTextureFormat.ARGBFloat,
				readWrite: RenderTextureReadWrite.Linear);
			target.enableRandomWrite = true;
			target.Create();
		}
	}
}
