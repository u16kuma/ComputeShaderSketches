using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoidsMain))]
public class BoidsRenderer : MonoBehaviour
{
	public Vector3 ObjectScale = new Vector3(0.1f, 0.2f, 0.5f);

	public BoidsMain BoidsMain;

	public Mesh InstanceMesh;
	public Material InstanceRenderMaterial;

	private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
	private ComputeBuffer argsBuffer;

	private void Update()
	{
		RenderInstancedMesh();
	}

	private void OnEnable()
	{
		argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
	}

	private void OnDisable()
	{
		if (argsBuffer != null)
		{
			argsBuffer.Release();
			argsBuffer = null;
		}
	}

	private void RenderInstancedMesh()
	{
		if (InstanceRenderMaterial == null || 
			BoidsMain == null ||
			!SystemInfo.supportsInstancing)
		{
			return;
		}

		// 指定したメッシュのインデックス数を取得
		uint numIndices = (InstanceMesh != null) ? (uint)InstanceMesh.GetIndexCount(0) : 0;
		args[0] = numIndices;

		args[1] = (uint)BoidsMain.MaxObjectNum;
		argsBuffer.SetData(args);

		// Boidデータを格納したバッファをマテリアルにセット
		InstanceRenderMaterial.SetBuffer("_BoidDataBuffer", BoidsMain.BoidDataBuffer);
		// Boidオブジェクトスケールをセット
		InstanceRenderMaterial.SetVector("_ObjectScale", ObjectScale);

		// 境界領域を定義
		var bounds = new Bounds(
			center: BoidsMain.WallCenter,
			size: BoidsMain.WallSize);

		// メッシュをGPUインスタンシングして描画
		Graphics.DrawMeshInstancedIndirect(
			mesh: InstanceMesh,
			submeshIndex: 0,
			material: InstanceRenderMaterial,
			bounds: bounds,
			bufferWithArgs: argsBuffer);
	}
}
