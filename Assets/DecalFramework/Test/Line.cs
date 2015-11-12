using UnityEngine;
using System.Collections;

public class Line : MonoBehaviour {
	public GameObject point1;
	public GameObject point2;
	
	public Line another;
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(1,1,1,1);
		
		if (point1 != null) {
			Gizmos.DrawCube(point1.transform.position, new Vector3(1,1,1));	
		}
		
		if (point2 != null) {
			Gizmos.DrawCube(point2.transform.position, new Vector3(1,1,1));	
		}
		
		if (point1 != null && point2 != null) {
			Gizmos.color = new Color(0,1,0,1);
			Gizmos.DrawLine(point1.transform.position,point2.transform.position);
		}
		
		if (another != null) {
			if (another.point1 != null && another.point2 != null) {
				Gizmos.color = new Color(1,0,0,1);
				
				Vector3 pt = new Vector3();
				
				if (BroadphaseGrid.lineIntersect3D(point1.transform.position,
													point2.transform.position,
													another.point1.transform.position,
													another.point2.transform.position,
													ref pt,
													1.0f))
				{
					Gizmos.DrawCube(pt, new Vector3(1,1,1));	
				}
			}
		}
	}
}
