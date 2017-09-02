using UnityEngine;

/// <summary>
/// 控制分辨率的脚本
/// </summary>
public class ResolutionConstrain 
{
    public static int ScreenWidth;
    public static int ScreenHeight;

    public const float SmallRatio = 4f / 3f;
    public const double dsmallRatioPixles = 1536 * 1152;
    private static Vector2 SmallResolution = new Vector2(1536, 1152);
    public const float BigRatio = 16f / 9f;
    private static Vector2 BigResolution = new Vector2(1600, 900);
    public const double dbigRatioPixles = 1600 * 900;

    private Vector2 currentResolution = Vector2.zero;

    private ResolutionConstrain()
    {
        _instance = this;
    }

	public void init(int width, int height, bool fullScreen)
	{
		Vector2 vTemp = Vector2.zero;
		if (!fullScreen) 
		{
			vTemp.x = width;
			vTemp.y =  height;
		}
		else
		{
			Resolution resolution = Screen.currentResolution;
			vTemp.x = resolution.width;
			vTemp.y =  resolution.height;
		}	
		currentResolution = vTemp;
		Screen.SetResolution((int)currentResolution.x,(int) currentResolution.y, fullScreen);
	}


    // Use this for initialization
    public void init()
    {
        Resolution resolution = Screen.currentResolution;
        double dpixles = resolution.width * resolution.height;
        float ratio = (float)resolution.width / (float)resolution.height;

        float ratios = Mathf.Abs(ratio - SmallRatio);
        float ratiob = Mathf.Abs(ratio - BigRatio);
        if (ratios > ratiob)
        {
            ///big than dbigRatioPixles ,need change reslution
            if (dpixles > dbigRatioPixles)
            {
                /// big resolution best!
                currentResolution = BigResolution;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
                Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, true);
#else
                Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, Screen.fullScreen);
#endif
            }
            else
            {
                Vector2 vTemp = Vector2.zero;
                vTemp.x = Screen.width;
                vTemp.y =  Screen.height;
                currentResolution = vTemp;
            }
        }
        else
        {
            ///big than dbigRatioPixles ,need change reslution
            if (dpixles > dsmallRatioPixles)
            {
                /// small resolution best!
                currentResolution = SmallResolution;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
                Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, true);
#else
                Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, Screen.fullScreen);
#endif
            }
            else
            {
                Vector2 vTemp = Vector2.zero;
                vTemp.x = Screen.width;
                vTemp.y =  Screen.height;
                currentResolution = vTemp;
            }
        }

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
                Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, true);
#else
        Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, Screen.fullScreen);
#endif

		#if UNITY_ANDROID
		#if __AUTO_TEST__

		GameObject go = GameObject.Find ("GUI/UICamera");
		UICollectorConstructor ucc = go.GetComponent<UICollectorConstructor>();
		if( ucc != null )
		{
			ucc.StartInit();
		}

			#endif
			#endif



		LogSystem.LogWarning ("ResolutionConstrain init_____ :"+currentResolution.ToString());
    }
    private static ResolutionConstrain _instance;

    public static ResolutionConstrain Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ResolutionConstrain();
            return _instance;
        }
    }

    public Vector2 CurrentResolution { get { return currentResolution; } }

    public int width { get { return (int)currentResolution.x; } }
    public int height { get { return (int)currentResolution.y; } }
}

