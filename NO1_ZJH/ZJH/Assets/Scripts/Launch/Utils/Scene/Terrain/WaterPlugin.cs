using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class WaterPlugin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public Vector4 waveSpeed = new Vector4(-2f, -2f, 2f, 2f);
	
	void Update()
    {
        if (GameScene.mainScene == null)
            return;

        if (!GetComponent<Renderer>())
            return;

        Material mat = GetComponent<Renderer>().sharedMaterial;
        if (!mat)
            return;

        float waveScale = mat.GetFloat("_WaveScale");
        float t = 0f;
        t = GameScene.mainScene.time;

        Vector4 offset4 = waveSpeed * (t * waveScale);

        // 设置水波偏移在(0-1)之间循环
        Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x, 1.0f), Mathf.Repeat(offset4.y, 1.0f), Mathf.Repeat(offset4.z, 1.0f), Mathf.Repeat(offset4.w, 1.0f));
        mat.SetVector("_WaveOffset", offsetClamped);

        // 更新水面光照(编辑器中实时更新)
        /**
        if (GameScene.isPlaying == false)
        {
            mat.SetVector("_SunLightDir", GameScene.mainScene.terrainConfig.sunLightDir);
            mat.SetFloat("_WaterSpecStrength", GameScene.mainScene.terrainConfig.waterSpecStrength);
            mat.SetFloat("_WaterSpecRange", GameScene.mainScene.terrainConfig.waterSpecRange);
        }**/
    }
}
