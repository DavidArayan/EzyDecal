using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BarycentricTest : MonoBehaviour {
	
	public GameObject tri1;
	public GameObject tri2;
	public GameObject tri3;
	public GameObject test;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float u = 0.0f;
		float v = 0.0f;
		float w = 0.0f;
		
		if (tri1 != null && tri2 != null && tri3 != null && test != null) {
			NearestPointTest.barycentric(tri1.transform.position, tri2.transform.position, tri3.transform.position, test.transform.position, ref u, ref v, ref w);
			
			Debug.Log("U: " + u + " V: " + v + " W: " + w + " TOTAL: " + (u + v + w));
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(1,0,0,1);
		if (tri1 != null && tri2 != null && tri3 != null && test != null) {
			Gizmos.DrawLine(tri1.transform.position, tri2.transform.position);
			Gizmos.DrawLine(tri2.transform.position, tri3.transform.position);
			Gizmos.DrawLine(tri3.transform.position, tri1.transform.position);
			Gizmos.DrawCube(test.transform.position, Vector3.one);
		}
	}
}