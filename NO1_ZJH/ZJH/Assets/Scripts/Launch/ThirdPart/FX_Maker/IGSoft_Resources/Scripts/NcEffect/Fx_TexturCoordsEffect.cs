using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 溶点特效自定义
/// 控制特效shader中的_Amount || _SliceAmount
/// </summary>
public class Fx_TexturCoordsEffect : MonoBehaviour, IResetAnimation
{
    public List<EffectEvent> effects;

	private Material _material;

	public string mstrMatName;

    public float fDelayTime;
    /// <summary>
    /// 需要修改的着色器属性
    /// </summary>
    public string strProperty = "_Amount";

    public bool isControlHSV;
    public int controlIndex = 0;
    private Vector4 startColorHSV;

    float fSetAmount;

    bool init = false;

    float time;

    int index;

    float value;

    float speed;

    float fStartTime;

    void Start()
    {
        if (effects == null)
            init = false;
        else
            init = true;
        time = 0;
        index = 0;
        fStartTime = 0;
        if (_material == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                if (string.IsNullOrEmpty(mstrMatName))
                {
                    _material = renderer.sharedMaterial;
                }
                else
                {
                    Material[] mats = null;
                    mats = renderer.sharedMaterials;
                    for (int i = 0; i < mats.Length; i++)
                    {
						if (mats[i].name.Contains(mstrMatName))
                        {
                            _material = mats[i];
                            break;
                        }
                    }
                }
            }
			
        }

        if (_material == null)
            init = false;
        
        if (effects.Count > 0 && init)
        {
            if (_material != null)
            {
                if (isControlHSV)
                {
                    Color startColor = _material.GetColor(strProperty);
                    startColorHSV = HSVControl.ConvertRGBToHSV(startColor);
                    fSetAmount = startColorHSV[controlIndex];
                }
                else
                {
                    fSetAmount = _material.GetFloat(strProperty);
                }
            }
                
            value = fSetAmount;
            float f = effects[0].value;
            speed = (f - value) / effects[0].time;
        }
        else
            init = false;

        if (!init)
            enabled = false;
    }

    void Update()
    {
        if (fStartTime <= fDelayTime)
        {
            fStartTime += Time.deltaTime;
            return;
        }       
        if (!init) return;
        if (index >= effects.Count)
        {
            enabled = false;
            return;
        }
        time += Time.deltaTime;
        if (effects[index] == null)
        {
            index++;
        }
        else 
        {
            value += speed * Time.deltaTime;
            if (time >= effects[index].time)
            {
                value = effects[index].value;
                index++;
                if (index < effects.Count)
                {
                    float f = effects[index].value;
                    speed = (f - value) / (effects[index].time - effects[index - 1].time);
                }
            }

            if (_material != null)
            {
                if (isControlHSV)
                {
                    startColorHSV[controlIndex] = value;
                    Color curColor = Color.HSVToRGB(startColorHSV.x, startColorHSV.y, startColorHSV.z);
                    _material.SetColor(strProperty, curColor);
                }
                else
                {
                    _material.SetFloat(strProperty, value);
                }
            }
                
        }
    }

    bool bParentIsNcDelay = false;
    /// <summary>
    /// 设置父对象是否nc
    /// </summary>
    /// <param name="bValue"></param>
    public void SetParentIsNcDelay(bool bValue)
    {
        bParentIsNcDelay = bValue;
    }

    /// <summary>
    /// 获取父对象是否是nc
    /// </summary>
    /// <returns></returns>
    public bool GetParentIsNcDelay()
    {
        return bParentIsNcDelay;
    }

    /// <summary>
    /// 重置值
    /// </summary>
    public void ResetAnimation()
    {
        if (!init)
            return;

		if (_material != null) {
			if (isControlHSV)
			{
				startColorHSV[controlIndex] = fSetAmount;
				Color curColor = Color.HSVToRGB(startColorHSV.x, startColorHSV.y, startColorHSV.z);
				_material.SetColor(strProperty, curColor);
			}
			else
			{
				_material.SetFloat(strProperty, fSetAmount);
			}
		}
			
        enabled = true;
        Start();
    }


    void OnDestroy()
    {
		if (this != null && _material != null) {
			if (isControlHSV)
			{
				startColorHSV[controlIndex] = fSetAmount;
				Color curColor = Color.HSVToRGB(startColorHSV.x, startColorHSV.y, startColorHSV.z);
				_material.SetColor(strProperty, curColor);
			}
			else
			{
				_material.SetFloat(strProperty, fSetAmount);
			}
		}
        //else
            //LogSystem.LogWarning("=======  Fx_TexturCoordsEffect _material null :" + this.transform.gameObject.name);
    }
}
