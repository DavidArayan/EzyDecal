using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EarClipTriangulatorTest : MonoBehaviour {
	
	public GameObject[] objs;
	
	void OnDrawGizmos() {
		if (objs.Length < 3) {
			return;	
		}
		
		Vector3 n = Vector3.Cross(objs[1].transform.position - objs[0].transform.position, objs[2].transform.position - objs[0].transform.position);
		n.Normalize();
		
		// draw original objects in red
		Vector3 size1 = new Vector3(0.05f,0.05f,0.05f);
		Vector3 size2 = new Vector3(0.1f,0.1f,0.1f);
		Gizmos.color = new Color(1,0,0,1);
		
		for (int i = 0; i < objs.Length; i++) {
			Gizmos.DrawCube(objs[i].transform.position, size1);
		}
		
		List<Vector2> positions = new List<Vector2>();
		
		for (int i = 0; i < objs.Length; i++) {
			positions.Add(new Vector2(objs[i].transform.position.x, objs[i].transform.position.z));
		}
		
		List<Vector2> tris = EarClipTriangulator.computeTriangles(positions);
		
		Gizmos.color = new Color(0,1,0,1);
		
		for (int i = 0; i < tris.Count; i++) {
			Gizmos.DrawCube(new Vector3(tris[i].x, 0.0f, tris[i].y), size2);
		}
		
		if (tris.Count == 0) {
			return;	
		}
		
		Vector2 line1 = tris[0];
		
		for (int i = 1; i < tris.Count; i++) {
			Gizmos.DrawLine(new Vector3(line1.x, 0.0f, line1.y), new Vector3(tris[i].x, 0.0f, tris[i].y));
			line1 = tris[i];
		}
		
		Gizmos.DrawLine(new Vector3(tris[0].x, 0.0f, tris[0].y), new Vector3(tris[tris.Count - 1].x, 0.0f, tris[tris.Count - 1].y));
	}
}
