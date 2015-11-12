using UnityEngine;
using System.Collections;

public class SlicerPlane : MonoBehaviour {
	void OnDrawGizmos() {
		Vector3 normal = transform.TransformDirection(Vector3.up);
		
		Vector3 pt1 = new Vector3(-1,0,-1);
		Vector3 pt2 = new Vector3(1,0,-1);
		Vector3 pt3 = new Vector3(1,0,1);
		Vector3 pt4 = new Vector3(-1,0,1);
		
		pt1 = transform.TransformPoint(pt1);
		pt2 = transform.TransformPoint(pt2);
		pt3 = transform.TransformPoint(pt3);
		pt4 = transform.TransformPoint(pt4);
		
		Gizmos.color = new Color(0,1,0,1);
		
		Gizmos.DrawLine(pt1,pt2);
		Gizmos.DrawLine(pt2,pt3);
		Gizmos.DrawLine(pt3,pt4);
		Gizmos.DrawLine(pt4,pt1);
		
		Gizmos.color = new Color(1,0,0,1);
		Gizmos.DrawLine(transform.position, transform.position + normal);
		
		Gizmos.DrawLine(pt1, pt1 - transform.TransformDirection(Vector3.forward));
		Gizmos.DrawLine(pt1, pt1 - transform.TransformDirection(Vector3.right));
		
		Gizmos.DrawLine(pt2, pt2 - transform.TransformDirection(Vector3.forward));
		Gizmos.DrawLine(pt2, pt2 + transform.TransformDirection(Vector3.right));
		
		Gizmos.DrawLine(pt3, pt3 + transform.TransformDirection(Vector3.forward));
		Gizmos.DrawLine(pt3, pt3 + transform.TransformDirection(Vector3.right));
		
		Gizmos.DrawLine(pt4, pt4 + transform.TransformDirection(Vector3.forward));
		Gizmos.DrawLine(pt4, pt4 - transform.TransformDirection(Vector3.right));
		
		Gizmos.color = new Color(0,0,1,1);
		
		Gizmos.DrawLine(pt1,transform.position);
		Gizmos.DrawLine(pt2,transform.position);
		Gizmos.DrawLine(pt3,transform.position);
		Gizmos.DrawLine(pt4,transform.position);
	}
}
