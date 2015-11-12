using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DictionaryTest : MonoBehaviour {
	
	public Dictionary<Vector3, int> vectorDict = new Dictionary<Vector3, int>();

	// Use this for initialization
	void Start () {
		Vector3 v1 = new Vector3(0.5f,0.5f,0.5f);
		Vector3 v2 = new Vector3(1f,0.5f,1f);
		Vector3 v3 = new Vector3(0,0.002f,0);
		
		vectorDict.Add(v1,1);
		vectorDict.Add(v2,1);
		vectorDict.Add(v3,1);
		
		Vector3 tV1 = new Vector3(0.5f,0.5f,0.5f);
		Vector3 tV2 = new Vector3(1f,0.5f,1f);
		Vector3 tV3 = new Vector3(0,0.002f,0);
		Vector3 tV4 = new Vector3(0,1,0);
		Vector3 tV5 = new Vector3(1,0,2);
		
		Debug.Log("Testing");
		
		if (vectorDict.ContainsKey(tV1)) {
			Debug.Log("Passed");	
		}
		else {
			Debug.Log("Failed");
		}
		
		if (vectorDict.ContainsKey(tV2)) {
			Debug.Log("Passed");	
		}
		else {
			Debug.Log("Failed");
		}
		
		if (vectorDict.ContainsKey(tV3)) {
			Debug.Log("Passed");	
		}
		else {
			Debug.Log("Failed");
		}
		
		if (!vectorDict.ContainsKey(tV4)) {
			Debug.Log("Passed");	
		}
		else {
			Debug.Log("Failed");
		}
		
		if (!vectorDict.ContainsKey(tV5)) {
			Debug.Log("Passed");	
		}
		else {
			Debug.Log("Failed");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
