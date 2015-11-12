using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DecalController : MonoBehaviour {
	
	public Material defaultMaterial;
	
	[HideInInspector]
	public List<ProjectedStaticDecal> psdList = new List<ProjectedStaticDecal>();
	
	[HideInInspector]
	public List<ProjectedStaticDecal> pddList = new List<ProjectedStaticDecal>();
	
	[HideInInspector]
	public List<DynamicDecal> ddList = new List<DynamicDecal>();
	
	[HideInInspector]
	public MeshBatcher psdBatcher = new MeshBatcher();
	
	[HideInInspector]
	public MeshBatcher pddBatcher = new MeshBatcher();
	
	public void createDynamicDecal(string dname, SceneData sd) {
		GameObject obj = new GameObject();
		
		if (dname == "") {
			obj.name = "Default Dynamic Decal";
		}
		else {
			obj.name = dname;
			dname = "";
		}
		
		obj.tag = "DynamicDecal";
		obj.AddComponent(typeof(DynamicDecal));
		obj.transform.parent = sd.transform;
		
		DynamicDecal decal = obj.GetComponent<DynamicDecal>();
		
		decal.create(sd);
		ddList.Add(decal);
	}
	
	public void createProjectedStaticDecal(string dname, SceneData sd) {
		GameObject obj = new GameObject();
		if (dname == "") {
			obj.name = "Default Projected Decal";
		}
		else {
			obj.name = dname;
			dname = "";
		}
		obj.tag = "ProjectedStaticDecal";
		obj.AddComponent(typeof(ProjectedStaticDecal));
		obj.transform.parent = sd.transform;
		
		ProjectedStaticDecal decal = obj.GetComponent<ProjectedStaticDecal>();
		decal.create(sd);
		psdList.Add(decal);
		
		// add to batch
		psdBatcher.addDecal(decal);
	}
	
	public void createProjectedDynamicDecal(Vector3 position, Quaternion rotation, Vector3 scale, Material mat, int layer, bool isCubeMap) {
		SceneData sd = gameObject.GetComponent<SceneData>();
		
		if (sd == null) {
			Debug.Log("Error in function createProjectedDynamicDecal(), SceneData not found. No action taken");
			return;
		}
		
		GameObject obj = new GameObject();
		
		obj.name = "ProjectedDynamicDecal";
		obj.tag = "ProjectedStaticDecal";
		ProjectedStaticDecal decal = obj.AddComponent<ProjectedStaticDecal>();
		obj.transform.position = position;
		obj.transform.rotation = rotation;
		obj.transform.localScale = scale;
		
		decal.material = mat;
		decal.layer = layer;
		decal.cubeMap = isCubeMap;
		
		decal.create(sd);
		pddList.Add(decal);
		
		pddBatcher.addDecal(decal);
		
		decal.updateMesh();
		
		pddBatcher.updateBatch();
	}
	
	public List<ProjectedStaticDecal> getProjectedStaticDecalList() {
		return psdList;	
	}
	
	public List<DynamicDecal> getDynamicDecalList() {
		return ddList;	
	}
	
	public void updateDynamicDecals() {
		foreach (DynamicDecal d in ddList) {
			d.updateMesh();	
		}
	}
	
	public void updateProjectedStaticDecals() {
		foreach (ProjectedStaticDecal d in psdList) {
			d.updateMesh();	
		}
	}
	
	public void removeDynamicDecals() {
		foreach (DynamicDecal d in ddList) {
			if (d != null) {
				d.destroy();	
			}
		}
		
		ddList.Clear();
	}
	
	public void removeProjectedStaticDecals() {
		foreach (ProjectedStaticDecal d in psdList) {
			if (d != null) {
				d.destroy();	
			}
		}
		
		psdList.Clear();
		psdBatcher.clearDecals();
	}
	
	void OnDrawGizmos() {
		// in editor rendering
		psdBatcher.updateBatch();
	}

	// Use this for initialization
	void Start () {
		foreach (ProjectedStaticDecal d in pddList) {
			if (d != null) {
				d.destroy();	
			}
		}
		
		pddList.Clear();
		pddBatcher.clearDecals();
		
		// only need to call once for static decals
		pddBatcher.updateBatch();
		psdBatcher.updateBatch();
	}
	
	void Update() {
		psdBatcher.renderBatch();
		pddBatcher.renderBatch();
	}
}
