using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
/// <summary>
/// 专为角色开发的“伪光源”。并且假定游戏运行过程中光源数据不会改变。
/// </summary>
public class DirectionalLightForCharacter : MonoBehaviour {

    public Color color = Color.white;
    public float intensity = 0.8f;

    //用于cg，灯光与角色绑定
    public GameObject role;

    public void UpdateLightData()
	{
        Shader.SetGlobalVector("_WorldSpaceLightForCharacter", -transform.forward);
        Shader.SetGlobalColor("_WorldSpaceLightColorForCharacter", color * intensity);
        Shader.EnableKeyword("_SECPOINTLIGHT");
    }

    public void UpdateSingleLightData(GameObject obj)
    {
        if (obj == null)
        {
            LogSystem.LogWarning(obj.name,"is null");
            return;
        }

        Renderer[] mSkinnedMeshRender = obj.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
            return;

        Material[] mats;
        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            mats = mSkinnedMeshRender[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j] == null)
                    continue;

                mats[j].SetVector("_WorldSpaceLightForCharacter", -transform.forward);
                mats[j].SetColor("_WorldSpaceLightColorForCharacter", color * intensity);
                mats[j].EnableKeyword("_SECPOINTLIGHT");
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        if (role != null)
        {
            UpdateSingleLightData(role);
        }
        else
        {
            UpdateLightData();
        }

	}

    void OnDestroy()
    {
        role = null;
    }

#if CUSTOMLIGHT
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.forward * 2);
    }


    // Update is called once per frame
    void Update()
    {
        if (role != null)
        {
            UpdateSingleLightData(role);
        }
    }
#endif
}
