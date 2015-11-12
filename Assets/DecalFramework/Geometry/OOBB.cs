using UnityEngine;
using System.Collections;

[System.Serializable]
public class OOBB {
	
	public Vector3[] aabbCoordsTrans;
	
	public Vector3 center;

	public float aabbLength;
	
	public float aabbWidth;
	
	public float aabbHeight;
	
	public Vector3[] oobbCoords;
	
	public Vector3[] oobbCoordsTrans;
	
	public OOBB() {
		aabbCoordsTrans = new Vector3[8];
		center = Vector3.zero;
		aabbLength = 2;
		aabbWidth = 2;
		aabbHeight = 2;
		oobbCoords = new Vector3[8];
		oobbCoordsTrans = new Vector3[8];
		
		oobbCoords[0] = new Vector3(-1,-1,-1);
		oobbCoords[1] = new Vector3(1,-1,-1);
		oobbCoords[2] = new Vector3(-1,1,-1);
		oobbCoords[3] = new Vector3(1,1,-1);
		oobbCoords[4] = new Vector3(-1,-1,1);
		oobbCoords[5] = new Vector3(1,-1,1);
		oobbCoords[6] = new Vector3(-1,1,1);
		oobbCoords[7] = new Vector3(1,1,1);
	}
	
	void OnEnable() {
		aabbCoordsTrans = new Vector3[8];
		center = Vector3.zero;
		aabbLength = 2;
		aabbWidth = 2;
		aabbHeight = 2;
		oobbCoords = new Vector3[8];
		oobbCoordsTrans = new Vector3[8];
		
		oobbCoords[0] = new Vector3(-1,-1,-1);
		oobbCoords[1] = new Vector3(1,-1,-1);
		oobbCoords[2] = new Vector3(-1,1,-1);
		oobbCoords[3] = new Vector3(1,1,-1);
		oobbCoords[4] = new Vector3(-1,-1,1);
		oobbCoords[5] = new Vector3(1,-1,1);
		oobbCoords[6] = new Vector3(-1,1,1);
		oobbCoords[7] = new Vector3(1,1,1);
	}
	
	public void update(GameObject obj) {
		center = obj.transform.position;
		
		// transform the OOBB box
		for (int i = 0; i < 8; i++) {
			oobbCoordsTrans[i] = obj.transform.TransformPoint(oobbCoords[i]);
		}
		
		// perform a reconstruction on the AABB box
		
		float lowestX = float.PositiveInfinity;
		float highestX = float.NegativeInfinity;
		float lowestY = float.PositiveInfinity;
		float highestY = float.NegativeInfinity;
		float lowestZ = float.PositiveInfinity;
		float highestZ = float.NegativeInfinity;	
		
		for (int i = 0; i < 8; i++) {
			Vector3 pt = oobbCoordsTrans[i];
			
			// x value
			if (pt.x < lowestX) {
				lowestX = pt.x;
			}
			
			if (pt.x > highestX) {
				highestX = pt.x;
			}
			
			// y value
			if (pt.y < lowestY) {
				lowestY = pt.y;
			}
			
			if (pt.y > highestY) {
				highestY = pt.y;
			}
			
			// z value
			if (pt.z < lowestZ) {
				lowestZ = pt.z;
			}
			
			if (pt.z > highestZ) {
				highestZ = pt.z;
			}
		}
		
		aabbLength = Mathf.Abs(lowestX - highestX);
		aabbWidth = Mathf.Abs(lowestZ - highestZ);
		aabbHeight = Mathf.Abs(lowestY - highestY);
		
		float halfLength = aabbLength / 2;
		float halfWidth = aabbWidth / 2;
		float halfHeight = aabbHeight / 2;
		
		aabbCoordsTrans[0] = new Vector3(center.x - halfLength, center.y - halfHeight, center.z - halfWidth);
		aabbCoordsTrans[1] = new Vector3(center.x + halfLength, center.y - halfHeight, center.z - halfWidth);
		aabbCoordsTrans[2] = new Vector3(center.x - halfLength, center.y + halfHeight, center.z - halfWidth);
		aabbCoordsTrans[3] = new Vector3(center.x + halfLength, center.y + halfHeight, center.z - halfWidth);
		aabbCoordsTrans[4] = new Vector3(center.x - halfLength, center.y - halfHeight, center.z + halfWidth);
		aabbCoordsTrans[5] = new Vector3(center.x + halfLength, center.y - halfHeight, center.z + halfWidth);
		aabbCoordsTrans[6] = new Vector3(center.x - halfLength, center.y + halfHeight, center.z + halfWidth);
		aabbCoordsTrans[7] = new Vector3(center.x + halfLength, center.y + halfHeight, center.z + halfWidth);
	}
}
