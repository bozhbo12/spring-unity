using UnityEngine;
using System.Collections;

/// <summary>
/// 保存环境光到预制文件
/// </summary>
[ExecuteInEditMode]
public class AmbientLight : MonoBehaviour
{
    public int ambientMode;
    public Color ambientLight;
    public Color ambientSkyColor;
    public Color ambientEquatorColor;
    public Color ambientGroundColor;
    public float ambientIntensity;

    private static float fAIRate = -1;
	// Use this for initialization
	void Start () {
        RenderSettings.ambientMode = (UnityEngine.Rendering.AmbientMode)ambientMode;
        RenderSettings.ambientLight = ambientLight;
        RenderSettings.ambientSkyColor = ambientSkyColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientGroundColor = ambientGroundColor;

        RenderSettings.ambientIntensity = ambientIntensity * AmbientIntenityRate;
	}

    /// <summary>
    /// 环境光比率 根据机型设置环境光比率为修复oppor7s爆光过度问题
    /// </summary>
    private float AmbientIntenityRate
    {
        get
        {
            if (fAIRate == -1)
            {
                fAIRate = 1f;
                string strTmp = Config.GetUpdaterConfig("AmbientIntensityRate", "Phone");
                if (string.IsNullOrEmpty(strTmp))
                    return fAIRate;

                string[] strTmps = strTmp.Split(';');
                string strPhone = Config.strPhoneType;
                strPhone = strPhone.Trim();
                strPhone = strPhone.Replace(" ", "");
                strPhone = strPhone.Replace("_", "");
                strPhone = strPhone.ToUpper();
                for (int i = 0; i < strTmps.Length; i++)
                {
                    strTmp = strTmps[i];
                    if (string.IsNullOrEmpty(strTmp))
                        continue;

                    if (strPhone.Contains(strTmp))
                    {
                        string sTmp = Config.GetUpdaterConfig("AmbientIntensityRate", "Value");
                        float fValue = 1f;
                        if(float.TryParse(sTmp, out fValue))
                        {
                            fAIRate = fValue;
                        }
                        break;
                    }
                }
            }
            return fAIRate;
        }
    }

#if UNITY_EDITOR
    // Update is called once per frame
	void Update ()
    {
        ambientMode = (int)RenderSettings.ambientMode;
        ambientLight = RenderSettings.ambientLight;
        ambientSkyColor = RenderSettings.ambientSkyColor;
        ambientEquatorColor = RenderSettings.ambientEquatorColor;
        ambientGroundColor = RenderSettings.ambientGroundColor;
        ambientIntensity = RenderSettings.ambientIntensity;
    }
#endif

}
