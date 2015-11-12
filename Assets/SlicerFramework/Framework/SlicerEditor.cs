using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SlicerEditor : EditorWindow {
	private GameObject objectToSlice;
	private List<SlicerPlane> slicers = new List<SlicerPlane>();
	private List<SlicerPlane> randomSlicers = new List<SlicerPlane>();
	private SlicerPlane extraSlicer;
	private bool random;
	private bool rigidbody;
	private bool collidable;
	private bool isConvex;
	private bool isHallow;
	private bool isBlended;
	private int sliceVal = 1;
	private bool showSlicers;
	private Material blendedMat = null;
	
	[MenuItem("Window/SlicerEditor")]
	static void ShowWindow() {
		EditorWindow window = EditorWindow.GetWindow(typeof(SlicerEditor));
						
		window.autoRepaintOnSceneChange = true;
		window.position = new Rect(400,400,300,600);
		window.maxSize = new Vector2(300,600);
		window.minSize = new Vector2(300,600);
	}
	
	static void Init() {
		EditorWindow window = EditorWindow.GetWindow(typeof(SlicerEditor));
				
		window.autoRepaintOnSceneChange = true;
		window.position = new Rect(400,400,300,600);
		window.maxSize = new Vector2(300,600);
		window.minSize = new Vector2(300,600);
	}
	
	void OnGUI() {
		GUILayout.BeginArea( new Rect(0, 0, 300, 600));
		EditorGUILayout.HelpBox("Object Slicer Framework and Editor \nCreated by David Arayan 2013",MessageType.None);
		
		objectToSlice = EditorGUILayout.ObjectField(objectToSlice, typeof(GameObject), true) as GameObject;
		
		if (objectToSlice == null) {
			EditorGUILayout.HelpBox("GameObject is Null \nSelect GameObject to expand further options",MessageType.Info);
			GUILayout.EndArea();
			return;
		}
		
		if (objectToSlice.GetComponent<SlicerStore>() == null) {
			objectToSlice.AddComponent<SlicerStore>();
		}
				
		SlicerStore store = objectToSlice.GetComponent<SlicerStore>();
		
		isConvex = store.isConvex;
		isHallow = store.isHallow;
		
		// this is where we set our slice options
		GUILayout.BeginVertical();
		
		EditorGUILayout.HelpBox("Slice Generator Options",MessageType.None);
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox("Object Convex",MessageType.None);
		
		isConvex = EditorGUILayout.Toggle(isConvex);
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox("Object Hallow",MessageType.None);
		
		if (isConvex) {
			isHallow = EditorGUILayout.Toggle(isHallow);	
		}
		else {
			isHallow = EditorGUILayout.Toggle(true);
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox("Material Blend",MessageType.None);
		
		isBlended = EditorGUILayout.Toggle(isBlended);
		
		GUILayout.EndHorizontal();
		
		if (!isBlended) {
			EditorGUILayout.HelpBox("Cross Section Material",MessageType.None);
			blendedMat = EditorGUILayout.ObjectField(blendedMat, typeof(Material), true) as Material;
		}
		else {
			blendedMat = null;	
		}
		
		store.isHallow = isHallow;
		store.isConvex = isConvex;
		
		GUILayout.EndVertical();
		
		EditorGUILayout.HelpBox("Instance Options",MessageType.None);
			
		random = store.isRandom;
		collidable = store.isCollidable;
		rigidbody = store.isRigidbody;
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox("Randomized Slice",MessageType.None);
		
		random = EditorGUILayout.Toggle(random);
		
		store.isRandom = random;
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		EditorGUILayout.HelpBox("Slices Collidable",MessageType.None);
		
		if (rigidbody) {
			collidable = true;	
		}
		
		collidable = EditorGUILayout.Toggle(collidable);
		
		store.isCollidable = collidable;
		
		GUILayout.EndHorizontal();
		
		if (collidable) {
			GUILayout.BeginHorizontal();
		
			EditorGUILayout.HelpBox("Slices Rigidbody",MessageType.None);
			
			rigidbody = EditorGUILayout.Toggle(rigidbody);
			
			GUILayout.EndHorizontal();	
		}
		
		store.isRigidbody = rigidbody;
		
		store.isRandom = random;
		
		if (random) {
			GUILayout.BeginVertical();
		
			EditorGUILayout.HelpBox("Slice Ammount 1 - 20",MessageType.None);
			
			sliceVal = EditorGUILayout.IntSlider(sliceVal,1,20);
			
			GUILayout.EndVertical();
			
			if (store.slicedMeshes.Count == 0) {
				if (GUILayout.Button("Slice Object")) {
					
					List<GameObject> obj = new List<GameObject>();
					// lets create our temporary GO's
					for (int i = 0; i < sliceVal; i++) {
						GameObject tmpObj = new GameObject();
						
						randomSlicers.Add(tmpObj.AddComponent<SlicerPlane>());
						
						tmpObj.transform.position = store.getRandomPosInMesh();
						tmpObj.transform.rotation = Random.rotation;
						
						obj.Add(tmpObj);
					}
					
					
					store.slice(randomSlicers, blendedMat);
					
					randomSlicers.Clear();
					
					for (int i = 0; i < obj.Count; i++) {
						DestroyImmediate(obj[i]);	
					}
					
				}	
			}
			else {
				if (GUILayout.Button("Instantiate Slices")) {
					store.instantiate();
				}
				if (GUILayout.Button("Clear Prev Slices")) {
					store.slicedMeshes.Clear();
				}	
			}
		}
		else {
			// loop and remove null slicers
			for (int i = 0; i < slicers.Count; i++) {
				if (slicers[i] == null) {
					slicers.RemoveAt(i);	
				}
			}
			
			GUILayout.BeginVertical();
		
			EditorGUILayout.HelpBox("Add a Slicer Plane to the List",MessageType.None);
			
			extraSlicer = EditorGUILayout.ObjectField(extraSlicer, typeof(SlicerPlane), true) as SlicerPlane;
			
			if (GUILayout.Button("Add Slicer Plane")) {
				if (extraSlicer != null) {
					slicers.Add(extraSlicer);
					extraSlicer = null;
				}
			}
			
			GUILayout.EndVertical();
			
			GUILayout.BeginVertical();
			
			showSlicers = EditorGUILayout.Foldout(showSlicers, " Slicer Plane List");
			
			if (showSlicers) {
				for (int i = 0; i < slicers.Count; i++) {
					slicers[i] = EditorGUILayout.ObjectField(slicers[i], typeof(SlicerPlane), true) as SlicerPlane;	
				}
				
				if (slicers.Count > 0) {
					if (GUILayout.Button("Clear All Slicers")) {
						slicers.Clear();
					}	
				}
			}
			
			if (store.slicedMeshes.Count == 0) {
				if (slicers.Count > 0) {
					if (GUILayout.Button("Slice Object")) {
						store.slice(slicers, blendedMat);
					}	
				}
				else {
					EditorGUILayout.HelpBox("There are no Slicers Available \nAdd some Slicer Planes",MessageType.Info);
					GUILayout.EndArea();
					return;
				}	
			}
			else {
				if (GUILayout.Button("Instantiate Slices")) {
					store.instantiate();
				}
				if (GUILayout.Button("Clear Prev Slices")) {
					store.slicedMeshes.Clear();
				}	
			}
		}
		
		GUILayout.EndArea();
	}
}
