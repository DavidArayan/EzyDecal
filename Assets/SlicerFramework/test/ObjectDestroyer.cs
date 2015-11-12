using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour {
	
	public SlicerStore slicer;
	
	void OnCollisionEnter(Collision collision) {
		if (slicer != null) {
			slicer.instantiate();
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
