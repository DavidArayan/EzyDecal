using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using DecalFramework;

public class ProjectedStaticDecal : MonoBehaviour {
	
	[HideInInspector]
	public NDPlane yPosPlane = new NDPlane();
	
	[HideInInspector]
	public NDPlane yNegPlane = new NDPlane();
	
	[HideInInspector]
	public NDPlane xPosPlane = new NDPlane();
	
	[HideInInspector]
	public NDPlane xNegPlane = new NDPlane();
	
	[HideInInspector]
	public NDPlane zPosPlane = new NDPlane();
	
	[HideInInspector]
	public NDPlane zNegPlane = new NDPlane();
	
	[HideInInspector]
	public List<Vector3> vertexPoints = new List<Vector3>();
	
	[HideInInspector]
	public List<int> indexPoints = new List<int>();
	
	[HideInInspector]
	public List<Vector2> uvPoints = new List<Vector2>();
	
	[HideInInspector]
	public OOBB oobb = new OOBB();
	
	public Material material;
	public int layer = 1;
	
	private static float offset = 0.0015f;
	
	[HideInInspector]
	public bool updateEnabled = true;
	
	[HideInInspector]
	public bool rtUpdateEnabled = false;
	
	[HideInInspector]
	public bool collInEditor = false;
	
	public bool cubeMap = false;
	
	//GameObject projection;
	//private Mesh decalMesh;
	
	[HideInInspector]
	public SceneData data;
	
	public void create(SceneData data) {
		this.data = data;
		
		//material = Resources.LoadAssetAtPath("Assets/DecalFramework/Textures/DefaultDecal.mat", typeof(Material)) as Material;
		offset = 0.01f;
		//decalMesh = new Mesh();
		
		/*if (projection == null) {	
			projection = new GameObject();
			projection.name = gameObject.name + "_proj";
			
			projectionFilter = (MeshFilter)projection.AddComponent(typeof(MeshFilter));
			projection.AddComponent(typeof(MeshRenderer));
			
			projectionFilter.sharedMesh = new Mesh();
			
			//projection.transform.position = offset;
			
			projection.transform.parent = transform.parent;
		}*/
		
		oobb.update(gameObject);
	}
	
	public void clear(SceneData data) {
		create(data);
	}
	
	void OnDrawGizmos() {
		Vector3 size = new Vector3(0.02f,0.02f,0.02f);
		
		if (Selection.Contains(gameObject)) {
			oobb.update(gameObject);
			
			// render the AABB in green
			Gizmos.color = new Color(0,1,0,1);
			
			// draw the center
			Gizmos.DrawCube(oobb.center, size);
			
			// draw the corners
			for (int i = 0; i < 8; i++) {
				Gizmos.DrawCube(oobb.aabbCoordsTrans[i], size);	
			}
			
			// draw the edges - total of 12 edges for AABB
			
			Gizmos.DrawLine(oobb.aabbCoordsTrans[0], oobb.aabbCoordsTrans[1]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[0], oobb.aabbCoordsTrans[2]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[0], oobb.aabbCoordsTrans[4]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[1], oobb.aabbCoordsTrans[3]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[1], oobb.aabbCoordsTrans[5]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[2], oobb.aabbCoordsTrans[6]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[2], oobb.aabbCoordsTrans[3]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[3], oobb.aabbCoordsTrans[7]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[4], oobb.aabbCoordsTrans[5]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[4], oobb.aabbCoordsTrans[6]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[5], oobb.aabbCoordsTrans[7]);
			Gizmos.DrawLine(oobb.aabbCoordsTrans[6], oobb.aabbCoordsTrans[7]);
			
			// render the OOBB in red
			Gizmos.color = new Color(1,0,0,1);
			
			// draw the corners
			for (int i = 0; i < 8; i++) {
				Gizmos.DrawCube(oobb.oobbCoordsTrans[i], size);	
			}
			
			// draw the edges - total of 12 edges for OOBB
			
			Gizmos.DrawLine(oobb.oobbCoordsTrans[0], oobb.oobbCoordsTrans[1]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[0], oobb.oobbCoordsTrans[2]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[0], oobb.oobbCoordsTrans[4]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[1], oobb.oobbCoordsTrans[3]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[1], oobb.oobbCoordsTrans[5]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[2], oobb.oobbCoordsTrans[6]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[2], oobb.oobbCoordsTrans[3]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[3], oobb.oobbCoordsTrans[7]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[4], oobb.oobbCoordsTrans[5]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[4], oobb.oobbCoordsTrans[6]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[5], oobb.oobbCoordsTrans[7]);
			Gizmos.DrawLine(oobb.oobbCoordsTrans[6], oobb.oobbCoordsTrans[7]);
			
			// draw intersection points if any
			/*Gizmos.color = new Color(0,0,1,1);
			
			for (int i = 0; i < vertexPoints.Count; i++) {
				Gizmos.DrawCube(vertexPoints[i], size);	
			}
			
			for (int i = 0; i < indexPoints.Count; i+=3) {
				Gizmos.DrawLine(vertexPoints[indexPoints[i]], vertexPoints[indexPoints[i + 1]]);
				Gizmos.DrawLine(vertexPoints[indexPoints[i + 1]], vertexPoints[indexPoints[i + 2]]);
				Gizmos.DrawLine(vertexPoints[indexPoints[i + 2]], vertexPoints[indexPoints[i]]);
			}*/
		}
	}
	
	private List<Vector3> points = new List<Vector3>();
	private List<Vector3> lpoints = new List<Vector3>();
	private List<Vector3> newTris = new List<Vector3>();
	
	public void updateMesh() {		
		vertexPoints.Clear();
		indexPoints.Clear();
		uvPoints.Clear();
		
		oobb.update(gameObject);
			
		Vector3 dirUp = gameObject.transform.TransformDirection(Vector3.up);
		Vector3 dirUpPos = gameObject.transform.position - (dirUp * gameObject.transform.localScale.y);
		
		Vector3 dirDown = gameObject.transform.TransformDirection(Vector3.down);
		Vector3 dirDownPos = gameObject.transform.position - (dirDown * gameObject.transform.localScale.y);
		
		Vector3 dirForward = gameObject.transform.TransformDirection(Vector3.forward);
		Vector3 dirForwardPos = gameObject.transform.position - (dirForward * gameObject.transform.localScale.z);
		
		Vector3 dirBack = gameObject.transform.TransformDirection(Vector3.back);
		Vector3 dirBackPos = gameObject.transform.position - (dirBack * gameObject.transform.localScale.z);
		
		Vector3 dirLeft = gameObject.transform.TransformDirection(Vector3.left);
		Vector3 dirLeftPos = gameObject.transform.position - (dirLeft * gameObject.transform.localScale.x);
		
		Vector3 dirRight = gameObject.transform.TransformDirection(Vector3.right);
		Vector3 dirRightPos = gameObject.transform.position - (dirRight * gameObject.transform.localScale.x);
		
		
		yPosPlane.SetValues(dirUp, dirUpPos);
		yNegPlane.SetValues(dirDown, dirDownPos);
		
		zPosPlane.SetValues(dirForward, dirForwardPos);
		zNegPlane.SetValues(dirBack, dirBackPos);
		
		xPosPlane.SetValues(dirLeft, dirLeftPos);
		xNegPlane.SetValues(dirRight, dirRightPos);
		
		if (data != null) {
			
			List<TriangleData> tris = data.getTrianglesInOOBB(oobb);
			
			//Debug.Log(tris.Count);
			
			Vector3 lineInt = new Vector3();
			
			for (int i = 0; i < tris.Count; i++) {
				Vector3[] pts = tris[i].getTransformedPoints();
				
				Vector3 point1 = pts[0];
				Vector3 point2 = pts[1];
				Vector3 point3 = pts[2];
				
				Vector3 n = Vector3.Cross(point2 - point1, point3 - point1);
				n.Normalize();
									
				points.Clear();
				newTris.Clear();
				lpoints.Clear();
				
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[0], ref oobb.oobbCoordsTrans[1], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[0], ref oobb.oobbCoordsTrans[2], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[0], ref oobb.oobbCoordsTrans[4], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[1], ref oobb.oobbCoordsTrans[3], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[1], ref oobb.oobbCoordsTrans[5], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[2], ref oobb.oobbCoordsTrans[6], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[2], ref oobb.oobbCoordsTrans[3], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[3], ref oobb.oobbCoordsTrans[7], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[4], ref oobb.oobbCoordsTrans[5], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[4], ref oobb.oobbCoordsTrans[6], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[5], ref oobb.oobbCoordsTrans[7], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);
				if (TriangleTests.IntersectLineTriangle(ref oobb.oobbCoordsTrans[6], ref oobb.oobbCoordsTrans[7], ref point1, ref point2, ref point3, ref lineInt)) lpoints.Add(lineInt);

                NDPlane.IntersectSixPlanesTriangle(xPosPlane, xNegPlane, yPosPlane, yNegPlane, zPosPlane, zNegPlane, ref point1, ref point2, ref point3, lpoints);
                NDPlane.SideOfSixPlanesFilter(xPosPlane, xNegPlane, yPosPlane, yNegPlane, zPosPlane, zNegPlane, lpoints, points);

                HullTests.ConvexHull2DTriangulated(points, newTris, ref n);
			
				for (int j = 0; j < newTris.Count; j += 3) {
					Vector3 v1 = newTris[j + 0];
					Vector3 v2 = newTris[j + 1];
					Vector3 v3 = newTris[j + 2];
					
					float tolerance = layer * offset;
					Vector3 dist = n * tolerance;
					
					v1 = v1 + dist;
					v2 = v2 + dist;
					v3 = v3 + dist;
					
					int vec1 = 0;
					int vec2 = 0;
					int vec3 = 0;
					
					// point is new, compute new UV coordinates for it
					if (!NearestPointTest.approxContains(vertexPoints, v1, tolerance, ref vec1)) {
						Vector3 transVec = gameObject.transform.InverseTransformPoint(vertexPoints[vec1]);
							
						if (!cubeMap) {
							uvPoints.Add(new Vector2((transVec.y + 1) / 2, (transVec.z + 1) / 2));	
						}
						else {
							uvPoints.Add(HullTests.CubeMap3DV(ref transVec, ref n));
						}
					}
					
					if (!NearestPointTest.approxContains(vertexPoints, v2, tolerance, ref vec2)) {
						Vector3 transVec = gameObject.transform.InverseTransformPoint(vertexPoints[vec2]);
							
						if (!cubeMap) {
							uvPoints.Add(new Vector2((transVec.y + 1) / 2, (transVec.z + 1) / 2));	
						}
						else {
							uvPoints.Add(HullTests.CubeMap3DV(ref transVec, ref n));
						}
					}
					
					if (!NearestPointTest.approxContains(vertexPoints, v3, tolerance, ref vec3)) {
						Vector3 transVec = gameObject.transform.InverseTransformPoint(vertexPoints[vec3]);
							
						if (!cubeMap) {
							uvPoints.Add(new Vector2((transVec.y + 1) / 2, (transVec.z + 1) / 2));	
						}
						else {
							uvPoints.Add(HullTests.CubeMap3DV(ref transVec, ref n));
						}
					}
					
					
					//Debug.Log(vertexPoints.Count + " " + vec1 + " " + vec2 + " " + vec3);
					if (HullTests.IsTriClockwise(vertexPoints[vec1], vertexPoints[vec2], vertexPoints[vec3], n)) {
						indexPoints.Add(vec1);
						indexPoints.Add(vec2);
						indexPoints.Add(vec3);	
					}
					else {
						indexPoints.Add(vec1);
						indexPoints.Add(vec3);
						indexPoints.Add(vec2);	
					}
				}
			}
		}
	}
	
	public void selectObj() {
		GameObject[] gos = new GameObject[2];
		gos[0] = gameObject;
		//gos[1] = projection;
		
		Selection.objects = gos;
	}
	
	public void destroy() {
		//DestroyImmediate(projection);
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
	
	public void fillBatchData(MeshBatchFiller batch) {
		//Debug.Log("Asking for Batch");
		batch.vertices = vertexPoints;
		batch.indices = indexPoints;
		batch.uv = uvPoints;
		batch.material = material;
	}
}
