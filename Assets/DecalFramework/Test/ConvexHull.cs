using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConvexHull : MonoBehaviour {
	
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
		
		List<Vector3> positions = new List<Vector3>();
		List<Vector3> tris = new List<Vector3>();
		
		for (int i = 0; i < objs.Length; i++) {
			positions.Add(objs[i].transform.position);
		}
		
		NearestPointTest.triangulate(positions, tris, n);
		
		Gizmos.color = new Color(0,1,0,1);
		
		for (int i = 0; i < tris.Count; i++) {
			Gizmos.DrawCube(tris[i], size2);
		}
		
		if (tris.Count == 0) {
			return;	
		}
		
		Vector3 line1 = tris[0];
		
		for (int i = 1; i < tris.Count; i++) {
			Gizmos.DrawLine(line1, tris[i]);
			line1 = tris[i];
		}
		
		Gizmos.DrawLine(tris[0], tris[tris.Count - 1]);
		
		/*Gizmos.color = new Color(0,0,1,1);
		
		line1 = tris[0];
		
		for (int i = 1; i < tris.Count - 1; i++) {
			Gizmos.DrawLine(line1, tris[i]);
			Gizmos.DrawLine(tris[i], tris[i + 1]);
			Gizmos.DrawLine(tris[i + 1], line1);
		}*/
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
