using UnityEngine;

public class DontCullWithFrustum : MonoBehaviour
{
    MeshFilter mf = null;

    // Use this for initialization
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 100000;
        vTemp.y =  100000;
        vTemp.z =  100000;
        mf.mesh.bounds = new Bounds(Vector3.zero, vTemp);
    }

}
