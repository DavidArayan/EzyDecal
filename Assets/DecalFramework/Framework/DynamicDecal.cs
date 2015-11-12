using UnityEngine;
using UnityEditor;
using System.Collections;

public class DynamicDecal : MonoBehaviour {
	public Vector3 offset;
	
	public Material[] materials;
	
	[HideInInspector]
	public bool updateEnabled = true;
	
	[HideInInspector]
	public bool rtUpdateEnabled = false;
	
	[HideInInspector]
	public int subdivisions = 2;
	
	[HideInInspector]
	public GameObject projection;
	
	[HideInInspector]
	public MeshFilter projectionFilter;
	
	[HideInInspector]
	public MeshCreator mesh;
	
	[HideInInspector]
	public SceneData data;
	
	[HideInInspector]
	public bool collInEditor = false;
	
	public void create(SceneData data) {
		this.data = data;
		
		mesh = new MeshCreator();
		
		if (projection == null) {	
			projection = new GameObject();
			projection.name = gameObject.name + "_proj";
			
			projectionFilter = (MeshFilter)projection.AddComponent(typeof(MeshFilter));
			projection.AddComponent(typeof(MeshRenderer));
			
			projectionFilter.sharedMesh = new Mesh();
			
			projection.transform.position = offset;
			
			projection.transform.parent = transform.parent;
			
			//projection.hideFlags = HideFlags.NotEditable;
		}
		
		mesh.fill(ref projectionFilter);
		
		subdivisions = 2;
	}
	
	public void clear(SceneData data) {
		create(data);
	}
	
	void OnDrawGizmos() {
		if (rtUpdateEnabled) {
			projection.transform.position = offset;
			
			if (materials != null) {
				projection.GetComponent<Renderer>().sharedMaterials = materials;
			}
			
			updateMesh();	
		}
		
		if (Selection.Contains(gameObject) || Selection.Contains(projection)) {
						
			Vector3 size = new Vector3(0.2f,0.2f,0.2f);
			
			Vector3[] verts = mesh.getVertices();
			
			Vector3 tDir = transform.TransformDirection(-Vector3.up) * transform.localScale.y;
			
			for (int i = 0; i < verts.Length; i++) {
				Gizmos.color = new Color(1,0,0,1);
				Vector3 tPoint = transform.TransformPoint(verts[i]);
				
				Gizmos.DrawCube(tPoint, size);
				
				Gizmos.color = new Color(0,1,0,0.2f);
				
				Gizmos.DrawLine(tPoint, tPoint + tDir);
			}
			
			Vector3[] tVerts = mesh.getTransVert();
			
			Gizmos.color = new Color(0,1,0,1);
			
			for (int i = 0; i < tVerts.Length; i++) {
				Gizmos.DrawCube(tVerts[i], size);
			}	
		}
	}
	
	public void updateMesh() {
		if (data != null && data.vertexCount() > 0) {
			Vector3[] pt = mesh.getVertices();
			Vector3[] tpt = mesh.getTransVert();
			
			int length = pt.Length;
			
			Vector3 r = Vector3.zero;
			
			for (int i = 0; i < length; i++) {
				if (data.intersectPt(transform.TransformPoint(pt[i]), (transform.TransformDirection(-Vector3.up) * transform.localScale.y), ref r)) {
					tpt[i] = r;
				}
				else {
					tpt[i] = transform.TransformPoint(pt[i]) + transform.TransformDirection(-Vector3.up) * transform.localScale.y;
				}	
			}
			
			mesh.fill(ref projectionFilter);
		}
	}
	
	public void selectObj() {
		GameObject[] gos = new GameObject[2];
		gos[0] = gameObject;
		gos[1] = projection;
		
		Selection.objects = gos;
	}
	
	public void destroy() {
		DestroyImmediate(projection);
		DestroyImmediate(gameObject);
	}
	
	public bool isUpdateEnabled() {
		return updateEnabled;	
	}
	
	public bool isRtUpdateEnabled() {
		return rtUpdateEnabled;	
	}
	
	public void setUpdateEnabled(bool enabled) {
		updateEnabled = enabled;
	}
	
	public void setRtUpdateEnabled(bool enabled) {
		rtUpdateEnabled = enabled;
	}
	
	public void subdivideUp() {
		
		subdivisions *= 2;
		
		mesh.subdivide(subdivisions);
	}
	
	public void subdivideDown() {
		if (subdivisions == 2) {
			return;	
		}
		
		subdivisions /= 2;
		
		mesh.subdivide(subdivisions);
	}
}
