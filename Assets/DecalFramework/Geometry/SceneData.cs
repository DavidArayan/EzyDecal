using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SceneData : MonoBehaviour {
	
	public int maxTriCount = 24;
	
	[HideInInspector]
	public bool gizmosEnabled = false;
	
	[HideInInspector]
	public bool gridGizmosEnabled = false;
	
	[HideInInspector]
	public List<OptiMesh> vertexList;
	
	[HideInInspector]
	public int rawCount = 0;
	
	[HideInInspector]
	public float length = 0;
	
	[HideInInspector]
	public float width = 0;
	
	[HideInInspector]
	public float height = 0;
	
	[HideInInspector]
	public Vector3 center;
	
	[HideInInspector]
	public static int CELL_SIZE = 8;
	
	[HideInInspector]
	public BroadphaseGrid grid;
	
	void Awake() {
		if (grid == null) {
			//Debug.Log("enters here");
			grid = ScriptableObject.CreateInstance<BroadphaseGrid>();	
		}
		
		if (vertexList == null) {
			vertexList = new List<OptiMesh>();	
		}
	}
	
	public void generateData() {
		vertexList.Clear();
		// get all objects with mesh rendering enabled
		MeshFilter[] filters = (MeshFilter[])GameObject.FindObjectsOfType(typeof(MeshFilter));
		
		if (filters != null) {
			for (int i = 0; i < filters.Length; i++) {
				if (filters[i].GetComponent<Renderer>() == null) {
					continue;
				}
				// check if renderer is actually enabled before batching data
				if (!filters[i].GetComponent<Renderer>().enabled) {
					continue;
				}
				
				// if so, continue with batching process
				Mesh mesh = filters[i].sharedMesh;
				
				if (filters[i].gameObject.isStatic) {
					
					if (maxTriCount <= 0 || (maxTriCount * 3) >= mesh.triangles.Length) {
						rawCount += mesh.vertexCount;
					
						OptiMesh opt = ScriptableObject.CreateInstance<OptiMesh>();
						//OptiMesh opt = new OptiMesh();
						//opt.onEnable();
						
						opt.objRef = filters[i].gameObject;
						
						//OptiMesh opt = new OptiMesh(filters[i].gameObject);
						
						for (int j = 0; j < mesh.triangles.Length; j+=3) {
							Vector3 p1 = mesh.vertices[mesh.triangles[j + 0]];
			    			Vector3 p2 = mesh.vertices[mesh.triangles[j + 1]];
			    			Vector3 p3 = mesh.vertices[mesh.triangles[j + 2]];
							
							opt.addTriangle(p1,p2,p3);
						}
						
						opt.transform();
						
						vertexList.Add(opt);		
					}
				}
			}
		}
		
		// nothing was added, check your shit
		if (vertexList.Count == 0) {
			return;	
		}
		
		// find lowest and highest points
		float lowestX = float.PositiveInfinity;
		float highestX = float.NegativeInfinity;
		float lowestY = float.PositiveInfinity;
		float highestY = float.NegativeInfinity;
		float lowestZ = float.PositiveInfinity;
		float highestZ = float.NegativeInfinity;	
		
		for (int i = 0; i < vertexList.Count; i++) {
			OptiMesh om = vertexList[i];
			
			// x value
			if (om.lowestX < lowestX) {
				lowestX = om.lowestX;
			}
			
			if (om.highestX > highestX) {
				highestX = om.highestX;
			}
			
			// y value
			if (om.lowestY < lowestY) {
				lowestY = om.lowestY;
			}
			
			if (om.highestY > highestY) {
				highestY = om.highestY;
			}
			
			// z value
			if (om.lowestZ < lowestZ) {
				lowestZ = om.lowestZ;
			}
			
			if (om.highestZ > highestZ) {
				highestZ = om.highestZ;
			}
		}
		
		// offset our values by a little bit to ensure everything fits in grid perfectly
		/*lowestX -= 10.0f;
		highestX += 10.0f;
		lowestY -= 10.0f;
		highestY += 10.0f;
		lowestZ -= 10.0f;
		highestZ += 10.0f;*/
		
		// build the axis aligned box around the scanned level
		center = new Vector3((lowestX + highestX) / 2, (lowestY + highestY) / 2, (lowestZ + highestZ) / 2);
		
		length = Mathf.Abs(lowestX - highestX);
		width = Mathf.Abs(lowestZ - highestZ);
		height = Mathf.Abs(lowestY - highestY);
		
		int max = (int)Mathf.Max(Mathf.Max(length,width),height);
		
		Vector3 halfsize = new Vector3(max/2, max/2, max/2);
			
		Vector3 offset = new Vector3(
			center.x - halfsize.x + (CELL_SIZE / 2), 
			center.y - halfsize.y + (CELL_SIZE / 2), 
			center.z - halfsize.z + (CELL_SIZE / 2));
		
		//Debug.Log("Length: " + ((max + CELL_SIZE - 1) / CELL_SIZE) + " @ " + CELL_SIZE);
		
		grid.setup(max, CELL_SIZE, vertexList, offset);
	}
	
	void OnDrawGizmos() {
		Vector3 gizmoSize = new Vector3(0.2f,0.2f,0.2f);
		
		if (gridGizmosEnabled && vertexList.Count > 0) {
							
			float max = Mathf.Max(Mathf.Max(length,width),height);
			
			int rows = (int)((max + CELL_SIZE - 1) / CELL_SIZE);
			
			Vector3 halfsize = new Vector3(max / 2, max / 2, max / 2);
			
			Vector3 offset = new Vector3(
				center.x - halfsize.x + (CELL_SIZE / 2), 
				center.y - halfsize.y + (CELL_SIZE / 2), 
				center.z - halfsize.z + (CELL_SIZE / 2));
			
			for (int x = 0; x < rows; x++) {
				for (int y = 0; y < rows; y++) {
					for (int z = 0; z < rows; z++) {
						Vector3 location = new Vector3(
							(x * CELL_SIZE) + offset.x, 
							(y * CELL_SIZE) + offset.y,
							(z * CELL_SIZE) + offset.z);
						
						Gizmos.color = new Color(0,0,0,0.05f);
						Gizmos.DrawWireCube(location,new Vector3(CELL_SIZE, CELL_SIZE, CELL_SIZE));
					}
				}
			}	
		}
		
		if (gizmosEnabled && vertexList.Count > 0) {
			
			for (int i = 0; i < vertexList.Count; i++) {
				OptiMesh ms = vertexList[i];
				
				TriangleData[] dat = ms.getTriangleData();
				
				for (int j = 0; j < dat.Length; j++) {
					Vector3[] tPoints = dat[j].getTransformedPoints();
					Vector3 tCenter = dat[j].getTransCenter();
					
					Gizmos.color = new Color(0,1,0,1);
					Gizmos.DrawCube(tCenter,gizmoSize);
					
					Gizmos.color = new Color(1,0,0,1);
					Gizmos.DrawCube(tPoints[0],gizmoSize);
					Gizmos.DrawCube(tPoints[1],gizmoSize);
					Gizmos.DrawCube(tPoints[2],gizmoSize);
					
					Gizmos.color = new Color(0,0,1,1);
					Gizmos.DrawLine(tPoints[0],tPoints[1]);
					Gizmos.DrawLine(tPoints[1],tPoints[2]);
					Gizmos.DrawLine(tPoints[2],tPoints[0]);
				}
			}
		}
	}
	
	public void update() {
		for (int i = 0; i < vertexList.Count; i++) {
			if (vertexList[i] != null && vertexList[i].getObjRef() != null) {
				vertexList[i].transform();		
			}
			else {
				vertexList.RemoveAt(i);	
			}
		}
	}
	
	public void clear() {
		vertexList.Clear();
		if (grid != null) {
			grid.clear();	
		}
		rawCount = 0;
	}
	
	public int vertexCount() {
		int count = 0;
		
		for (int i = 0; i < vertexList.Count; i++) {
			count += vertexList[i].getVertexCount();
		}
		
		return count;
	}
	
	public Vector3 closestPt(Vector3 vec, Vector3 direction) {
		return grid.closestPt(vec, direction);	
	}
	
	public bool intersectPt(Vector3 vec, Vector3 direction, ref Vector3 r) {
		return grid.intersectPt(vec, direction, ref r);	
	}
	
	public List<TriangleData> getTrianglesInOOBB(OOBB col) {
		return grid.getTrianglesInOOBB(col);	
	}
	
	public int vertexRawCount() {
		return rawCount;
	}
	
	public void disableGizmos() {
		gizmosEnabled = false;
	}
	
	public void enableGizmos() {
		gizmosEnabled = true;
	}
	
	public void disableGridGizmos() {
		gridGizmosEnabled = false;
	}
	
	public void enableGridGizmos() {
		gridGizmosEnabled = true;
	}
	
	public bool isGizmosEnabled() {
		return gizmosEnabled;	
	}
	
	public bool isGridGizmosEnabled() {
		return gridGizmosEnabled;	
	}
}
