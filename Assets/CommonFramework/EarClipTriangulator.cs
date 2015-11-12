using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarClipTriangulator {
	
	private static int CONCAVE = -1;
	private static int CONVEX = 1;
	
	private static List<Vector2> vertices = new List<Vector2>();
	private static int vertexCount;
	private static int[] vertexTypes;
	private static List<Vector2> triangles = new List<Vector2>();
	
	public static List<Vector2> computeTriangles(List<Vector2> polygon) {
		vertices.Clear();
		vertices.AddRange(polygon);
		vertexCount = vertices.Count;

		/* Ensure vertices are in clockwise order. */
		if (!areVerticesClockwise()) {
			vertices.Reverse();
		}

		vertexTypes = new int[vertexCount];
		
		for (int i = 0; i < vertexCount; ++i) {
			vertexTypes[i] = classifyVertex(i);
		}
		
		triangles.Clear();

		while (vertexCount > 3) {
			int earTipIndex = findEarTip();
			cutEarTip(earTipIndex);

			// Only the type of the two vertices adjacent to the clipped vertex can have changed,
			// so no need to reclassify all of them.
			int previousIndex = computePreviousIndex(earTipIndex);
			int nextIndex = earTipIndex == vertexCount ? 0 : earTipIndex;
			vertexTypes[previousIndex] = classifyVertex(previousIndex);
			vertexTypes[nextIndex] = classifyVertex(nextIndex);
		}

		/*
		 * ESpitz: If there are only three verts left to test, or there were only three verts to begin with, we have the final
		 * triangle.
		 */
		if (vertexCount == 3) {
			triangles.AddRange(vertices);
		}
		
		return triangles;
	}
	
	private static bool areVerticesClockwise() {
		float area = 0;
		
		for (int i = 0; i < vertexCount; i++) {
			Vector2 p1 = vertices[i];
			Vector2 p2 = vertices[computeNextIndex(i)];
			
			area += p1.x * p2.y - p2.x * p1.y;
		}
		
		return area < 0;
	}
	
	private static int classifyVertex(int index) {
		Vector2 previousVertex = vertices[computePreviousIndex(index)];
		Vector2 currentVertex = vertices[index];
		Vector2 nextVertex = vertices[computeNextIndex(index)];

		return computeSpannedAreaSign(ref previousVertex, ref currentVertex, ref nextVertex);
	}
	
	private static int computeSpannedAreaSign(ref Vector2 p1, ref Vector2 p2, ref Vector2 p3) {
		float area = 0;
		
		area += p1.x * (p3.y - p2.y);
		area += p2.x * (p1.y - p3.y);
		area += p3.x * (p2.y - p1.y);
		
		return (int)Mathf.Sign(area);
	}
	
	private static int findEarTip () {
		for (int index = 0; index < vertexCount; index++) {
			if (isEarTip(index)) {
				return index;
			}
		}
		
		return desperatelyFindEarTip();
	}
	
	private static int desperatelyFindEarTip () {
		// Desperate mode: if no vertex is an ear tip, we are dealing with a degenerate polygon (e.g. nearly collinear).
		// Note that the input was not necessarily degenerate, but we could have made it so by clipping some valid ears.

		// Idea taken from Martin Held, "FIST: Fast industrial-strength triangulation of polygons", Algorithmica (1998),
		// http://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.115.291

		// Return a convex or tangential vertex if one exists
		for (int index = 0; index < vertexCount; index++) {
			if (vertexTypes[index] != CONCAVE) {
				return index;
			}
		}

		// If all vertices are concave, just return the first one
		return 0;
	}
	
	private static bool isEarTip(int pEarTipIndex) {
		if (vertexTypes[pEarTipIndex] == CONCAVE) {
			return false;
		}

		int previousIndex = computePreviousIndex(pEarTipIndex);
		int nextIndex = computeNextIndex(pEarTipIndex);
		
		Vector2 p1 = vertices[previousIndex];
		Vector2 p2 = vertices[pEarTipIndex];
		Vector2 p3 = vertices[nextIndex];

		// Check if any point is inside the triangle formed by previous, current and next vertices.
		// Only consider vertices that are not part of this triangle, or else we'll always find one inside.
		for (int i = computeNextIndex(nextIndex); i != previousIndex; i = computeNextIndex(i)) {
			// Concave vertices can obviously be inside the candidate ear, but so can tangential vertices
			// if they coincide with one of the triangle's vertices.
			if (vertexTypes[i] != CONVEX) {
				Vector2 v = vertices[i];

				int areaSign1 = computeSpannedAreaSign(ref p1, ref p2, ref v);
				int areaSign2 = computeSpannedAreaSign(ref p2, ref p3, ref v);
				int areaSign3 = computeSpannedAreaSign(ref p3, ref p1, ref v);

				// Because the polygon has clockwise winding order, the area sign will be positive if the point is strictly inside.
				// It will be 0 on the edge, which we want to include as well, because incorrect results can happen if we don't
				// (http://code.google.com/p/libgdx/issues/detail?id=815).
				if (areaSign1 >= 0 && areaSign2 >= 0 && areaSign3 >= 0) {
					return false;
				}
			}
		}
		
		return true;
	}
	
	private static void cutEarTip (int pEarTipIndex) {
		int previousIndex = computePreviousIndex(pEarTipIndex);
		int nextIndex = computeNextIndex(pEarTipIndex);

		triangles.Add(vertices[previousIndex]);
		triangles.Add(vertices[pEarTipIndex]);
		triangles.Add(vertices[nextIndex]);
		
		vertices.RemoveAt(pEarTipIndex);
		
		System.Array.Copy(vertexTypes, pEarTipIndex + 1, vertexTypes, pEarTipIndex, vertexCount - pEarTipIndex - 1);
		
		vertexCount--;
	}
	
	private static int computePreviousIndex (int pIndex) {
		return pIndex == 0 ? vertexCount - 1 : pIndex - 1;
	}

	private static int computeNextIndex (int pIndex) {
		return pIndex == vertexCount - 1 ? 0 : pIndex + 1;
	}
}
