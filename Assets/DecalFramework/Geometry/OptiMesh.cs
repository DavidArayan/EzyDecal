using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OptiMesh : ScriptableObject {
	
	[HideInInspector]
	public List<TriangleData> triangles;
	
	[HideInInspector]
	public GameObject objRef;
	
	[HideInInspector]
	public float lowestX;
	
	[HideInInspector]
	public float highestX;
	
	[HideInInspector]
	public float lowestY;
	
	[HideInInspector]
	public float highestY;
	
	[HideInInspector]
	public float lowestZ;
	
	[HideInInspector]
	public float highestZ;
	
	void OnEnable() {
		if (triangles == null) {
			triangles = new List<TriangleData>();
		
			lowestX = float.PositiveInfinity;
			highestX = float.NegativeInfinity;
			lowestY = float.PositiveInfinity;
			highestY = float.NegativeInfinity;
			lowestZ = float.PositiveInfinity;
			highestZ = float.NegativeInfinity;	
		}
	}
	
	public void addTriangle(Vector3 a, Vector3 b, Vector3 c) {
		TriangleData tri = ScriptableObject.CreateInstance<TriangleData>();
		
		tri.create(a,b,c);
		
		triangles.Add(tri);
	}
	
	public void transform() {
		for (int i = 0; i < triangles.Count; i++) {
			Vector3[] tPoints = triangles[i].getPoints();
			Vector3[] nPoints = triangles[i].getTransformedPoints();
			
			// transform points
			nPoints[0] = objRef.transform.TransformPoint(tPoints[0]);
			nPoints[1] = objRef.transform.TransformPoint(tPoints[1]);
			nPoints[2] = objRef.transform.TransformPoint(tPoints[2]);
			
			// find the lowest and highest points
			for (int j = 0; j < 3; j++) {
				// x value
				if (nPoints[j].x < lowestX) {
					lowestX = nPoints[j].x;
				}
				
				if (nPoints[j].x > highestX) {
					highestX = nPoints[j].x;
				}
				
				// y value
				if (nPoints[j].y < lowestY) {
					lowestY = nPoints[j].y;
				}
				
				if (nPoints[j].y > highestY) {
					highestY = nPoints[j].y;
				}
				
				// z value
				if (nPoints[j].z < lowestZ) {
					lowestZ = nPoints[j].z;
				}
				
				if (nPoints[j].z > highestZ) {
					highestZ = nPoints[j].z;
				}
			}
			
			// transform center
			triangles[i].setTransCenter(objRef.transform.TransformPoint(triangles[i].getCenter()));
		}
	}
	
	public float getLength() {
		return Mathf.Abs(lowestX - highestX);	
	}
	
	public float getWidth() {
		return Mathf.Abs(lowestZ - highestZ);		
	}
	
	public float getHeight() {
		return Mathf.Abs(lowestY - highestY);		
	}
	
	public TriangleData[] getTriangleData() {
		return triangles.ToArray();	
	}
	
	public int getVertexCount() {
		return triangles.Count * 3;	
	}
	
	public GameObject getObjRef() {
		return objRef;	
	}
	
}
