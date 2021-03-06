using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorGrid : MonoBehaviour {
	//snaps everything in unity to a grid


	public float cell_size = 1f;

	void Update() {
#if UNITY_EDITOR
		if (Application.isPlaying) return;
		float x, y, z;
		x = Mathf.Round(transform.position.x / cell_size) * cell_size;
		y = Mathf.Round(transform.position.y / cell_size) * cell_size;
		z = Mathf.Round(transform.position.z / cell_size) * cell_size;
		z = 0;
		transform.position = new Vector3(x, y, z);
#endif
	}

}