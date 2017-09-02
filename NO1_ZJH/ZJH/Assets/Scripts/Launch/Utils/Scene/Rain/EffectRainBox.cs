using UnityEngine;
using System.Collections;

public class EffectRainBox : MonoBehaviour
{

	private MeshFilter mf;
	private Vector3 defaultPosition;
	private Bounds bounds;

	private EffectRainManager manager;

	private Transform cachedTransform;
	private float cachedMinY;
	private float cachedAreaHeight;
	private float cachedFallingSpeed;

	void Start() {
		manager = transform.parent.GetComponent<EffectRainManager> ();

		Vector3 b1 = new Vector3 (transform.position.x, manager.minYPosition, transform.position.z);
		Vector3 b2 = new Vector3 (manager.areaSize * 1.35f, Mathf.Max (manager.areaSize, manager.areaHeight) * 1.35f, manager.areaSize * 1.35f);
		bounds = new Bounds (b1, b2);	

		mf = GetComponent<MeshFilter> ();		
		mf.sharedMesh = manager.GetPreGennedMesh ();

		cachedTransform = transform;
		cachedMinY = manager.minYPosition;
		cachedAreaHeight = manager.areaHeight;
		cachedFallingSpeed = manager.fallingSpeed;

		enabled = false;
	}

	void OnBecameVisible () {
		enabled = true;
	}

	void OnBecameInvisible () {
		enabled = false;
	}

	void Update() {		
		cachedTransform.position -= Vector3.up * Time.deltaTime * cachedFallingSpeed;

		if(cachedTransform.position.y + cachedAreaHeight < cachedMinY) {
			cachedTransform.position = cachedTransform.position + Vector3.up * cachedAreaHeight * 2.0f;
		}
	}

	void OnDrawGizmos () {
		#if UNITY_EDITOR
		// do not display a weird mesh in edit mode
		if (!Application.isPlaying) {
		mf = GetComponent<MeshFilter> ();		
		mf.sharedMesh = null;	
		}
		#endif

		if (transform.parent) {
			Gizmos.color = new Color(0.2f,0.3f,3.0f,0.35f);
			EffectRainManager manager = transform.parent.GetComponent<EffectRainManager>() as EffectRainManager; 
			if (manager) {
				Vector3 v1 = (transform.position + transform.up * manager.areaHeight * 0.5f);
				Vector3 v2 = new Vector3 (manager.areaSize, manager.areaHeight, manager.areaSize);
				Gizmos.DrawWireCube (v1, v2);
			}
		}
	}
}

