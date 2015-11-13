using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearestPointTest {
	
	//public static List<Vector3> convexHull = new List<Vector3>();
	
	/*public static void triangulate(List<Vector3> v, List<Vector3> tri, Vector3 n) {
		
		if (v.Count < 3) {
			return;	
		}
		
		if (v.Count == 3) {
			triangulate3V(v[0],v[1],v[2],tri);	
			return;
		}
		
		if (v.Count == 4) {
			triangulate4V(v[0],v[1],v[2],v[3],tri);
			return;
		}
		
		convexHull.Clear();
		
		convexHull2D(v,convexHull,n);
		
		if (convexHull.Count < 3) {
			return;	
		}
		
		//tri.AddRange(convexHull);
		
		triangulateNV(convexHull, tri);
	}*/
	
	/*public static void triangulateSafe(List<Vector3> v, List<Vector3> tri, Vector3 n) {
		
		if (v.Count < 3) {
			return;	
		}
		
		if (v.Count == 3) {
			triangulate3VSafe(v[0],v[1],v[2],tri);	
			return;
		}
		
		if (v.Count == 4) {
			triangulate4VSafe(v[0],v[1],v[2],v[3],tri);
			return;
		}
		
		convexHull.Clear();
		
		convexHull2D(v,convexHull,n);
		
		if (convexHull.Count < 3) {
			return;	
		}
		
		//tri.AddRange(convexHull);
		
		triangulateNV(convexHull, tri);
	}*/
	
	// perform a triangle triangulation - uses barycentric coordinates to determine triangles
	// returns result in CW order for triangles
	/*public static void triangulate3V(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {		
		addTriClockwise(a, b, c, tri);
	}
	
	public static void triangulate3VSafe(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {		
		tri.Add(a);
		tri.Add(b);
		tri.Add(c);
	}*/
	
	// perform a quad triangulation - uses barycentric coordinates to determine triangles
	// returns result in CW order for triangles
	/*public static void triangulate4V(Vector3 a, Vector3 b, Vector3 c, Vector3 p, List<Vector3> tri) {
		
		// check fouth point to see how to create indices according to barycentric coords
		float bu = 0.0f;
		float bv = 0.0f;
		float bw = 0.0f;
		
		barycentric(a,b,c,p, ref bu, ref bv, ref bw);
		
		// coordinate lies inside the triangle, quick exit, we don't need extra triangles
		if (bu > 0.0f && bv > 0.0f && bw > 0.0f) {
			addTriClockwise(a, b, c, tri);
			
			return;	
		}
		
		// if coordinate lies on edges, a bigger triangle exists between new point, so take that point and exit
		
		// if true, take a-c-p discarding b
		if (bu < 0.0f && bv > 0.0f && bw < 0.0f) {
			addTriClockwise(a, c, p, tri);
			
			return;
		}
		
		// if true, take a-b-p, discarding c
		if (bu < 0.0f && bv < 0.0f && bw > 0.0f) {
			addTriClockwise(a, b, p, tri);
			
			return;
		}
		
		// if true, take b-c-p, discarding a
		if (bu > 0.0f && bv < 0.0f && bw < 0.0f) {
			addTriClockwise(b, c, p, tri);
			
			return;
		}
		
		// coordinate needed, we will form two triangles in such a way that they don't overlap - register triangle a-b-c
		addTriClockwise(a, b, c, tri);
		
		// if true, form another triangle b-c-p and exit
		if (bu < 0.0f && bv > 0.0f && bw > 0.0f) {
			addTriClockwise(b, c, p, tri);
			
			return;
		}
		
		// if true, form another triangle b-a-p and exit
		if (bu > 0.0f && bv > 0.0f && bw < 0.0f) {
			addTriClockwise(a, b, p, tri);
			
			return;
		}
		
		// if true, form another triangle a-c-p and exit
		if (bu > 0.0f && bv < 0.0f && bw > 0.0f) {
			addTriClockwise(a, c, p, tri);
			
			return;
		}
	}*/
	
	/*public static void triangulate4VSafe(Vector3 a, Vector3 b, Vector3 c, Vector3 p, List<Vector3> tri) {
		
		// check fouth point to see how to create indices according to barycentric coords
		float bu = 0.0f;
		float bv = 0.0f;
		float bw = 0.0f;
		
		barycentric(a,b,c,p, ref bu, ref bv, ref bw);
		
		bu = Mathf.Sign(bu);
		bv = Mathf.Sign(bv);
		bw = Mathf.Sign(bw);
		
		// coordinate lies inside the triangle, quick exit, we don't need extra triangles
		if (bu > 0.0f && bv > 0.0f && bw > 0.0f) {
			tri.Add(a);
			tri.Add(b);
			tri.Add(c);
			
			return;	
		}
		
		// if coordinate lies on edges, a bigger triangle exists between new point, so take that point and exit
		
		// if true, take a-c-p discarding b
		if (bu < 0.0f && bv > 0.0f && bw < 0.0f) {
			tri.Add(a);
			tri.Add(c);
			tri.Add(p);
			
			return;
		}
		
		// if true, take a-b-p, discarding c
		if (bu < 0.0f && bv < 0.0f && bw > 0.0f) {
			tri.Add(a);
			tri.Add(b);
			tri.Add(p);
			
			return;
		}
		
		// if true, take b-c-p, discarding a
		if (bu > 0.0f && bv < 0.0f && bw < 0.0f) {
			tri.Add(a);
			tri.Add(c);
			tri.Add(p);
			
			return;
		}
		
		// coordinate needed, we will form two triangles in such a way that they don't overlap - register triangle a-b-c
		tri.Add(a);
		tri.Add(b);
		tri.Add(c);
		
		// if true, form another triangle b-c-p and exit
		if (bu < 0.0f && bv > 0.0f && bw > 0.0f) {
			tri.Add(b);
			tri.Add(c);
			tri.Add(p);
			
			return;
		}
		
		// if true, form another triangle b-a-p and exit
		if (bu > 0.0f && bv > 0.0f && bw < 0.0f) {
			tri.Add(a);
			tri.Add(b);
			tri.Add(p);
			
			return;
		}
		
		// if true, form another triangle a-c-p and exit
		if (bu > 0.0f && bv < 0.0f && bw > 0.0f) {
			tri.Add(a);
			tri.Add(c);
			tri.Add(p);
			
			return;
		}
	}/*
	
	public static void triangulateNV(List<Vector3> pt, List<Vector3> tri) {
		Vector3 line1 = pt[0];
		
		for (int i = 1; i < pt.Count - 1; i++) {
			tri.Add(line1);
			tri.Add(pt[i]);
			tri.Add(pt[i + 1]);
		}
	}
	
	/*public static void mapPlanar3D2D(List<Vector3> p3, List<Vector2> p2, Vector3 n) {
		Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0,1,0) : r = new Vector3(1,0,0);
		
		Vector3 v = Vector3.Normalize(Vector3.Cross(r,n));
		Vector3 u = Vector3.Cross(n,v);
		
		for (int i = 0; i < p3.Count; i++) {			
			p2.Add(new Vector2(Vector3.Dot(p3[i],u), Vector3.Dot(p3[i], v)));
		}
	}*/
	
	/*public static Vector2 cubeMap3DV(Vector3 vec, Vector3 n) {
		Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0,1,0) : r = new Vector3(1,0,0);
		
		Vector3 v = Vector3.Normalize(Vector3.Cross(r,n));
		Vector3 u = Vector3.Cross(n,v);
		
		return new Vector2((Vector3.Dot(vec,u) + 1) / 2, (Vector3.Dot(vec,v) + 1) / 2);
	}
	
	private static List<Vector2> genUV = new List<Vector2>();
	private static List<MapPoint2D> genUVM = new List<MapPoint2D>();*/
	
	/*public static List<Vector2> genUVFromVertex(List<Vector3> vertices, Vector3 n) {
		genUV.Clear();
		genUVM.Clear();
		
		mapPlanar3D2D(vertices, genUVM, n);
		
		// we now have 2D points, lets generate our UV coordinates
		// find the largest point in either X or Y coordinate to bring everything back to UV space
		
		float divX = 1.0f;
		float divY = 1.0f;
		
		for (int i = 0; i < genUVM.Count; i++) {
			if (genUVM[i].map.x > divX) {
				divX = genUVM[i].map.x;
			}
			
			if (genUVM[i].map.y > divY) {
				divY = genUVM[i].map.y;
			}
		}
		
		divX = (divX + 1 / 2);
		divY = (divY + 1 / 2);
		
		// now its a simple manner, U = x / div, V = y / div
		for (int i = 0; i < genUVM.Count; i++) {
			genUV.Add(new Vector2(genUVM[i].map.x / divX - 0.5f, genUVM[i].map.y / divY - 0.5f));
		}
		
		// each UV maps to its respective 3D coordinate
		return genUV;
	}*/
	
	/*public static float cross2D(MapPoint2D O, MapPoint2D A, MapPoint2D B) {
		return (A.Map.x - O.Map.x) * (B.Map.y - O.Map.y) - (A.Map.y - O.Map.y) * (B.Map.x - O.Map.x);
	}*/
	
	// check if triangle specified by a-b-c is clockwise. true if so, false otherwise
	/*public static bool isTriClockwise(Vector3 a, Vector3 b, Vector3 c) {		
		return (a.x * (b.y * c.z - b.z * c.y) - a.y * (b.x * c.z - b.z * c.x) + a.z * (b.x * c.y - b.y * c.x)) >= 0;
	}*/
	
	/*public static bool isTriClockwise(Vector3 a, Vector3 b, Vector3 c, Vector3 normal) {
		Vector3 n = Vector3.Cross(b - a, c - a);
		//n.Normalize();
		
		//Debug.Log(Vector3.Dot(normal,n));
		
		return Vector3.Dot(normal,n) >= 0;
	}*/
	
	/*public static float triArea3D(Vector3 a, Vector3 b, Vector3 c) {
		return Vector3.Cross(a - b, a - c).magnitude * 0.5f;	
	}*/
	
	/*public static void addTriClockwise(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {
		if (isTriClockwise(a,b,c)) {
			tri.Add(a);
			tri.Add(b);
			tri.Add(c);	
		}
		else {
			tri.Add(a);
			tri.Add(c);
			tri.Add(b);	
		}
	}*/
	
	// helper function
	/*public static bool pointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c) {
		return Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)) >= 0.0f;	
	}*/
	
	public static bool valueWithinRange(float a, float b, float c) {
		return b > a ? c > a && c < b : c > b && c < a;
	}
	
	// this function will test to see if pt is approximetly equal to any of the points in points
	// according to some tolerance value, if so, true is returned and index is stored in a reference
	// otherwise it is added and index is stored while returning false
	public static bool approxContains(List<Vector3> points, Vector3 pt, float tol, ref int index) {
		//Debug.Log("PASS");
		float tolSQ = (tol * tol);
		
		for (int i = 0; i < points.Count; i++) {
			if (Vector3.SqrMagnitude(points[i] - pt) <= tolSQ) {
				//Debug.Log("SQR: " + Vector3.SqrMagnitude(points[i] - pt) + " PT: " + tolSQ);
				// found a vector, store index and return true
				points[i] = (points[i] + pt) / 2;
				index = i;
				return true;
			}
		}
		
		// nothing found, return false
		points.Add(pt);
		index = points.Count - 1;
		return false;
	}
	
	// this function will test to see if pt is approximetly equal to any of the points in points
	// according to some tolerance value, if so, true is returned and index is stored in a reference
	// otherwise it is added and index is stored while returning false
	public static bool approxContainsSafe(List<Vector3> points, Vector3 pt, float tol, ref int index) {
		//Debug.Log("PASS");
		float tolSQ = (tol * tol);
		for (int i = 0; i < points.Count; i++) {
			if (Vector3.SqrMagnitude(points[i] - pt) <= tolSQ) {
				//Debug.Log("SQR: " + Vector3.SqrMagnitude(points[i] - pt) + " PT: " + tolSQ);
				// found a vector, store index and return true
				index = i;
				return true;
			}
		}
		// nothing found, return false
		return false;
	}
}