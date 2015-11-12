using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridDataPointer : ScriptableObject {
	
	[HideInInspector]
	public List<TriangleData> dataPt;
	
	void OnEnable() {
		if (dataPt == null) {
			dataPt = new List<TriangleData>();	
		}
	}
}
