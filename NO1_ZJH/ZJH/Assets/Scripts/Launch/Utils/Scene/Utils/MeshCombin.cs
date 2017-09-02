using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [ExecuteInEditMode]
public class MeshCombin : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {
		
	}

	public bool rootChanged = false;

	public void Combin()
	{
		int i = 0;
		int j = 0;
		Vector3[] combinVeices;
		Vector3[] combinNormals;
		Vector4[] combinTangents;
		Vector2[] combinUVs1;
		Vector2[] combinUVs2;
		Vector2[] combinUVs3;
		List<int[]> trisList = new List<int[]>();

		List<Material> mtls = new List<Material> ();


		int combinVertexCount = 0;
		int f = 0;
		for (i = 0; i < transform.childCount || f == 0; i++)
		{
			MeshFilter mf = null;
			MeshRenderer mr = null;
			if (f == 0) {
				MeshFilter rMf = gameObject.GetComponent<MeshFilter> ();
				if (rMf != null) {
					mf = rMf;
				}
				MeshRenderer rMr = gameObject.GetComponent<MeshRenderer> ();
				if (rMr != null) {
					mr = rMr;
				}
				if (rMf != null && rMr != null)
					i = 0;
				f = 1;
			}

			if (mf == null)
				mf = transform.GetChild (i).GetComponent<MeshFilter>();
			if (mf != null && mf.sharedMesh != null) 
			{
				Mesh mesh = mf.sharedMesh;

				int[] triangles = mesh.triangles;

				// change tri offset
				for (j = 0; j < triangles.Length; j++) 
				{
					triangles [j] += combinVertexCount;
				}
				trisList.Add (triangles);

				combinVertexCount += mesh.vertexCount;
			}

			if (mr == null)
				mr = transform.GetChild (i).GetComponent<MeshRenderer> ();
			if (mr != null && mr.sharedMaterial != null)
				mtls.Add (mr.sharedMaterial);
		}

		combinVeices = new Vector3[combinVertexCount];
		combinNormals = new Vector3[combinVertexCount];
		combinTangents = new Vector4[combinVertexCount];
		combinUVs1 = new Vector2[combinVertexCount];
		combinUVs2 = new Vector2[combinVertexCount];
		combinUVs3 = new Vector2[combinVertexCount];

		int vertCount = 0;
		f = 0;
		for (i = 0; i < transform.childCount || f == 0; i++) 
		{
			MeshFilter mf = null;
			MeshFilter rMf = null;
			GameObject ins = null;
			if (f == 0) {
				rMf = gameObject.GetComponent<MeshFilter> ();
				if (rMf != null) {
					mf = rMf;
					ins = gameObject;
					i = 0;
				}
				f = 1;
			}
			if (ins == null)
				ins = transform.GetChild (i).gameObject;
			if (mf == null)
				mf = ins.GetComponent<MeshFilter>();
			
			if (mf != null && mf.sharedMesh != null) 
			{
				Mesh mesh = mf.sharedMesh;
				Vector3[] vertices = mesh.vertices;
				Vector3[] normals = mesh.normals;
				Vector4[] tangents = mesh.tangents;
				Vector2[] uvs1 = mesh.uv;
				Vector2[] uvs2 = mesh.uv2;
				Vector2[] uvs3 = mesh.uv3;
				if (uvs2.Length < 1)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
					uvs2 = uvs1;

				if (uvs3.Length < 1)
					uvs3 = uvs2;

				if (normals.Length < 1)
					Debug.LogError ("请计算模型的Normals:" + mf.name);

				if (tangents.Length < 1)
					Debug.LogError ("请计算模型的Tangents:" + mf.name);

				int[] triangles = mesh.triangles;

				for (j = 0; j < mesh.vertexCount; j++)
				{
					Vector3 v = vertices [j];
					v = ins.transform.localToWorldMatrix.MultiplyPoint (v);
					v = this.transform.worldToLocalMatrix.MultiplyPoint (v);
					combinVeices[vertCount] = v;
					if (rMf == null || (rMf != null && rootChanged == false)) {
						combinNormals [vertCount] = ins.transform.localToWorldMatrix.MultiplyVector (normals [j]);
						combinTangents [vertCount] = ins.transform.localToWorldMatrix.MultiplyVector (tangents [j]);

					} else {
						combinNormals [vertCount] = normals [j];
						combinTangents [vertCount] = tangents [j];
					}
					combinUVs1[vertCount] = uvs1[j];
					combinUVs2 [vertCount] = uvs2[j];

					// 第二层合并复用uv3
					if (rMf == null || (rMf != null && rootChanged == false)) {
						combinUVs3 [vertCount] = uvs2 [j];
					}
					else
						combinUVs3 [vertCount] = uvs3 [j];
					vertCount++;
				}

				if (rMf != null)
					rootChanged = true;
			}
		}

		// create combin mesh
		Mesh combinMesh = new Mesh();
		combinMesh.vertices = combinVeices;
		combinMesh.normals = combinNormals;
		combinMesh.tangents = combinTangents;
		combinMesh.uv = combinUVs1;
		combinMesh.uv2 = combinUVs2;
		combinMesh.uv3 = combinUVs3;

		combinMesh.subMeshCount = trisList.Count;

		for (i = 0; i < trisList.Count; i++) {
			combinMesh.SetTriangles (trisList[i], i);
		}
		combinMesh.UploadMeshData (true);


		MeshFilter combinMf = this.gameObject.GetComponent<MeshFilter>();
		if (combinMf == null)
			combinMf = this.gameObject.AddComponent<MeshFilter> ();
		combinMf.sharedMesh = combinMesh;

		MeshRenderer combinMr = this.gameObject.GetComponent<MeshRenderer> ();
		if (combinMr == null)
			combinMr = this.gameObject.AddComponent<MeshRenderer> ();
		combinMr.sharedMaterials = mtls.ToArray ();
	}
}
