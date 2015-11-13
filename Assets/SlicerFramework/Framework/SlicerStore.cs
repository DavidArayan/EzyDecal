using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DecalFramework;

public class SlicerStore : MonoBehaviour {	
	[HideInInspector]
	public List<Mesh> slicedMeshes = new List<Mesh>();
	
	[HideInInspector]
	public bool isCollidable = false;
	
	[HideInInspector]
	public bool isRigidbody = false;
	
	[HideInInspector]
	public bool isRandom = false;
	
	[HideInInspector]
	public bool isConvex = false;
	
	[HideInInspector]
	public bool isHallow = false;
	
	private List<Vector3> debugIntPt = new List<Vector3>();
	
	private List<Material> materials = new List<Material>();
	
	public void slice(List<SlicerPlane> planes, Material subMat) {
		if (planes.Count == 0) {
			return;	
		}
		
		materials.Clear();
		
		materials.AddRange(gameObject.GetComponent<Renderer>().sharedMaterials);
		
		if (subMat != null) {
			if (!isHallow) {
				for (int i = 0; i < planes.Count; i++) {
					materials.Add(subMat);	
				}	
			}
			else {
				materials.Add(subMat);
			}
		}
		else {
			if (!isHallow) {
				for (int i = 0; i < planes.Count; i++) {
					materials.Add(gameObject.GetComponent<Renderer>().sharedMaterial);
				}
			}
			else {
				materials.Add(subMat);
			}
		}
		
		if (slicedMeshes.Count == 0) {
			MeshFilter filter = gameObject.GetComponent<MeshFilter>();
				
			if (filter == null) {
				// no filter? means no mesh, just return
				return;
			}
			
			slicedMeshes.Add(filter.sharedMesh);

            MeshModifiers.SliceMeshes(planes, slicedMeshes, gameObject, isHallow);
		}
		
		if (slicedMeshes.Count > 0) {
			// finalise all our stored mesh data
			for (int i = 0; i < slicedMeshes.Count; i++) {
				Mesh m = slicedMeshes[i];
				
				//m.RecalculateNormals();
				
				if (isCollidable) {
					m.RecalculateBounds();	
				}
				
				MeshCreator.calculateMeshTangents(ref m);
				
				m.Optimize();
			}	
		}
	}
	
	public Vector3 getRandomPosInMesh() {
		Mesh sm = gameObject.GetComponent<MeshFilter>().sharedMesh;
		
		int pos1 = Random.Range(0, sm.vertices.Length - 1);
		int pos2 = Random.Range(0, sm.vertices.Length - 1);
		
		return Vector3.Lerp(transform.TransformPoint(sm.vertices[pos1]), transform.TransformPoint(sm.vertices[pos2]), Random.value);
	}
	
	public void instantiate() {
		// perform GO Instantiation
		for (int i = 0; i < slicedMeshes.Count; i++) {
			GameObject obj = new GameObject();
			obj.name = gameObject.name + " Slice " + i;
			
			// set renderer
			MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
			renderer.sharedMaterials = materials.ToArray();
			
			// set filter
			MeshFilter filter = obj.AddComponent<MeshFilter>();
			filter.sharedMesh = slicedMeshes[i];
			
			if (isCollidable) {
				MeshCollider collider = obj.AddComponent<MeshCollider>();
				collider.convex = true;
			}
			
			if (isRigidbody) {
				obj.AddComponent<Rigidbody>();	
			}
			
			// set transform
			obj.transform.position = transform.position;
			obj.transform.localScale = transform.localScale;
			obj.transform.rotation = transform.rotation;
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(1,0,0,1);
		
		Vector3 size = new Vector3(0.1f,0.1f,0.1f);
		
		for (int i = 0; i < debugIntPt.Count; i++) {
			Gizmos.DrawCube(debugIntPt[i], size);
		}
	}
	
}
