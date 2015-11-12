using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MeshBatchFiller {
	
	[HideInInspector]
	public List<Vector3> vertices;
	
	[HideInInspector]
	public List<int> indices;
	
	[HideInInspector]
	public List<Vector2> uv;
	
	[HideInInspector]
	public Material material;
	
	public void reset() {
		vertices = null;
		indices = null;
		uv = null;
		material = null;
	}
}
