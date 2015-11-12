using UnityEngine;
using System.Collections;

[System.Serializable]
public class NDPlane {
	
	public Vector3 n;
	public float d;
	
	public NDPlane(Vector3 dir, Vector3 point) {
		this.n = dir;
		this.d = Vector3.Dot(n,point);
	}
	
	public void computeD(Vector3 point) {
		this.d = Vector3.Dot(n,point);	
	}
	
	public void setValues(Vector3 dir, Vector3 point) {
		this.n = dir;
		this.d = Vector3.Dot(n,point);
	}
	
	public NDPlane() {
		d = 0.0f;
	}
}
