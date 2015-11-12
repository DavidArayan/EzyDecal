using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DecalEditor : EditorWindow {
	Vector2 DDscrollPos = Vector2.zero;
	Vector2 PDSscrollPos = Vector2.zero;
	string dname = "";
	
	bool showSceneOptions = true;
	bool showDecalOptions = true;
	bool showDynamicDecalsOption = true;
	bool showProjectedDecalsOption = true;
	
	[MenuItem("Window/DecalEditor")]
	static void ShowWindow() {
		EditorWindow window = EditorWindow.GetWindow(typeof(DecalEditor));
						
		window.autoRepaintOnSceneChange = true;
		window.position = new Rect(400,400,300,600);
		window.maxSize = new Vector2(300,600);
		window.minSize = new Vector2(300,600);
		
		GameObject eObj = GameObject.FindGameObjectWithTag("DecalEditor");
		
		if (eObj == null) {
			GameObject emptyObject = new GameObject();
			emptyObject.AddComponent(typeof(SceneData));
			emptyObject.AddComponent(typeof(DecalController));
			
			emptyObject.tag = "DecalEditor";
			emptyObject.name = "DecalEditor";
		}
	}
	
	static void Init() {
		EditorWindow window = EditorWindow.GetWindow(typeof(DecalEditor));
				
		window.autoRepaintOnSceneChange = true;
		window.position = new Rect(400,400,300,600);
		window.maxSize = new Vector2(300,600);
		window.minSize = new Vector2(300,600);
		
		GameObject eObj = GameObject.FindGameObjectWithTag("DecalEditor");
		
		if (eObj == null) {
			GameObject emptyObject = new GameObject();
			emptyObject.AddComponent(typeof(SceneData));
			emptyObject.AddComponent(typeof(DecalController));
			
			emptyObject.tag = "DecalEditor";
			emptyObject.name = "DecalEditor";
		}
	}
	
	void OnGUI() {
		SceneData sd = GameObject.FindGameObjectWithTag("DecalEditor").GetComponent<SceneData>();
		DecalController dc = GameObject.FindGameObjectWithTag("DecalEditor").GetComponent<DecalController>();
		
		if (sd == null) {
			return;	
		}
		GUILayout.BeginArea( new Rect(0, 0, 300, 600));
		EditorGUILayout.HelpBox("Decal Framework and Editor \nCreated by David Arayan 2013",MessageType.None);
		//GUILayout.Box("Decal Framework and Editor \n Created by David Arayan 2013");
		
		EditorGUILayout.HelpBox("Scene Control Functions",MessageType.None);
		//GUILayout.Label ("Scene Control Functions", EditorStyles.boldLabel);
		showSceneOptions = EditorGUILayout.Foldout(showSceneOptions, " Scene Options");
		
		if (showSceneOptions) {
			GUILayout.Label ("Scene Optimal Data: [" + sd.vertexCount() + "]");
			GUILayout.Label ("Scene Raw Data: [" + sd.vertexRawCount() + "]");
			
			if (GUILayout.Button("Generate Quad Scene Data")) {
				sd.clear();
				sd.generateData();
			}
		
			if (sd.vertexCount() > 0) {
				if (sd.isGizmosEnabled()) {
					if (GUILayout.Button("Disable Render Gizmos")) {
						sd.disableGizmos();
					}
				}
				else {
					if (GUILayout.Button("Enable Render Gizmos")) {
						sd.enableGizmos();
					}
				}
				
				if (sd.isGridGizmosEnabled()) {
					if (GUILayout.Button("Disable Render Broadphase Grid")) {
						sd.disableGridGizmos();
					}
				}
				else {
					if (GUILayout.Button("Enable Render Broadphase Grid")) {
						sd.enableGridGizmos();
					}
				}
				
				if (GUILayout.Button("Update Data")) {
					sd.update();
				}
				
				if (GUILayout.Button("Clear Data")) {
					sd.clear();
				}
			}	
		}
		
		EditorGUILayout.HelpBox("Decal Control Functions",MessageType.None);
		//GUILayout.Label ("Decal Control Functions", EditorStyles.boldLabel);
		GUILayout.BeginVertical();
		
		showDecalOptions = EditorGUILayout.Foldout(showDecalOptions, " Decal Options");
		
		List<ProjectedStaticDecal> projDecalList = dc.getProjectedStaticDecalList();
		List<DynamicDecal> decalList = dc.getDynamicDecalList();
		
		if (showDecalOptions) {
			dname = GUILayout.TextField(dname,25);
			
			if (GUILayout.Button("Create Dynamic Decal")) {
				dc.createDynamicDecal(dname, sd);
			}
			
			if (GUILayout.Button("Create Projected Decal")) {
				dc.createProjectedStaticDecal(dname, sd);
			}
			
			if (decalList.Count > 0) {
				if (GUILayout.Button("Update All Dynamic Decals")) {
					dc.updateDynamicDecals();
				}
				
				if (GUILayout.Button("Remove All Dynamic Decals")) {
					dc.removeDynamicDecals();
				}
			}
			
			if (projDecalList.Count > 0) {
				if (GUILayout.Button("Update All Projected Decals")) {
					dc.updateProjectedStaticDecals();
				}
				
				if (GUILayout.Button("Remove All Projected Decals")) {
					dc.removeProjectedStaticDecals();
				}
			}
			GUILayout.EndVertical();	
		}
		
		EditorGUILayout.HelpBox("Full Decal List",MessageType.None);
		
		//GUILayout.Label ("Full Decal List", EditorStyles.boldLabel);
		
		showDynamicDecalsOption = EditorGUILayout.Foldout(showDynamicDecalsOption, " Dynamic Decal List");
		
		if (showDynamicDecalsOption && decalList.Count > 0) {
			
			DDscrollPos = GUILayout.BeginScrollView(DDscrollPos);
			
			for (int i = 0; i < decalList.Count; i++) {				
				if (decalList[i] == null) {
					continue;
				}
				
				GUILayout.BeginVertical();
				
				decalList[i].collInEditor = EditorGUILayout.Foldout(decalList[i].collInEditor, " " + decalList[i].name);
				
				if (decalList[i].collInEditor) {
					//GUILayout.Label(decalList[i].name);
				
					GUILayout.BeginHorizontal();
					
					if (GUILayout.Button("Select")) {
						decalList[i].selectObj();	
					}
					
					if (GUILayout.Button("Update")) {
						decalList[i].updateMesh();
					}
					
					if (GUILayout.Button("Reset")) {
						decalList[i].clear(sd);
					}
					
					if (GUILayout.Button("Remove")) {
						decalList[i].destroy();
						decalList.RemoveAt(i);
						continue;
					}
					GUILayout.EndHorizontal();
					
					if (GUILayout.Button("Subdivide Upwards")) {
						decalList[i].subdivideUp();
					}
					
					if (GUILayout.Button("Subdivide Downwards")) {
						decalList[i].subdivideDown();
					}
					
					if (decalList[i].isRtUpdateEnabled()) {
						if (GUILayout.Button("Disable Realtime Update")) {
							decalList[i].setRtUpdateEnabled(false);
						}
					}
					else {
						if (GUILayout.Button("Enable Realtime Update")) {
							decalList[i].setRtUpdateEnabled(true);
						}
					}
					
					if (decalList[i].isUpdateEnabled()) {
						if (GUILayout.Button("Disable Update")) {
							decalList[i].setUpdateEnabled(false);
						}
					}
					else {
						if (GUILayout.Button("Enable Update")) {
							decalList[i].setUpdateEnabled(true);
						}
					}	
				}
				
				GUILayout.EndVertical();
			}
			
			GUILayout.EndScrollView();
		}
		
		showProjectedDecalsOption = EditorGUILayout.Foldout(showProjectedDecalsOption, " Projected Decal List");
		
		if (showProjectedDecalsOption && projDecalList.Count > 0) {
			
			PDSscrollPos = GUILayout.BeginScrollView(PDSscrollPos);
			
			for (int i = 0; i < projDecalList.Count; i++) {
				if (projDecalList[i] == null) {
					continue;
				}
				
				GUILayout.BeginVertical();
				
				projDecalList[i].collInEditor = EditorGUILayout.Foldout(projDecalList[i].collInEditor, " " + projDecalList[i].name);
				
				//GUILayout.Label(projDecalList[i].name);
				
				if (projDecalList[i].collInEditor) {
					GUILayout.BeginHorizontal();
				
					if (GUILayout.Button("Select")) {
						projDecalList[i].selectObj();	
					}
					
					if (GUILayout.Button("Update")) {
						projDecalList[i].updateMesh();
					}
					
					if (GUILayout.Button("Reset")) {
						projDecalList[i].clear(sd);
					}
					
					if (GUILayout.Button("Remove")) {
						projDecalList[i].destroy();
						projDecalList.RemoveAt(i);
						continue;
					}
					GUILayout.EndHorizontal();
					
					if (projDecalList[i].isRtUpdateEnabled()) {
						if (GUILayout.Button("Disable Realtime Update")) {
							projDecalList[i].setRtUpdateEnabled(false);
						}
					}
					else {
						if (GUILayout.Button("Enable Realtime Update")) {
							projDecalList[i].setRtUpdateEnabled(true);
						}
					}
					
					if (projDecalList[i].isUpdateEnabled()) {
						if (GUILayout.Button("Disable Update")) {
							projDecalList[i].setUpdateEnabled(false);
						}
					}
					else {
						if (GUILayout.Button("Enable Update")) {
							projDecalList[i].setUpdateEnabled(true);
						}
					}
				}
				
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}
		
		GUILayout.EndArea();
		
		SceneView.RepaintAll();
	}
}
