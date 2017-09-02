using UnityEngine;
using System.Collections;

public class DissolveControl : MonoBehaviour
{
    public Material[] dissolveMaterial;

    public Vector3 dissolveStartPos;
    public float objectHeight;

    public float dissolveSpeed = 3;

    public float dissolveValue = 1;

    public float dissolveStr = 80;

    public bool isFlip;

	// Use this for initialization
	void Start () {
	    
	}

    void OnEnable()
    {
        if (dissolveMaterial != null && dissolveMaterial.Length != 0)
        {
            for (int i = 0; i < dissolveMaterial.Length; i++)
            {
                if (dissolveMaterial[i] != null)
                {
                    dissolveMaterial[i].EnableKeyword("_DISSOLVE");
                    dissolveMaterial[i].SetVector("_DissolveStartPos", dissolveStartPos);
                    dissolveMaterial[i].SetFloat("_ObjectHeight", objectHeight);
                    dissolveMaterial[i].SetFloat("_DissolveStr", dissolveStr);
                }
            }
        }


    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (isFlip)
	    {
	        dissolveValue -= Time.deltaTime*dissolveSpeed;
	    }
	    else
	    {
            dissolveValue += Time.deltaTime * dissolveSpeed;
        }

        Shader.SetGlobalFloat("_DissolveThreshold", dissolveValue);
    }

    void OnDisable()
    {
        if (dissolveMaterial != null && dissolveMaterial.Length != 0)
        {
            for (int i = 0; i < dissolveMaterial.Length; i++)
            {
                if (dissolveMaterial[i] != null)
                {
                    dissolveMaterial[i].DisableKeyword("_DISSOLVE");
                }
            }
        }
    }


}
