using UnityEngine;
using System.Collections;

public class EffectRainsplashBox : MonoBehaviour
{
	private MeshFilter mf;	
	private Bounds bounds; 

	private EffectRainsplashManager manager;

	void Start () {
		transform.localRotation = Quaternion.identity;

		manager = transform.parent.GetComponent<EffectRainsplashManager> (); 
		bounds = new Bounds (new Vector3 (transform.position.x,0.0f,transform.position.z),
			new Vector3 (manager.areaSize,Mathf.Max(manager.areaSize,manager.areaHeight),manager.areaSize));	

		mf = GetComponent<MeshFilter> ();		
		mf.sharedMesh = manager.GetPreGennedMesh ();		

		enabled = false;
	}

	void OnBecameVisible () {
		enabled = true;
	}

	void OnBecameInvisible () {
		enabled = false;
	}

	void OnDrawGizmos () {
		if (transform.parent) {
			manager = transform.parent.GetComponent<EffectRainsplashManager> (); 
			Gizmos.color = new Color(0.5f,0.5f,0.65f,0.5f);
			if(manager)
				Gizmos.DrawWireCube (	transform.position + transform.up * manager.areaHeight * 0.5f, 
					new Vector3 (manager.areaSize,manager.areaHeight, manager.areaSize) );
		}
	}
}
