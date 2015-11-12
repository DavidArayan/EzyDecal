using UnityEngine;
using System.Collections;

public class MeshCreator : ScriptableObject {
	
	[HideInInspector]
	public Vector3[] vertices;
	
	[HideInInspector]
	public Vector3[] transVert;
	
	[HideInInspector]
	public Vector2[] uvs;
	
	[HideInInspector]
	public int[] triangles;
	
	public MeshCreator() {		
		subdivide(1);
	}
	
	public void subdivide(int sub) {		
		int widthSegments = (sub > 0) ? sub : 1;
		int lengthSegments = (sub > 0) ? sub : 1;
		
		float length = 1.0f;
		float width = 1.0f;
		
		int hCount2 = widthSegments+1;
		int vCount2 = lengthSegments+1;
		int numTriangles = widthSegments * lengthSegments * 6;
		int numVertices = hCount2 * vCount2;
		
		vertices = new Vector3[numVertices];
		transVert = new Vector3[numVertices];
		
		uvs = new Vector2[numVertices];
		triangles = new int[numTriangles];
		
		int index = 0;
		
		float uvFactorX = 1.0f / widthSegments;
		float uvFactorY = 1.0f / lengthSegments;
		float scaleX = width / widthSegments;
		float scaleY = length / lengthSegments;
		
		for (float y = 0.0f; y < vCount2; y++) {
			for (float x = 0.0f; x < hCount2; x++) {
				vertices[index] = new Vector3(x * scaleX - width / 2f, 0.0f, y * scaleY - length / 2f);
				transVert[index] = vertices[index];
				
				uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
			}
		}
		
		index = 0;
		
		for (int y = 0; y < lengthSegments; y++) {
			for (int x = 0; x < widthSegments; x++) {
				triangles[index]   = (y     * hCount2) + x;
				triangles[index+1] = ((y + 1) * hCount2) + x;
				triangles[index+2] = (y     * hCount2) + x + 1;
				
				triangles[index+3] = ((y + 1) * hCount2) + x;
				triangles[index+4] = ((y + 1) * hCount2) + x + 1;
				triangles[index+5] = (y     * hCount2) + x + 1;
				index += 6;
			}
		}
	}
	
	public Vector3[] getVertices() {
		return vertices;	
	}
	
	public Vector3[] getTransVert() {
		return transVert;	
	}
	
	public int[] getIndices() {
		return triangles;	
	}
	
	public void fill(ref MeshFilter m) {
		m.sharedMesh.Clear();
		
		m.sharedMesh.vertices = transVert;
		m.sharedMesh.uv = uvs;
		m.sharedMesh.triangles = triangles;
		m.sharedMesh.RecalculateNormals();
		
		calculateMeshTangents(ref m);
	}
	
	public static void calculateMeshTangents(ref Mesh mesh) {
		
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for (long a = 0; a < triangleCount; a += 3) {
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f / div;
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		for (long a = 0; a < vertexCount; ++a) {
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		mesh.tangents = tangents;
	}
	
	public static void calculateMeshTangents(ref MeshFilter mesh) {
		
		int[] triangles = mesh.sharedMesh.triangles;
		Vector3[] vertices = mesh.sharedMesh.vertices;
		Vector2[] uv = mesh.sharedMesh.uv;
		Vector3[] normals = mesh.sharedMesh.normals;
		
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		
		Vector4[] tangents = new Vector4[vertexCount];
		
		for (long a = 0; a < triangleCount; a += 3) {
			long i1 = triangles[a + 0];
			long i2 = triangles[a + 1];
			long i3 = triangles[a + 2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f / div;
			
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		for (long a = 0; a < vertexCount; ++a) {
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		
		mesh.sharedMesh.tangents = tangents;
	}
}
