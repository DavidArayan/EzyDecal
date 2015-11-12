using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BatchedGeometry {
	
	[HideInInspector]
	public List<Vector3> vertices = new List<Vector3>();
	
	[HideInInspector]
	public List<int> indices = new List<int>();
	
	[HideInInspector]
	public List<Vector2> uv = new List<Vector2>();
	
	[HideInInspector]
	public Mesh finalMesh = new Mesh();
	
	public BatchedGeometry() {
		/*vertices = new List<Vector3>();
		indices = new List<int>();
		uv = new List<Vector2>();
		
		finalMesh = new Mesh();*/
	}
	
	public void batch() {
		finalMesh.Clear();
		
		finalMesh.vertices = vertices.ToArray();
		finalMesh.triangles = indices.ToArray();
		finalMesh.uv = uv.ToArray();
		finalMesh.RecalculateNormals();
		MeshCreator.calculateMeshTangents(ref finalMesh);
		
		vertices.Clear();
		indices.Clear();
		uv.Clear();
	}
}
