using UnityEngine;
using System.Collections;

[System.Serializable]
public class TriangleData : ScriptableObject {
	
	[HideInInspector]
	public Vector3[] points;
	
	[HideInInspector]
	public Vector3[] nPoints;
	
	[HideInInspector]
	public Vector3 center;
	
	[HideInInspector]
	public Vector3 tCenter;
	
	[HideInInspector]
	public float radius;
	
	void OnEnable() {
		if (points == null) {
			points = new Vector3[3];
			nPoints = new Vector3[3];
			radius = 0.0f;
		}
	}
	
	public void create(Vector3 p1, Vector3 p2, Vector3 p3) {		
		points[0] = p1;
		points[1] = p2;
		points[2] = p3;
		
		this.center = (p1 + p2 + p3) / 3;
	}
	
	public void setTransCenter(Vector3 c) {
		tCenter = c;	
		
		float rad1 = (tCenter - nPoints[0]).magnitude;
		float rad2 = (tCenter - nPoints[1]).magnitude;
		float rad3 = (tCenter - nPoints[2]).magnitude;
		
		radius = Mathf.Max(Mathf.Max(rad1,rad2), rad3);
	}
		
	public Vector3[] getTransformedPoints() {
		return nPoints;
	}
	
	public Vector3[] getPoints() {
		return points;
	}
	
	public Vector3 getCenter() {
		return center;	
	}
	
	public Vector3 getTransCenter() {
		return tCenter;
	}
	
	public float getRadius() {
		return radius;
	}
}
