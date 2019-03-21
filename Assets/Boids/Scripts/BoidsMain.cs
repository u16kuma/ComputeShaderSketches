using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class BoidsMain : MonoBehaviour
{
	[System.Serializable]
	public struct BoidData
	{
		public Vector3 position;
		public Vector3 velocity;
	}

	const int SIMULATION_BLOCK_SIZE = 64;

	[Header("Boids Parameters")]
	[Range(256, 32768)]
	public int MaxObjectNum = 16384;

	// 結合を適用する他の個体との半径
	public float CohesionNeighborhoodRadius = 2.0f;
	// 整列を適用する他の個体との半径
	public float AlignmentNeighborhoodRadius = 2.0f;
	// 分離を適用する他の個体との半径
	public float SeparateNeighborhoodRadius = 1.0f;

	// 速度の最大値
	public float MaxSpeed = 5.0f;
	// 操舵力の最大値
	public float MaxSteerForce = 0.5f;

	// 結合する力の重み
	public float CohesionWeight = 1.0f;
	// 整理する力の重み
	public float AlignmentWeight = 1.0f;
	// 分離する力の重み
	public float SeparateWeight = 3.0f;

	// 壁を避ける力の重み
	public float AvoidWallWeight = 10.0f;

	// 壁の中心座標
	public Vector3 WallCenter = Vector3.zero;
	// 壁のサイズ
	public Vector3 WallSize = new Vector3(32.0f, 32.0f, 32.0f);

	[Header("Built-in Resources")]
	public ComputeShader BoidsCS;

	// Boidの操舵力(Force)を格納したバッファ
	private ComputeBuffer boidForceBuffer;
	// Boidの基本データ(速度、位置)を格納したバッファ
	private ComputeBuffer boidDataBuffer;
	public ComputeBuffer BoidDataBuffer { get { return boidDataBuffer; } }

	private void Update()
	{
		Simulation();
	}

	private void OnEnable()
	{
		InitBuffer();
	}

	private void OnDisable()
	{
		ReleaseBuffer();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(WallCenter, WallSize);
	}

	private void InitBuffer()
	{
		// バッファを初期化
		boidDataBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(BoidData)));
		boidForceBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(Vector3)));

		// Boidデータ、Forceバッファを初期化
		var forces = new Vector3[MaxObjectNum];
		var boidData = new BoidData[MaxObjectNum];
		for (int i = 0; i < MaxObjectNum; i++)
		{
			forces[i] = Vector3.zero;
			boidData[i].position = Random.insideUnitSphere * 1.0f;
			boidData[i].velocity = Random.insideUnitSphere * 0.1f;
		}

		boidForceBuffer.SetData(forces);
		boidDataBuffer.SetData(boidData);
		forces = null;
		boidData = null;
	}

	private void ReleaseBuffer()
	{
		if (boidDataBuffer != null)
		{
			boidDataBuffer.Release();
			boidDataBuffer = null;
		}

		if (boidForceBuffer != null)
		{
			boidForceBuffer.Release();
			boidForceBuffer = null;
		}
	}

	private void Simulation()
	{
		ComputeShader cs = BoidsCS;
		int id = -1;

		// スレッドグループの数を求める
		int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);

		// 操舵力を計算
		id = cs.FindKernel("ForceCS");
		cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
		cs.SetFloat("_CohesionNeighborhoodRadius", CohesionNeighborhoodRadius);
		cs.SetFloat("_AlignmentNeighborhoodRadius", AlignmentNeighborhoodRadius);
		cs.SetFloat("_SeparateNeighborhoodRadius", SeparateNeighborhoodRadius);
		cs.SetFloat("_MaxSpeed", MaxSpeed);
		cs.SetFloat("_MaxSteerForce", MaxSteerForce);
		cs.SetFloat("_CohesionWeight", CohesionWeight);
		cs.SetFloat("_AlignmentWeight", AlignmentWeight);
		cs.SetFloat("_SeparateWeight", SeparateWeight);
		cs.SetVector("_WallCenter", WallCenter);
		cs.SetVector("_WallSize", WallSize);
		cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);
		cs.SetBuffer(id, "_BoidDataBufferRead", boidDataBuffer);
		cs.SetBuffer(id, "_BoidForceBufferWrite", boidForceBuffer);
		cs.Dispatch(id, threadGroupSize, 1, 1);

		// 操舵力から、速度と位置を計算
		id = cs.FindKernel("IntegrateCS");
		cs.SetFloat("_DeltaTime", Time.deltaTime);
		cs.SetBuffer(id, "_BoidForceBufferRead", boidForceBuffer);
		cs.SetBuffer(id, "_BoidDataBufferWrite", boidDataBuffer);
		cs.Dispatch(id, threadGroupSize, 1, 1);
	}
}
