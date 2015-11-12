using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MeshBatcher {
	
	[HideInInspector]
	public Dictionary<Material, BatchedGeometry> batch;
	
	[HideInInspector]
	public List<ProjectedStaticDecal> decals;
	
	[HideInInspector]
	public MeshBatchFiller filler;
	
	public MeshBatcher() {
		batch = new Dictionary<Material, BatchedGeometry>();
		decals = new List<ProjectedStaticDecal>();
		filler = new MeshBatchFiller();
	}
	
	public void addDecal(ProjectedStaticDecal decal) {
		decals.Add(decal);	
	}
	
	public void clearDecals() {
		decals.Clear();
		
		foreach (Material key in batch.Keys) {
			BatchedGeometry geometry = batch[key];
			
			geometry.finalMesh.Clear();
		}
	}
	
	public void updateBatch() {	
		// update the decals and respective geometry
		for (int j = 0; j < decals.Count; j++) {
			ProjectedStaticDecal decal = decals[j];
			
			if (decal == null || decal.gameObject == null) {
				decals.RemoveAt(j);
				continue;
			}
			
			if (decal.isRtUpdateEnabled()) {
				decal.updateMesh();	
			}
			
			filler.reset();
			
			decal.fillBatchData(filler);
			
			// we have our batched data, add it into overall batch
			if (filler.material != null) {
				// we already have this material, add to batch
				if (batch.ContainsKey(filler.material)) {
					BatchedGeometry m = batch[filler.material];
					
					int offset = m.vertices.Count;
					
					m.vertices.AddRange(filler.vertices);
					
					// offset out indices to new vertex positions
					for (int i = 0; i < filler.indices.Count; i++) {
						m.indices.Add(filler.indices[i] + offset);	
					}
					
					m.uv.AddRange(filler.uv);
				}
				else {
					// add a new key because key doesnt exist, and add to batch
					batch.Add(filler.material, new BatchedGeometry());
					
					BatchedGeometry m = batch[filler.material];
					
					int offset = m.vertices.Count;
					
					m.vertices.AddRange(filler.vertices);
					
					// offset our indices to new vertex positions
					for (int i = 0; i < filler.indices.Count; i++) {
						m.indices.Add(filler.indices[i] + offset);	
					}
					
					m.uv.AddRange(filler.uv);
				}
			}
		}
		
		// batch all the updated decals and clear previous buffers
		foreach (BatchedGeometry b in batch.Values) {
			b.batch();	
		}
	}
	
	public void renderBatch() {
		foreach (Material key in batch.Keys) {
			BatchedGeometry geometry = batch[key];
			
			Graphics.DrawMesh(geometry.finalMesh, Vector3.zero, Quaternion.identity, key, 0);
		}
	}
	
	public void renderBatchNow() {
		foreach (Material key in batch.Keys) {
			BatchedGeometry geometry = batch[key];
			
			key.SetPass(1);
			Graphics.DrawMeshNow(geometry.finalMesh,Vector3.zero,Quaternion.identity);
		}
	}
}
