using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NearestPointTest {
	public struct MapPoint2D {
        public Vector2 map;
		public Vector3 ptRef;
	}
	// for triangle a-b-c return a point q in triangle that is closest to p
	public static Vector3 closestPtPointTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c) {
		// optimised version as found in book Real time collision detection
		Vector3 ab = b - a;
		Vector3 ac = c - a;
		Vector3 ap = p - a;
		
		float d1 = Vector3.Dot(ab,ap);
		float d2 = Vector3.Dot(ac,ap);
		
		if (d1 <= 0.0f && d2 <= 0.0f) {
			return a;	
		}
		
		Vector3 bp = p - b;
		
		float d3 = Vector3.Dot(ab, bp);
		float d4 = Vector3.Dot(ac, bp);
		
		if (d3 >= 0.0f && d4 <= d3) {
			return b;	
		}
		
		float vc = d1 * d4 - d3 * d2;
		
		if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f) {
			float v = d1 / (d1 - d3);
			return a + v * ab;
		}
		
		Vector3 cp = p - c;
		float d5 = Vector3.Dot(ab, cp);
		float d6 = Vector3.Dot(ac, cp);
		
		if (d6 >= 0.0f && d5 <= d6) {
			return c;	
		}
		
		float vb = d5 * d2 - d1 * d6;
		
		if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f) {
			float w = d2 / (d2 - d6);
			return a + w * ac;
		}
		
		float va = d3 * d6 - d5 * d4;
		
		if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f) {
			float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
			return b + w * (c - b);
		}
		
		float denom = 1.0f / (va + vb + vc);
		float vn = vb * denom;
		float wn = vc * denom;
		
		return a + ab * vn + ac * wn;
	}
	
	public static bool pointInTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 p) {
		a -= p;
		b -= p;
		c -= p;
		
		float ab = Vector3.Dot(a,b);
		float ac = Vector3.Dot(a,c);
		float bc = Vector3.Dot(b,c);
		float cc = Vector3.Dot(c,c);
		
		if (bc * ac - cc * ab < 0.0f) return false;
		
		float bb = Vector3.Dot(b,b);
		
		if (ab * bc - ac * bb < 0.0f) return false;
		
		return true;
	}
	
	// for triangle a-b-c, intersect the line made by p-q and store intersection point in r
	public static bool intersectLineTriangle(Vector3 p, Vector3 q, Vector3 a, Vector3 b, Vector3 c, ref Vector3 r) {
		// as found in book real time collision detection
		Vector3 pq = q - p;
		Vector3 pa = a - p;
		Vector3 pb = b - p;
		Vector3 pc = c - p;
		
		Vector3 m = Vector3.Cross(pq,pc);
		
		float u = Vector3.Dot(pb,m);		
		float v = -Vector3.Dot(pa,m);
		
		if (Mathf.Sign(u) != Mathf.Sign(v)) {
			return false;
		}
		
		// scalar triple product
		float w = Vector3.Dot(pq, Vector3.Cross(pb,pa));
		
		if (Mathf.Sign(u) != Mathf.Sign(w)) {
			return false;
		}
		
		float denom = 1.0f / (u + v + w);
		
		r = ((u * denom) * a) + ((v * denom) * b) + ((w * denom) * c);
		
		return true;
	}
	
	private static Vector3 intersectionPointAB = new Vector3();
	private static Vector3 intersectionPointAC = new Vector3();
	private static Vector3 intersectionPointBC = new Vector3();
	// will attempt to intersect the triangle formed by a-b-c on plane and store intersection results in intA and intB (if any)
	// returns the number of intersection points found
	public static int intersectPlaneTriangle(NDPlane plane, Vector3 a, Vector3 b, Vector3 c, ref Vector3 intA, ref Vector3 intB) {		
		int intersectionCounter = 0;
		
		// test segment a-b
		if (intersectLinePlane(plane, a, b, ref intersectionPointAB)) {
			intersectionCounter++;
			
			intA = intersectionPointAB;
		}
		
		// test segment a-c
		if (intersectLinePlane(plane, a, c, ref intersectionPointAC)) {
			if (intersectionCounter == 0) {
				intA = intersectionPointAC;
			}
			else {
				intB = intersectionPointAC;
			}
			
			intersectionCounter++;
		}
		
		// only test last segment if and only if any of last intersections failed
		if (intersectionCounter != 2) {
			
			// test segment b-c
			if (intersectLinePlane(plane, b, c, ref intersectionPointBC)) {
				if (intersectionCounter == 0) {
					intA = intersectionPointBC;
				}
				
				if (intersectionCounter == 1) {
					intB = intersectionPointBC;
				}
				
				intersectionCounter++;
			}
		}
		
		return intersectionCounter;
	}
	
	// intersect the line a-b with plane p and store result in q
	// returns true or false if intersection is found
	public static bool intersectLinePlane(NDPlane plane, Vector3 a, Vector3 b, ref Vector3 q) {
		Vector3 ab = b - a;
		
		float t = (plane.d - Vector3.Dot(plane.n, a)) / Vector3.Dot(plane.n, ab);
		
		if (t >= -0.0001f && t <= 1.0001f) {
			q = a + t * ab;
			
			return true;
		}
		
		return false;
	}
	
	// intersect the line a-b with plane p
	// returns true or false if intersection is found
	public static bool intersectLinePlane(NDPlane plane, Vector3 a, Vector3 b) {
		Vector3 ab = b - a;
		
		float t = (plane.d - Vector3.Dot(plane.n, a)) / Vector3.Dot(plane.n, ab);
		
		return t >= -0.001f && t <= 1.0001f;
	}
	
	// check to see where the point p lies on plane
	public static bool sideOfPlane(NDPlane plane, Vector3 p) {
		return Vector3.Dot(plane.n, p) >= plane.d - 0.001f;	
	}
	
	public static bool sideOfPlane(NDPlane plane, Vector3 p, float tol) {
		return Vector3.Dot(plane.n, p) >= plane.d + tol;	
	}
	
	// check to see if point p lies anywhere inbetween any of the six planes
	public static bool sideOfSixPlanes(	NDPlane plane1, 
										NDPlane plane2,
										NDPlane plane3,
										NDPlane plane4,
										NDPlane plane5,
										NDPlane plane6,
										Vector3 p) 
	{
		return	Vector3.Dot(plane1.n, p) >= plane1.d - 0.001f &&
				Vector3.Dot(plane2.n, p) >= plane2.d - 0.001f &&
				Vector3.Dot(plane3.n, p) >= plane3.d - 0.001f &&
				Vector3.Dot(plane4.n, p) >= plane4.d - 0.001f &&
				Vector3.Dot(plane5.n, p) >= plane5.d - 0.001f &&
				Vector3.Dot(plane6.n, p) >= plane6.d - 0.001f;
	}
	
	public static void sideOfSixPlanesFilter(	NDPlane plane1, 
												NDPlane plane2,
												NDPlane plane3,
												NDPlane plane4,
												NDPlane plane5,
												NDPlane plane6, 
												List<Vector3> lpoints, 
												List<Vector3> points)
	{
		for (int i = 0; i < lpoints.Count; i++) {
			if (sideOfSixPlanes(plane1,plane2,plane3,plane4,plane5,plane6,lpoints[i])) {
				points.Add(lpoints[i]);
			}
		}
	}
	
	private static Vector3 intersect1 = new Vector3();
	private static Vector3 intersect2 = new Vector3();
	
	// a non filtered intersection test
	public static void intersectSixPlanesTriangleUF(NDPlane yPosPlane,
													NDPlane yNegPlane,
													NDPlane xPosPlane,
													NDPlane xNegPlane,
													NDPlane zPosPlane,
													NDPlane zNegPlane,
													Vector3 point1,
													Vector3 point2,
													Vector3 point3,
													List<Vector3> intPoints)	
	{
		// quick exit code - if all points are inside plane, no intersection needed
		int intersection = 0;
		int inTris = 0;
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point1)) {
			intPoints.Add(point1);
			inTris++;
		}
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point2)) {
			intPoints.Add(point2);
			inTris++;
		}
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point3)) {
			intPoints.Add(point3);
			inTris++;
		}
		
		if (inTris == 3) {
			return;	
		}
		
		// if ptCount is anything else, we must perform full intersection with triangles
		intersection = NearestPointTest.intersectPlaneTriangle(yPosPlane, point1, point2, point3, ref intersect1, ref intersect2);
			
			
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
		
		intersection = NearestPointTest.intersectPlaneTriangle(yNegPlane, point1, point2, point3, ref intersect1, ref intersect2);
		
		
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
		
		intersection = NearestPointTest.intersectPlaneTriangle(xPosPlane, point1, point2, point3, ref intersect1, ref intersect2);
		
		
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
		
		intersection = NearestPointTest.intersectPlaneTriangle(xNegPlane, point1, point2, point3, ref intersect1, ref intersect2);
		
		
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
		
		intersection = NearestPointTest.intersectPlaneTriangle(zPosPlane, point1, point2, point3, ref intersect1, ref intersect2);
		
		
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
		
		intersection = NearestPointTest.intersectPlaneTriangle(zNegPlane, point1, point2, point3, ref intersect1, ref intersect2);
		
		
		if (intersection == 1) {
			intPoints.Add(intersect1);
		}
		else if (intersection == 2) {
			intPoints.Add(intersect1);
			intPoints.Add(intersect2);
		}
	}
	
	public static bool intersectSixPlanesTriangle(	NDPlane yPosPlane,
													NDPlane yNegPlane,
													NDPlane xPosPlane,
													NDPlane xNegPlane,
													NDPlane zPosPlane,
													NDPlane zNegPlane,
													Vector3 point1,
													Vector3 point2,
													Vector3 point3,
													List<Vector3> intPoints) 
	{
		// perform early exit, no need to test if no point passes the side test
		int intersection = 0;
		
		bool performIntersect = false;
		
		// check to ensure if any of the points are inside the plane
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point1)) {
			performIntersect = true;
			intPoints.Add(point1);
		}
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point2)) {
			performIntersect = true;
			intPoints.Add(point2);
		}
		
		if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, point3)) {
			performIntersect = true;
			intPoints.Add(point3);
		}
		
		// if none found - make sure the lines do not intersect the planes
		if (!performIntersect) {
			// test plane 1 YPOSPLANE - point1-point2, point2-point3, point3-point1
			bool intersectAbove = false;
			
			if (NearestPointTest.intersectLinePlane(yPosPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(yPosPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(yPosPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	yPosPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}
			}
			
			intersectAbove = false;
			
			// test plane 1 YNEGPLANE - point1-point2, point2-point3, point3-point1
								
			if (NearestPointTest.intersectLinePlane(yNegPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(yNegPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(yNegPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	yNegPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}
			}
			
			intersectAbove = false;
				
			if (NearestPointTest.intersectLinePlane(xPosPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(xPosPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(xPosPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	xPosPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}
			}
			
			intersectAbove = false;
			
			// test plane 1 YNEGPLANE - point1-point2, point2-point3, point3-point1
								
			if (NearestPointTest.intersectLinePlane(xNegPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(xNegPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(xNegPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	xNegPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}	
			}
			
			intersectAbove = false;
				
			if (NearestPointTest.intersectLinePlane(zPosPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(zPosPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(zPosPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	zPosPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}
			}
			
			intersectAbove = false;
			
			// test plane 1 YNEGPLANE - point1-point2, point2-point3, point3-point1
								
			if (NearestPointTest.intersectLinePlane(zNegPlane, point1, point2)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(zNegPlane, point2, point3)) {
				intersectAbove = true;
			}
			else if (NearestPointTest.intersectLinePlane(zNegPlane, point3, point1)) {
				intersectAbove = true;
			}
			
			if (intersectAbove) {
				intersection = NearestPointTest.intersectPlaneTriangle(	zNegPlane,
																		point1, point2, point3,
																		ref intersect1, ref intersect2);
				if (intersection == 1) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
				}
				else if (intersection == 2) {
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
						intPoints.Add(intersect1);
					}
					
					if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
						intPoints.Add(intersect2);
					}
				}	
			}
			
			intersectAbove = false;
		}
		
		// if any of the points are inside the planes, there must be an intersection, find the intersection points
		if (performIntersect) {			
			intersection = NearestPointTest.intersectPlaneTriangle(yPosPlane,
																	point1, point2, point3,
																	ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
			
			intersection = NearestPointTest.intersectPlaneTriangle(yNegPlane,
												point1, point2, point3,
												ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
			
			intersection = NearestPointTest.intersectPlaneTriangle(xPosPlane,
												point1, point2, point3,
												ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
			
			intersection = NearestPointTest.intersectPlaneTriangle(xNegPlane,
												point1, point2, point3,
												ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
			
			intersection = NearestPointTest.intersectPlaneTriangle(zPosPlane,
												point1, point2, point3,
												ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
			
			intersection = NearestPointTest.intersectPlaneTriangle(zNegPlane,
												point1, point2, point3,
												ref intersect1, ref intersect2);
			
			
			if (intersection == 1) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
			}
			else if (intersection == 2) {
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect1)) {
					intPoints.Add(intersect1);
				}
				
				if (NearestPointTest.sideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, intersect2)) {
					intPoints.Add(intersect2);
				}
			}
		}
		
		return intPoints.Count > 0;
	}
	
	// for tetrahedron a-b-c-d return a point q in tetrahedron that is closest to p
	public static Vector3 closestPtPointTetrahedron(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		// version as found in book real time collision detection
		Vector3 closestPt = p;
		float bestSqDist = float.MaxValue;
		
		// a-b-c
		if (pointOutsideOfPlane(p, a, b, c)) {
			Vector3 q = closestPtPointTriangle(p, a, b, c);
			
			float sqDist = (q - p).sqrMagnitude;
			
			if (sqDist < bestSqDist) {
				bestSqDist = sqDist;
				closestPt = q;
			}
		}
		
		// a-c-d
		if (pointOutsideOfPlane(p, a, c, d)) {
			Vector3 q = closestPtPointTriangle(p, a, c, d);
			
			float sqDist = (q - p).sqrMagnitude;
			
			if (sqDist < bestSqDist) {
				bestSqDist = sqDist;
				closestPt = q;
			}
		}
		
		// a-d-b
		if (pointOutsideOfPlane(p, a, d, b)) {
			Vector3 q = closestPtPointTriangle(p, a, d, b);
			
			float sqDist = (q - p).sqrMagnitude;
			
			if (sqDist < bestSqDist) {
				bestSqDist = sqDist;
				closestPt = q;
			}
		}
		
		// b-d-c
		if (pointOutsideOfPlane(p, b, d, c)) {
			Vector3 q = closestPtPointTriangle(p, b, d, c);
			
			float sqDist = (q - p).sqrMagnitude;
			
			if (sqDist < bestSqDist) {
				bestSqDist = sqDist;
				closestPt = q;
			}
		}
		
		return closestPt;
	}
	
	// compute the barycentric coordinates of triangle a-b-c in regards to point p and store result in references u-v-w respectively
	public static void barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 p, ref float u, ref float v, ref float w) {
		Vector3 m = Vector3.Cross(b - a, c - a);
		
		float nu;
		float nv;
		float ood;
		
		float x = Mathf.Abs(m.x);
		float y = Mathf.Abs(m.y);
		float z = Mathf.Abs(m.z);
		
		// compute areas of plane with largest projections
		if (x >= y && x >= z) {
			// area of PBC in yz plane
			nu = triArea2D(p.y, p.z, b.y, b.z, c.y, c.z);
			// area of PCA in yz plane
			nv = triArea2D(p.y, p.z, c.y, c.z, a.y, a.z);
			// 1/2*area of ABC in yz plane
			ood = 1.0f / m.x;
		}
		else if (y >= x && y >= z) {
			// project in xz plane
			nu = triArea2D(p.x, p.z, b.x, b.z, c.x, c.z);
			nv = triArea2D(p.x, p.z, c.x, c.z, a.x, a.z);
			ood = 1.0f / -m.y;
		}
		else {
			// project in xy plane
			nu = triArea2D(p.x, p.y, b.x, b.y, c.x, c.y);
			nv = triArea2D(p.x, p.y, c.x, c.y, a.x, a.y);
			ood = 1.0f / m.z;
		}
		
		u = nu * ood;
		v = nv * ood;
		w = 1.0f - u - v;
	}
	
	private static List<Mesh> tmpMeshes = new List<Mesh>();
	
	// used to store the vertices in the left cut
	private static List<Vector3> leftCutVert = new List<Vector3>();
	// used to store the trianggles in left cut - subindex1
	private static Dictionary<int, List<int>> leftCutTri = new Dictionary<int, List<int>>();
	// used for adding new submesh of cut cross section
	private static List<int> leftCutTri2 = new List<int>();
	// used to store the generated UV coordinates for left cut
	private static List<Vector2> leftCutUV = new List<Vector2>();
	// used to store vertex normals for the left cut
	private static List<Vector3> leftCutNorm = new List<Vector3>();
	// temporary storage for triangulator
	private static List<Vector3> leftTriPT = new List<Vector3>();
	// used to store the vertices in the right cut
	private static List<Vector3> rightCutVert = new List<Vector3>();
	// used to store the trianggles in right cut
	private static Dictionary<int, List<int>> rightCutTri = new Dictionary<int, List<int>>();
	// used for adding new submesh of cut cross section
	private static List<int> rightCutTri2 = new List<int>();
	// used to store the generated UV coordinates for right cut
	private static List<Vector2> rightCutUV = new List<Vector2>();
	// used to store vertex normals for the left cut
	private static List<Vector3> rightCutNorm = new List<Vector3>();
	// temporary storage for triangulator
	private static List<Vector3> rightTriPT = new List<Vector3>();
	
	private static List<Vector3> closedTri = new List<Vector3>();
	private static List<Vector3> tri = new List<Vector3>();
	private static NDPlane planeEQ = new NDPlane();
	
	public static void sliceMesh(List<SlicerPlane> planes, List<Mesh> meshes, GameObject go, bool isHallow) {
		Vector3 intA = Vector3.zero;
		Vector3 intB = Vector3.zero;
		
		// all states must be zero
		leftCutVert.Clear();
		leftCutUV.Clear();
		leftCutNorm.Clear();

		rightCutVert.Clear();
		rightCutUV.Clear();
		rightCutNorm.Clear();
		
		closedTri.Clear();
		tmpMeshes.Clear();
		tri.Clear();
		
		// TODO - This needs to go on a loop
		for (int j = 0; j < planes.Count; j++) {
			// we first need to bring the plane into the coordinate frame of our object
			// these are our original points
			Vector3 position = planes[j].transform.position;
			Vector3 direction = planes[j].transform.TransformDirection(Vector3.up);
			
			// transform them to our coordinates for proper intersection testing
			Vector3 tPosition = go.transform.InverseTransformPoint(position);
			Vector3 tDirection = go.transform.InverseTransformDirection(direction);
			
			// setup our NDPlane for intersection
			planeEQ.setValues(tDirection, tPosition);
			
			// planeEQ is now in our objects coordinate frame
			// we can now perform our intersection tests
			// this pass will create a new Mesh assembly - clear old data
			
			// if the count is 0, we need to slice our current mesh
			// we need to slice our original meshes
			
			for (int k = 0; k < meshes.Count; k++) {
				Mesh mesh = meshes[k];
				
				leftCutVert.Clear();
				leftCutUV.Clear();
				leftCutTri2.Clear();
				leftCutNorm.Clear();
		
				rightCutVert.Clear();
				rightCutUV.Clear();
				rightCutTri2.Clear();
				rightCutNorm.Clear();
				
				closedTri.Clear();
				// otherwise grab mesh and start performing full intersections
				
				// we want to compute each submesh seperately. - each will be added seperately
				for (int subIndex = 0; subIndex < mesh.subMeshCount; subIndex++) {
					// begin looping through all the triangles
					
					// clear previous submesh triangle data from leftCut
					if (leftCutTri.ContainsKey(subIndex)) {
						leftCutTri[subIndex].Clear();	
					}
					else {
						leftCutTri.Add(subIndex, new List<int>());
					}
					
					// clear previous submesh triangle data from rightCut
					if (rightCutTri.ContainsKey(subIndex)) {
						rightCutTri[subIndex].Clear();	
					}
					else {
						rightCutTri.Add(subIndex, new List<int>());
					}
					
					List<int> leftCutTri1 = leftCutTri[subIndex];
					List<int> rightCutTri1 = rightCutTri[subIndex];
					
					// grab triangles from current submesh
					int[] meshTriangles = mesh.GetTriangles(subIndex);
					
					for (int i = 0; i < meshTriangles.Length; i+=3) {
						Vector3 a = mesh.vertices[meshTriangles[i + 0]];
						Vector3 b = mesh.vertices[meshTriangles[i + 1]];	
						Vector3 c = mesh.vertices[meshTriangles[i + 2]];
						
						// grab original UV coordinates
						Vector2 uva = mesh.uv[meshTriangles[i + 0]];
						Vector2 uvb = mesh.uv[meshTriangles[i + 1]];
						Vector2 uvc = mesh.uv[meshTriangles[i + 2]];
						
						// grab the original normals
						Vector3 na = mesh.normals[meshTriangles[i + 0]];
						Vector3 nb = mesh.normals[meshTriangles[i + 1]];
						Vector3 nc = mesh.normals[meshTriangles[i + 2]];
						
						Vector3 triN = Vector3.Cross(b - a, c - a);
						triN.Normalize();
						
						// clear our previous triangle buffer
						leftTriPT.Clear();
						rightTriPT.Clear();
						
						// lets see if this triangle is actually capable of intersection
						if (NearestPointTest.sideOfPlane(planeEQ, a, -0.001f)) {						
							rightTriPT.Add(a);
						}
						
						if (!NearestPointTest.sideOfPlane(planeEQ, a, 0.001f)) {
							leftTriPT.Add(a);
						}
						
						if (NearestPointTest.sideOfPlane(planeEQ, b, -0.001f)) {						
							rightTriPT.Add(b);
						}
						
						if (!NearestPointTest.sideOfPlane(planeEQ, b, 0.001f)) {
							leftTriPT.Add(b);
						}
						
						if (NearestPointTest.sideOfPlane(planeEQ, c, -0.001f)) {						
							rightTriPT.Add(c);
						}
						
						if (!NearestPointTest.sideOfPlane(planeEQ, c, 0.001f)) {
							leftTriPT.Add(c);
						}
						
						// if counter is 0 or 3, triangle cannot intersect the plane, add as is and early exit
						if (leftTriPT.Count == 3) {
							// add vertices to the left cut
							int vertPosA = leftCutVert.Count;
							leftCutVert.Add(a);	
							// add new UV coord
							leftCutUV.Add(uva);
							// add normal
							leftCutNorm.Add(na);
							
							int vertPosB = leftCutVert.Count;
							leftCutVert.Add(b);
							// add new UV coord
							leftCutUV.Add(uvb);
							// add normal
							leftCutNorm.Add(nb);
							
							int vertPosC = leftCutVert.Count;
							leftCutVert.Add(c);
							// add new UV coord
							leftCutUV.Add(uvc);
							// add normal
							leftCutNorm.Add(nc);
							
							// add triangles to specified indexes
							leftCutTri1.Add(vertPosA);
							leftCutTri1.Add(vertPosB);
							leftCutTri1.Add(vertPosC);
							
							if (isHallow) {
								leftCutTri2.Add(vertPosA);
								leftCutTri2.Add(vertPosC);
								leftCutTri2.Add(vertPosB);
							}
							
							leftTriPT.Clear();
						}
						
						if (rightTriPT.Count == 3) {
							// add vertices to the right cut
							int vertPosA = rightCutVert.Count;
							rightCutVert.Add(a);
							// add new UV coord
							rightCutUV.Add(uva);
							// add normal
							rightCutNorm.Add(na);
							
							int vertPosB = rightCutVert.Count;
							rightCutVert.Add(b);
							// add new UV coord
							rightCutUV.Add(uvb);
							// add normal
							rightCutNorm.Add(nb);
							
							int vertPosC = rightCutVert.Count;
							// check to ensure vertex doesnt exist
							rightCutVert.Add(c);
							// add new UV coord
							rightCutUV.Add(uvc);
							// add normal
							rightCutNorm.Add(nc);
						
							// add triangles
							rightCutTri1.Add(vertPosA);
							rightCutTri1.Add(vertPosB);
							rightCutTri1.Add(vertPosC);
							
							if (isHallow) {
								rightCutTri2.Add(vertPosA);
								rightCutTri2.Add(vertPosC);
								rightCutTri2.Add(vertPosB);
							}
							
							rightTriPT.Clear();
						}
						
						// determine if we need to proceed
						if (leftTriPT.Count > 0 || rightTriPT.Count > 0) {
							// otherwise we have an intersection, find the points
							
							// find the intersection points between plane and triangle a-b-c
							int intCounter = NearestPointTest.intersectPlaneTriangle(planeEQ, a, b, c,ref intA, ref intB);
							float u = 0.0f;
							float v = 0.0f;
							float w = 0.0f;
							
							//Debug.Log("INT_COUNTER: " + intCounter);
							
							if (intCounter == 1) {
								
								//Vector3 invIntPT = transform.TransformPoint(intA);
								
								leftTriPT.Add(intA);
								rightTriPT.Add(intA);
								
								// we also add these points for the closing the mesh
								closedTri.Add(intA);
								
								//debugIntPt.Add(transform.TransformPoint(intA));	
							}
							else if (intCounter == 2) {
								
								leftTriPT.Add(intA);
								leftTriPT.Add(intB);
								
								rightTriPT.Add(intB);
								rightTriPT.Add(intA);
								
								closedTri.Add(intA);
								closedTri.Add(intB);
								
								//debugIntPt.Add(transform.TransformPoint(intA));	
								//debugIntPt.Add(transform.TransformPoint(intB));	
							}
							
							// we have our lists, we should triangulate and add the points into our final mesh
							// lets do the left mesh first - we shouldnt get more than 4 points per intersection
							tri.Clear();
							NearestPointTest.triangulateSafe(leftTriPT, tri, triN);
							
							// used for computing the barycentric coordinates
							
							for (int m = 0; m < tri.Count; m += 3) {
								Vector3 ma = tri[m + 0];
								Vector3 mb = tri[m + 1];
								Vector3 mc = tri[m + 2];
								
								bool order = false;
								
								if (NearestPointTest.isTriClockwise(ma,mb,mc,triN)) {
									order = true;
								}
								
								// add vertices to the left cut
								//int vertPosA = leftCutVert.Count;
								if (!isHallow) {
									int vertPosA = leftCutVert.Count;	
									leftCutVert.Add(ma);
									
									NearestPointTest.barycentric(a, b, c, ma, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									
									int vertPosB = leftCutVert.Count;
									leftCutVert.Add(mb);
									
									NearestPointTest.barycentric(a, b, c, mb, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									
									int vertPosC = leftCutVert.Count;							
									leftCutVert.Add(mc);
									
									NearestPointTest.barycentric(a, b, c, mc, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									
									if (order) {
										leftCutTri1.Add(vertPosA);
										leftCutTri1.Add(vertPosB);
										leftCutTri1.Add(vertPosC);	
									}
									else {
										leftCutTri1.Add(vertPosA);
										leftCutTri1.Add(vertPosC);
										leftCutTri1.Add(vertPosB);	
									}	
								}
								else {
									int vertPosA1 = leftCutVert.Count;	
									leftCutVert.Add(ma);
									int vertPosA2 = leftCutVert.Count;	
									leftCutVert.Add(ma);
									
									NearestPointTest.barycentric(a, b, c, ma, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									leftCutNorm.Add(-triN);
									
									int vertPosB1 = leftCutVert.Count;
									leftCutVert.Add(mb);
									int vertPosB2 = leftCutVert.Count;
									leftCutVert.Add(mb);
									
									NearestPointTest.barycentric(a, b, c, mb, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									leftCutNorm.Add(-triN);
									
									int vertPosC1 = leftCutVert.Count;							
									leftCutVert.Add(mc);
									int vertPosC2 = leftCutVert.Count;							
									leftCutVert.Add(mc);
									
									NearestPointTest.barycentric(a, b, c, mc, ref u, ref v, ref w);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutUV.Add(u * uva + v * uvb + w * uvc);
									leftCutNorm.Add(triN);
									leftCutNorm.Add(-triN);
									
									if (order) {
										leftCutTri1.Add(vertPosA1);
										leftCutTri1.Add(vertPosB1);
										leftCutTri1.Add(vertPosC1);	
										
										leftCutTri2.Add(vertPosA2);
										leftCutTri2.Add(vertPosC2);
										leftCutTri2.Add(vertPosB2);
									}
									else {
										leftCutTri1.Add(vertPosA1);
										leftCutTri1.Add(vertPosC1);
										leftCutTri1.Add(vertPosB1);	
										
										leftCutTri2.Add(vertPosA2);
										leftCutTri2.Add(vertPosB2);
										leftCutTri2.Add(vertPosC2);
									}
								}
							}
							
							// perform same operation for the right hand side mesh
							tri.Clear();
							NearestPointTest.triangulateSafe(rightTriPT, tri, triN);
							
							for (int m = 0; m < tri.Count; m += 3) {
								Vector3 ma = tri[m + 0];
								Vector3 mb = tri[m + 1];
								Vector3 mc = tri[m + 2];
								
								bool order = false;
								
								if (NearestPointTest.isTriClockwise(ma,mb,mc,triN)) {
									order = true;
								}
								
								if (!isHallow) {
									int vertPosA = rightCutVert.Count;
									rightCutVert.Add(ma);
									
									NearestPointTest.barycentric(a, b, c, ma, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									
									int vertPosB = rightCutVert.Count;
									rightCutVert.Add(mb);
									
									NearestPointTest.barycentric(a, b, c, mb, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									
									int vertPosC = rightCutVert.Count;							
									rightCutVert.Add(mc);
									
									NearestPointTest.barycentric(a, b, c, mc, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									
									if (order) {
										rightCutTri1.Add(vertPosA);
										rightCutTri1.Add(vertPosB);
										rightCutTri1.Add(vertPosC);	
									}
									else {
										rightCutTri1.Add(vertPosA);
										rightCutTri1.Add(vertPosC);
										rightCutTri1.Add(vertPosB);	
									}
								}
								else {
									int vertPosA1 = rightCutVert.Count;
									rightCutVert.Add(ma);
									int vertPosA2 = rightCutVert.Count;
									rightCutVert.Add(ma);
									
									NearestPointTest.barycentric(a, b, c, ma, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									rightCutNorm.Add(-triN);
									
									int vertPosB1 = rightCutVert.Count;
									rightCutVert.Add(mb);
									int vertPosB2 = rightCutVert.Count;
									rightCutVert.Add(mb);
									
									NearestPointTest.barycentric(a, b, c, mb, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									rightCutNorm.Add(-triN);
									
									int vertPosC1 = rightCutVert.Count;							
									rightCutVert.Add(mc);
									int vertPosC2 = rightCutVert.Count;							
									rightCutVert.Add(mc);
									
									NearestPointTest.barycentric(a, b, c, mc, ref u, ref v, ref w);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutUV.Add(u * uva + v * uvb + w * uvc);
									rightCutNorm.Add(triN);
									rightCutNorm.Add(-triN);
									
									if (order) {
										rightCutTri1.Add(vertPosA1);
										rightCutTri1.Add(vertPosB1);
										rightCutTri1.Add(vertPosC1);
										
										rightCutTri2.Add(vertPosA2);
										rightCutTri2.Add(vertPosC2);
										rightCutTri2.Add(vertPosB2);
									}
									else {
										rightCutTri1.Add(vertPosA1);
										rightCutTri1.Add(vertPosC1);
										rightCutTri1.Add(vertPosB1);
										
										rightCutTri2.Add(vertPosA2);
										rightCutTri2.Add(vertPosB2);
										rightCutTri2.Add(vertPosC2);
									}
								}
							}
						}
					}
				}
				
				// we will now attempt to "close off" the mesh completly
				// using the index information for each intersection point as a clue
				
				if (!isHallow) {
					tri.Clear();
					NearestPointTest.triangulateSafe(closedTri, tri, -planeEQ.n);
					
					List<Vector2> N_UV = genUVFromVertex(tri, -planeEQ.n);
					
					// close off the mesh
					for (int i = 0; i < tri.Count; i+=3) {
						Vector3 cta = tri[i + 0];
						Vector3 ctb = tri[i + 1];
						Vector3 ctc = tri[i + 2];
						
						Vector2 ctauv = N_UV[i + 0];
						Vector2 ctbuv = N_UV[i + 1];
						Vector2 ctcuv = N_UV[i + 2];
						
						bool order = false;
						
						if (NearestPointTest.isTriClockwise(cta,ctb,ctc, -planeEQ.n)) {
							order = true;	
						}
						
						int rV1 = rightCutVert.Count;
						rightCutVert.Add(cta);
						rightCutUV.Add(ctauv);
						rightCutNorm.Add(-planeEQ.n);
						
						int rV2 = rightCutVert.Count;
						rightCutVert.Add(ctb);
						rightCutUV.Add(ctbuv);
						rightCutNorm.Add(-planeEQ.n);
						
						int rV3 = rightCutVert.Count;
						rightCutVert.Add(ctc);
						rightCutUV.Add(ctcuv);
						rightCutNorm.Add(-planeEQ.n);
						
						int lV1 = leftCutVert.Count;
						leftCutVert.Add(cta);
						leftCutUV.Add(ctauv);
						leftCutNorm.Add(planeEQ.n);
						
						int lV2 = leftCutVert.Count;
						leftCutVert.Add(ctb);
						leftCutUV.Add(ctbuv);					
						leftCutNorm.Add(planeEQ.n);
						
						int lV3 = leftCutVert.Count;
						leftCutVert.Add(ctc);
						leftCutUV.Add(ctcuv);
						leftCutNorm.Add(planeEQ.n);
						
						// register triangle on left side
						if (order) {
							leftCutTri2.Add(lV1);
							leftCutTri2.Add(lV3);
							leftCutTri2.Add(lV2);
							
							// register triangle on right side
							rightCutTri2.Add(rV1);
							rightCutTri2.Add(rV2);
							rightCutTri2.Add(rV3);
						}
						else {
							leftCutTri2.Add(lV1);
							leftCutTri2.Add(lV2);
							leftCutTri2.Add(lV3);
							
							// register triangle on right side
							rightCutTri2.Add(rV1);
							rightCutTri2.Add(rV3);
							rightCutTri2.Add(rV2);
						}
					}	
				}
				
				tri.Clear();
				closedTri.Clear();
				
				// we have finished assembly - add to meshes and exit
				if (leftCutVert.Count > 3) {
					Mesh leftMesh = new Mesh();
					leftMesh.Clear();
					
					leftMesh.subMeshCount = mesh.subMeshCount + 1;
					
					leftMesh.vertices = leftCutVert.ToArray();
					leftMesh.uv = leftCutUV.ToArray();
					leftMesh.normals = leftCutNorm.ToArray();
					
					for (int i = 0; i < mesh.subMeshCount; i++) {
						leftMesh.SetTriangles(leftCutTri[i].ToArray(), i);
					}
					
					leftMesh.SetTriangles(leftCutTri2.ToArray(), mesh.subMeshCount);
					
					tmpMeshes.Add(leftMesh);
				}
				
				if (rightCutVert.Count > 3) {
					Mesh rightMesh = new Mesh();
					rightMesh.Clear();
					
					rightMesh.subMeshCount = mesh.subMeshCount + 1;
					
					rightMesh.vertices = rightCutVert.ToArray();
					rightMesh.uv = rightCutUV.ToArray();
					rightMesh.normals = rightCutNorm.ToArray();
					
					for (int i = 0; i < mesh.subMeshCount; i++) {
						rightMesh.SetTriangles(rightCutTri[i].ToArray(), i);
					}
					
					rightMesh.SetTriangles(rightCutTri2.ToArray(), mesh.subMeshCount);
					
					tmpMeshes.Add(rightMesh);
				}
			}
			
			meshes.Clear();
			
			meshes.AddRange(tmpMeshes);
			
			tmpMeshes.Clear();
		}
	}
	
	public static void barycentricSign(Vector3 a, Vector3 b, Vector3 c, Vector3 p, ref float u, ref float v, ref float w) {
		Vector3 f1 = a - p;
		Vector3 f2 = b - p;
		Vector3 f3 = c - p;
		
		Vector3 va = Vector3.Cross(a - b, a - c);
		Vector3 va1 = Vector3.Cross(f2, f3);
		Vector3 va2 = Vector3.Cross(f3, f1);
		Vector3 va3 = Vector3.Cross(f1, f2);
		
		float area = va.magnitude;
		
		u = va1.magnitude / area * Mathf.Sign(Vector3.Dot(va, va1));
		v = va2.magnitude / area * Mathf.Sign(Vector3.Dot(va, va2));
		w = va3.magnitude / area * Mathf.Sign(Vector3.Dot(va, va3));
	}
	
	public static List<Vector3> convexHull = new List<Vector3>();
	
	public static void triangulate(List<Vector3> v, List<Vector3> tri, Vector3 n) {
		
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
	}
	
	public static void triangulateSafe(List<Vector3> v, List<Vector3> tri, Vector3 n) {
		
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
	}
	
	// perform a triangle triangulation - uses barycentric coordinates to determine triangles
	// returns result in CW order for triangles
	public static void triangulate3V(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {		
		addTriClockwise(a, b, c, tri);
	}
	
	public static void triangulate3VSafe(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {		
		tri.Add(a);
		tri.Add(b);
		tri.Add(c);
	}
	
	// perform a quad triangulation - uses barycentric coordinates to determine triangles
	// returns result in CW order for triangles
	public static void triangulate4V(Vector3 a, Vector3 b, Vector3 c, Vector3 p, List<Vector3> tri) {
		
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
	}
	
	public static void triangulate4VSafe(Vector3 a, Vector3 b, Vector3 c, Vector3 p, List<Vector3> tri) {
		
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
	}
	
	public static void triangulateNV(List<Vector3> pt, List<Vector3> tri) {
		Vector3 line1 = pt[0];
		
		for (int i = 1; i < pt.Count - 1; i++) {
			tri.Add(line1);
			tri.Add(pt[i]);
			tri.Add(pt[i + 1]);
		}
	}
	
	// helper function used to compute triangles barycentric coordinates
	public static float triArea2D(float x1, float y1, float x2, float y2, float x3, float y3) {
		return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);	
	}
	
	public static void mapPlanar3D2D(List<Vector3> p3, List<MapPoint2D> p2, Vector3 n) {
		Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0,1,0) : r = new Vector3(1,0,0);
		
		Vector3 v = Vector3.Normalize(Vector3.Cross(r,n));
		Vector3 u = Vector3.Cross(n,v);
		
		for (int i = 0; i < p3.Count; i++) {
			MapPoint2D v2 = new MapPoint2D();
			v2.map = new Vector2(Vector3.Dot(p3[i],u), Vector3.Dot(p3[i], v));
			v2.ptRef = p3[i];
			
			p2.Add(v2);
		}
	}
	
	public static void mapPlanar3D2D(List<Vector3> p3, List<Vector2> p2, Vector3 n) {
		Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0,1,0) : r = new Vector3(1,0,0);
		
		Vector3 v = Vector3.Normalize(Vector3.Cross(r,n));
		Vector3 u = Vector3.Cross(n,v);
		
		for (int i = 0; i < p3.Count; i++) {			
			p2.Add(new Vector2(Vector3.Dot(p3[i],u), Vector3.Dot(p3[i], v)));
		}
	}
	
	public static Vector2 cubeMap3DV(Vector3 vec, Vector3 n) {
		Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0,1,0) : r = new Vector3(1,0,0);
		
		Vector3 v = Vector3.Normalize(Vector3.Cross(r,n));
		Vector3 u = Vector3.Cross(n,v);
		
		return new Vector2((Vector3.Dot(vec,u) + 1) / 2, (Vector3.Dot(vec,v) + 1) / 2);
	}
	
	private static List<Vector2> genUV = new List<Vector2>();
	private static List<MapPoint2D> genUVM = new List<MapPoint2D>();
	
	public static List<Vector2> genUVFromVertex(List<Vector3> vertices, Vector3 n) {
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
	}
	
	private static List<MapPoint2D> pt2 = new List<MapPoint2D>();
	
	public static void convexHull2D(List<Vector3> pt, List<Vector3> o, Vector3 norm) {
		// clear our previous mapped points
		pt2.Clear();
		
		mapPlanar3D2D(pt,pt2,norm);
		
		pt2.Sort((x,p) => {
			return (x.map.x < p.map.x || (x.map.x == p.map.x && x.map.y < p.map.y)) ? -1 : 1;
		});
		
		int n = pt2.Count;
		int k = 0;
		
		//pt2.Reverse();
		
		MapPoint2D[] H = new MapPoint2D[n * 2];
		
		// build lower hull
		for (int i = 0; i < n; i++) {
			while (k >= 2 && triArea2D(H[k-2].map.x, H[k-2].map.y, H[k-1].map.x, H[k-1].map.y, pt2[i].map.x, pt2[i].map.y) <= 0) k--;
			//while (k >= 2 && cross2D(H[k-2], H[k-1], pt2[i]) <= 0) k--;
			
			H[k++] = pt2[i];
		}
		
		// build upper hull
		for (int i = n - 2, t = k + 1; i >= 0; i--) {
			while (k >= t && triArea2D(H[k-2].map.x, H[k-2].map.y, H[k-1].map.x, H[k-1].map.y, pt2[i].map.x, pt2[i].map.y) <= 0) k--;
			//while (k >= t && cross2D(H[k-2], H[k-1], pt2[i]) <= 0) k--;
			
			H[k++] = pt2[i];
		}
		
		for (int i = 0; i < k - 1; i++) {
			//if (o.Contains(H[i].ptRef)) {
				o.Add(H[i].ptRef);	
			//}	
		}
		
		//o.Reverse();
	}
	
	public static float cross2D(MapPoint2D O, MapPoint2D A, MapPoint2D B) {
		return (A.map.x - O.map.x) * (B.map.y - O.map.y) - (A.map.y - O.map.y) * (B.map.x - O.map.x);
	}
	
	// check if triangle specified by a-b-c is clockwise. true if so, false otherwise
	public static bool isTriClockwise(Vector3 a, Vector3 b, Vector3 c) {		
		return (a.x * (b.y * c.z - b.z * c.y) - a.y * (b.x * c.z - b.z * c.x) + a.z * (b.x * c.y - b.y * c.x)) >= 0;
	}
	
	public static bool isTriClockwise(Vector3 a, Vector3 b, Vector3 c, Vector3 normal) {
		Vector3 n = Vector3.Cross(b - a, c - a);
		//n.Normalize();
		
		//Debug.Log(Vector3.Dot(normal,n));
		
		return Vector3.Dot(normal,n) >= 0;
	}
	
	public static float triArea3D(Vector3 a, Vector3 b, Vector3 c) {
		return Vector3.Cross(a - b, a - c).magnitude * 0.5f;	
	}
	
	public static void addTriClockwise(Vector3 a, Vector3 b, Vector3 c, List<Vector3> tri) {
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
	}
	
	// helper function
	public static bool pointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c) {
		return Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)) >= 0.0f;	
	}
	
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