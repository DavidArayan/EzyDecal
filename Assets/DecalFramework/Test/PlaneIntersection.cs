using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PlaneIntersection : MonoBehaviour {
	
	public GameObject intersect;
	
	private List<Vector3> intPoints = new List<Vector3>();
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(0,1,0,1);
		Vector3 size = new Vector3(0.2f,0.2f,0.2f);
		
		Gizmos.DrawCube(gameObject.transform.position, size);
		Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.TransformDirection(Vector3.forward));
		
		if (intersect == null) {
			return;	
		}
		
		if (Selection.activeGameObject != gameObject) {
			intPoints.Clear();
			return;
		}
		
		if (Selection.activeGameObject == gameObject) {
			Vector3 intersect1 = new Vector3();
			Vector3 intersect2 = new Vector3();
			
			intPoints.Clear();
			NDPlane plane = new NDPlane(gameObject.transform.TransformDirection(Vector3.forward), gameObject.transform.position);
			
			Mesh m = intersect.GetComponent<MeshFilter>().sharedMesh;
			
			int[] tris = m.triangles;
			
			for (int i = 0; i < tris.Length; i+=3) {
				Vector3 point1 = intersect.transform.TransformPoint(m.vertices[tris[i + 0]]);
				Vector3 point2 = intersect.transform.TransformPoint(m.vertices[tris[i + 1]]);
				Vector3 point3 = intersect.transform.TransformPoint(m.vertices[tris[i + 2]]);
				
				int counter = 0;
				
				if (NearestPointTest.sideOfPlane(plane, point1)) {
					// check to ensure all positive
					counter++;
					
					intPoints.Add(point1);
					
					if (NearestPointTest.sideOfPlane(plane, point2)) {
						counter++;
						
						intPoints.Add(point2);
					}
					
					if (NearestPointTest.sideOfPlane(plane, point3)) {
						counter++;
						
						intPoints.Add(point3);
					}
				}
				else {
					counter++;
					
					if (!NearestPointTest.sideOfPlane(plane, point2)) {
						counter++;
					}
					
					if (!NearestPointTest.sideOfPlane(plane, point3)) {
						counter++;
					}
				}
				
				// if any discripency between triangles on which side they lay on
				if (counter != 3) {
					// perform the plane-triangle intersection
					int intersection = NearestPointTest.intersectPlaneTriangle(	plane,
																				point1, point2, point3,
																				ref intersect1, ref intersect2);
				
					if (intersection == 1) {
						intPoints.Add(intersect1);
					}
					
					if (intersection == 2) {
						intPoints.Add(intersect1);
						intPoints.Add(intersect2);
					}	
				}
			}
		}
		
		for (int i = 0; i < intPoints.Count; i++) {
			Gizmos.color = new Color(1,0,0,1);
			
			Gizmos.DrawCube(intPoints[i], size);
		}
	}
}
