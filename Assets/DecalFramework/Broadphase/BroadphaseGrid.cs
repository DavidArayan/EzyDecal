using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DecalFramework;

// a basic grid implementation for storage and query of point data
public class BroadphaseGrid : ScriptableObject {
	
	[HideInInspector]
	public GridDataPointer[] grid;
	
	[HideInInspector]
	public Vector3 offset;
	
	[HideInInspector]
	public int rows;
	
	[HideInInspector]
	public int cols;
	
	[HideInInspector]
	public int depth;
	
	[HideInInspector]
	public int cellSize;
	
	[HideInInspector]
	public int[] pxyz;
	
	[HideInInspector]
	public List<TriangleData> retList;
	
	void OnEnable() {
		if (grid == null) {
			grid = new GridDataPointer[0];	
		}
		
		if (retList == null) {
			retList = new List<TriangleData>();	
		}
	}
	
	public void setup(int size, int cellSize, List<OptiMesh> data, Vector3 offset) {
		//grid = new GridDataPointer[0];
		retList.Clear();
		
		this.cellSize = cellSize;
		this.offset = offset;
		
		rows = (size + cellSize - 1) / cellSize;
		cols = (size + cellSize - 1) / cellSize;
		depth = (size + cellSize - 1) / cellSize;
		
		grid = new GridDataPointer[rows * cols * depth];
		
		pxyz = new int[(rows * 3) * 2];
		
		// fill with empty data
		for (int x = 0; x < cols; x++) {
			for (int y = 0; y < rows; y++) {
				for (int z = 0; z < depth; z++) {
					int hash = x + rows * (y + depth * z);
					
					GridDataPointer gdp = ScriptableObject.CreateInstance<GridDataPointer>();
					
					grid[hash] = gdp;		
				}
			}
		}
		
		// let us populate our broadphase
		for (int i = 0; i < data.Count; i++) {
			OptiMesh om = data[i];
			TriangleData[] tri = om.getTriangleData();
			
			for (int j = 0; j < tri.Length; j++) {
				Vector3 center = tri[j].getTransCenter() - offset;
		
				// map our coordinates
				int topLeftX = Mathf.Max(0, Mathf.FloorToInt(center.x / cellSize));
				int topLeftY = Mathf.Max(0, Mathf.FloorToInt(center.y / cellSize));
				int topLeftZ = Mathf.Max(0, Mathf.FloorToInt(center.z / cellSize));
				
				int bottomRightX = Mathf.Min(cols - 1, Mathf.CeilToInt((center.x + tri[j].getRadius() - 1) / cellSize));
				int bottomRightY = Mathf.Min(rows - 1, Mathf.CeilToInt((center.y + tri[j].getRadius() - 1) / cellSize));
				int bottomRightZ = Mathf.Min(depth - 1, Mathf.CeilToInt((center.z + tri[j].getRadius() - 1) / cellSize));
				
				//Debug.Log("TOP" + topLeftX + " " + topLeftY + " " + topLeftZ);
				//Debug.Log("BOT" + bottomRightX + " " + bottomRightY + " " + bottomRightZ);
				
				for (int x = topLeftX; x <= bottomRightX; x++) {
					for (int y = topLeftY; y <= bottomRightY; y++) {
						for (int z = topLeftZ; z <= bottomRightZ; z++) {
							int hash = x + rows * (y + depth * z);
							
							grid[hash].dataPt.Add(tri[j]);
						}
					}
				}
			}
		}
			
			//for (int j = 0; j < tri.Length; j++) {
				//Vector3 triCent = tri[j].getTransCenter();
				
				/*int cellX = (int)((triCent.x - offset.x) / cellSize);
				int cellY = (int)((triCent.y - offset.y) / cellSize);
				int cellZ = (int)((triCent.z - offset.z) / cellSize);*/
				
				//Debug.Log("Adding to cell X: " + cellX + " Y: " + cellY + " Z: " + cellZ);
				
				//grid[cellX,cellY,cellZ].Add(new GridDataPointer(tri[j]));
				// do same with other triangle points
				
				//Vector3[] triTPt = tri[j].getTransformedPoints();
				
				// get all triangles and convert them to grid-coordinate frame by applying offset
				/*Vector3 pt1 = new Vector3((triTPt[0].x - offset.x) / cellSize,(triTPt[0].y - offset.y) / cellSize,(triTPt[0].z - offset.z) / cellSize);
				Vector3 pt2 = new Vector3((triTPt[1].x - offset.x) / cellSize,(triTPt[1].y - offset.y) / cellSize,(triTPt[1].z - offset.z) / cellSize);
				Vector3 pt3 = new Vector3((triTPt[2].x - offset.x) / cellSize,(triTPt[2].y - offset.y) / cellSize,(triTPt[2].z - offset.z) / cellSize);*/
				
				// plot all cells between point 1 and point 2
				//int length1 = plotAll(pt1.x, pt1.y, pt1.z, pt2.x, pt2.y, pt2.z, pxyz);
				/*int length1 = plotAll(triTPt[0].x, triTPt[0].y, triTPt[0].z, triTPt[1].x, triTPt[1].y, triTPt[1].z, pxyz);
				
				if (length1 > 0) {
					for (int k = 0; k < length1 - 3; k+=3) {
						// add this triangle to grid locations
						grid[pxyz[k],pxyz[k+1],pxyz[k+2]].Add(new GridDataPointer(tri[j]));
					}
				}*/
				
				// plot all cells between point 2 and point 3
				//int length2 = plotAll(pt2.x, pt2.y, pt2.z, pt3.x, pt3.y, pt3.z, pxyz);
				/*int length2 = plotAll(triTPt[1].x, triTPt[1].y, triTPt[1].z, triTPt[2].x, triTPt[2].y, triTPt[2].z, pxyz);
				
				if (length2 > 0) {
					for (int k = 0; k < length2 - 3; k+=3) {
						// add this triangle to grid locations
						grid[pxyz[k],pxyz[k+1],pxyz[k+2]].Add(new GridDataPointer(tri[j]));
					}
				}*/
				
				// plot all cells between point 3 and point 1
				//int length3 = plotAll(pt3.x, pt3.y, pt3.z, pt1.x, pt1.y, pt1.z, pxyz);
				/*int length3 = plotAll(triTPt[2].x, triTPt[2].y, triTPt[2].z, triTPt[0].x, triTPt[0].y, triTPt[0].z, pxyz);
				
				if (length3 > 0) {
					for (int k = 0; k < length3 - 3; k+=3) {
						// add this triangle to grid locations
						grid[pxyz[k],pxyz[k+1],pxyz[k+2]].Add(new GridDataPointer(tri[j]));
					}
				}*/
				
				/*for (int k = 0; k < triTPt.Length; k++) {
					cellX = (int)((triTPt[k].x - offset.x) / cellSize);
					cellY = (int)((triTPt[k].y - offset.y) / cellSize);
					cellZ = (int)((triTPt[k].z - offset.z) / cellSize);
				
					grid[cellX,cellY,cellZ].Add(new GridDataPointer(tri[j]));
				}*/
			//}
	}
	
	public void clear() {
		grid = new GridDataPointer[0];
		retList.Clear();
	}
	
	public List<TriangleData> getTrianglesInOOBB(OOBB col) {
		// clear previous list
		retList.Clear();
		
		Vector3 center = col.center - offset;
		
		// map our coordinates
		int topLeftX = Mathf.Max(0, Mathf.FloorToInt(center.x / cellSize));
		int topLeftY = Mathf.Max(0, Mathf.FloorToInt(center.y / cellSize));
		int topLeftZ = Mathf.Max(0, Mathf.FloorToInt(center.z / cellSize));
		
		int bottomRightX = Mathf.Min(cols - 1, Mathf.CeilToInt((center.x + col.aabbWidth - 1) / cellSize));
		int bottomRightY = Mathf.Min(rows - 1, Mathf.CeilToInt((center.y + col.aabbHeight - 1) / cellSize));
		int bottomRightZ = Mathf.Min(depth - 1, Mathf.CeilToInt((center.z + col.aabbLength - 1) / cellSize));
		
		//Debug.Log("TOP" + topLeftX + " " + topLeftY + " " + topLeftZ);
		//Debug.Log("BOT" + bottomRightX + " " + bottomRightY + " " + bottomRightZ);
		if (grid.Length == 0) {
			return retList;	
		}
		
		//int count = 0;
		for (int x = topLeftX; x <= bottomRightX; x++) {
			for (int y = topLeftY; y <= bottomRightY; y++) {
				for (int z = topLeftZ; z <= bottomRightZ; z++) {
					int hash = x + rows * (y + depth * z);
					//Debug.Log(x + " " + y + " " + z);
					//Debug.Log("Getting into Grid: " + x + " " + y + " " + z);
					// there is data in this grid
					if (grid[hash].dataPt.Count > 0) {
						// loop through
						//count++;
						foreach (TriangleData dat in grid[hash].dataPt) {
							if (!retList.Contains(dat)) {
								retList.Add(dat);
							}
						}
					}
				}
			}
		}
		
		//Debug.Log(count + " " + retList.Count);
		
		// finally, return our list of triangles
		return retList;
	}
	
	// algorithm as presented in book real time collision detection - originally written in c for 2D plotting
	// this function will return upon arriving at the first non-empty cell in the grid
	private int[] plot(float x1, float y1, float z1, float x2, float y2, float z2, int[] xyz) {
		x1 = x1 - offset.x;
		y1 = y1 - offset.y;
		z1 = z1 - offset.z;
		
		x2 = x2 - offset.x;
		y2 = y2 - offset.y;
		z2 = z2 - offset.z;
		
		int i = Mathf.FloorToInt(x1 / cellSize);
		int j = Mathf.FloorToInt(y1 / cellSize);
		int k = Mathf.FloorToInt(z1 / cellSize);
		
		int iend = Mathf.FloorToInt(x2 / cellSize);
		int jend = Mathf.FloorToInt(y2 / cellSize);
		int kend = Mathf.FloorToInt(z2 / cellSize);
		
		int di = ((x1 < x2) ? 1 : ((x1 > x2) ? -1: 0));
		int dj = ((y1 < y2) ? 1 : ((y1 > y2) ? -1: 0));
		int dk = ((z1 < z2) ? 1 : ((z1 > z2) ? -1: 0));
		
		float minx = cellSize * Mathf.Floor(x1 / cellSize);
		float maxx = minx + cellSize;
		float tx = ((x1 > x2) ? (x1 - minx) : (maxx - x1)) / Mathf.Abs(x2 - x1);
		
		float miny = cellSize * Mathf.Floor(y1 / cellSize);
		float maxy = miny + cellSize;
		float ty = ((y1 > y2) ? (y1 - miny) : (maxy - y1)) / Mathf.Abs(y2 - y1);
		
		float minz = cellSize * Mathf.Floor(z1 / cellSize);
		float maxz = minz + cellSize;
		float tz = ((z1 > z2) ? (z1 - minz) : (maxz - z1)) / Mathf.Abs(z2 - z1);
		
		float deltatx = cellSize / Mathf.Abs(x2 - x1);
		float deltaty = cellSize / Mathf.Abs(y2 - y1);
		float deltatz = cellSize / Mathf.Abs(z2 - z1);
		
		while(true) {
			int hash = i + rows * (j + depth * k);
			
			int val = grid[hash].dataPt.Count;
			
			if (val > 0) {
				xyz[0] = i;
				xyz[1] = j;
				xyz[2] = k;
				
				return xyz;
			}
			
			if (tx <= ty && tx <= tz) {
				if (i == iend) {
					return xyz;	
				}
				
				tx += deltatx;
				i += di;
			}
			else if (ty <= tx && ty <= tz) {
				if (j == jend) {
					return xyz;	
				}
				
				ty += deltaty;
				j += dj;
			}
			else {
				if (k == kend) {
					return xyz;	
				}
				
				tz += deltatz;
				k += dk;
			}
		}
	}
	
	// algorithm as presented in book real time collision detection - originally written in c for 2D plotting
	// modified to return all cells within a start and end point for initial data placement.
	private int plotAll(float x1, float y1, float z1, float x2, float y2, float z2, int[] xyz) {
		x1 = x1 - offset.x;
		y1 = y1 - offset.y;
		z1 = z1 - offset.z;
		
		x2 = x2 - offset.x;
		y2 = y2 - offset.y;
		z2 = z2 - offset.z;
		
		int i = Mathf.FloorToInt(x1 / cellSize);
		int j = Mathf.FloorToInt(y1 / cellSize);
		int k = Mathf.FloorToInt(z1 / cellSize);
		
		int iend = Mathf.FloorToInt(x2 / cellSize);
		int jend = Mathf.FloorToInt(y2 / cellSize);
		int kend = Mathf.FloorToInt(z2 / cellSize);
		
		int di = ((x1 < x2) ? 1 : ((x1 > x2) ? -1: 0));
		int dj = ((y1 < y2) ? 1 : ((y1 > y2) ? -1: 0));
		int dk = ((z1 < z2) ? 1 : ((z1 > z2) ? -1: 0));
		
		float minx = cellSize * Mathf.Floor(x1 / cellSize);
		float maxx = minx + cellSize;
		float tx = ((x1 > x2) ? (x1 - minx) : (maxx - x1)) / Mathf.Abs(x2 - x1);
		
		float miny = cellSize * Mathf.Floor(y1 / cellSize);
		float maxy = miny + cellSize;
		float ty = ((y1 > y2) ? (y1 - miny) : (maxy - y1)) / Mathf.Abs(y2 - y1);
		
		float minz = cellSize * Mathf.Floor(z1 / cellSize);
		float maxz = minz + cellSize;
		float tz = ((z1 > z2) ? (z1 - minz) : (maxz - z1)) / Mathf.Abs(z2 - z1);
		
		float deltatx = cellSize / Mathf.Abs(x2 - x1);
		float deltaty = cellSize / Mathf.Abs(y2 - y1);
		float deltatz = cellSize / Mathf.Abs(z2 - z1);
		
		int index = 0;
		
		while(true) {
			// bounds checking to ensure decal still inside grid
			if (i < 0 || j < 0 || k < 0 || i > (rows - 1) || j > (cols - 1) || k > (depth - 1)) {
				return index;	
			}
			
			xyz[index] = i;
			xyz[index + 1] = j;
			xyz[index + 2] = k;
			
			index += 3;
			
			if (tx <= ty && tx <= tz) {
				if (i == iend) {
					return index;	
				}
				
				tx += deltatx;
				i += di;
				
				// early return if delta values go out of scope
				if (tx < 0) {
					return index;	
				}
			}
			else if (ty <= tx && ty <= tz) {
				if (j == jend) {
					return index;	
				}
				
				ty += deltaty;
				j += dj;
				
				// early return if delta values go out of scope
				if (ty < 0) {
					return index;	
				}
			}
			else {
				if (k == kend) {
					return index;	
				}
				
				tz += deltatz;
				k += dk;
				
				// early return if delta values go out of scope
				if (tz < 0) {
					return index;	
				}
			}
		}
	}
	
	public static bool lineIntersect3D(Vector3 linea1, Vector3 linea2, Vector3 lineb1, Vector3 lineb2,ref Vector3 ip, float tol) {
		Vector3 da = linea2 - linea1;
		Vector3 db = lineb2 - lineb1;
		Vector3 dc = lineb1 - linea1;
		
		Vector3 dadbcross = Vector3.Cross(da,db);
		
		float dotTol = Vector3.Dot(dc, dadbcross);
		
		//Debug.Log(dotTol);
		
		float toln = tol * 100;
		
		if (!NearestPointTest.valueWithinRange(-toln, toln, dotTol)) {
			return false;
		}
		
		float s = Vector3.Dot(Vector3.Cross(dc,db),dadbcross) / dadbcross.sqrMagnitude;
		
		if (s >= 0.0f && s <= 1.0f) {
			Vector3 mp = da * s;
			ip = linea1 + mp;
			return true;
		}
		
		return false;
	}
	
	public bool intersectPt(Vector3 point, Vector3 direction, ref Vector3 r) {
		pxyz[0] = -1;		
		int length = plotAll(point.x, point.y, point.z, point.x + direction.x, point.y + direction.y, point.z + direction.z, pxyz);
		
		if (pxyz[0] == -1) {
			return false;
		}
		
		for (int i = 0; i < length; i+=3) {
			
			int hash = pxyz[i] + rows * (pxyz[i + 1] + depth * pxyz[i + 2]);
			
			//List<GridDataPointer> list = grid[pxyz[i],pxyz[i + 1],pxyz[i + 2]];
			
			if (grid[hash].dataPt.Count > 0) {
				Vector3 nearestPt = Vector3.zero;
				float nearestSqDist = float.PositiveInfinity;
				bool ret = false;
				
				for (int j = 0; j < grid[hash].dataPt.Count; j++) {
					
					TriangleData dp = grid[hash].dataPt[j];
					
					Vector3[] tPoints = dp.getTransformedPoints();

                    Vector3 nPt = point + direction;
					
					if (TriangleTests.IntersectLineTriangle(ref point, ref nPt, ref tPoints[0], ref tPoints[1], ref tPoints[2], ref r)) {
						ret = true;
						float npSqDist = (point - r).sqrMagnitude;
						
						if (npSqDist < nearestSqDist) {
							nearestSqDist = npSqDist;
							nearestPt = r;
						}
					}
				}
				
				if (ret) {
					r = nearestPt;
					return true;
				}
			}
		}
		
		return false;
	}
	
	public Vector3 closestPt(Vector3 point, Vector3 direction) {
		pxyz[0] = -1;
		
		//int[] data = plot(point.x, point.y, point.z, point.x + direction.x, point.y + direction.y, point.z + direction.z, pxyz);
		
		int length = plotAll(point.x, point.y, point.z, point.x + direction.x, point.y + direction.y, point.z + direction.z, pxyz);
		
		if (pxyz[0] == -1) {
			return Vector3.zero;
		}
		
		Vector3 nearestPt = Vector3.zero;
		float nearestSqDist = float.PositiveInfinity;
		
		for (int i = 0; i < length; i+=3) {
			//Debug.Log("x: " + pxyz[i] + " y: " + pxyz[i + 1] + " z: " + pxyz[i + 2]);
			
			int hash = pxyz[i] + rows * (pxyz[i + 1] + depth * pxyz[i + 2]);
			
			if (grid[hash].dataPt.Count > 0) {
						
				for (int j = 0; j < grid[hash].dataPt.Count; j++) {
					TriangleData dp = grid[hash].dataPt[j];
					
					Vector3[] tPoints = dp.getTransformedPoints();

                    Vector3 np = new Vector3();

                    TriangleTests.ClosestPointTriangle(ref tPoints[0], ref tPoints[1], ref tPoints[2], ref point, ref np);
					
					float npSqDist = (point - np).sqrMagnitude;
						
					if (npSqDist < nearestSqDist) {
						nearestSqDist = npSqDist;
						nearestPt = np;
					}
				}		
			}
		}
		
		return nearestPt;
	}
}
