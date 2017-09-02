using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景lightmap设置
/// </summary>
public class SceneLightMap:CacheObject
{

    public static SceneLightMap GetSceneLightMap()
    {
        return CacheObjects.SpawnCache<SceneLightMap>();
    }

    private LightMapInfo[] listLightMap = null;

    /// <summary>
    /// 当前显示lightmap
    /// </summary>
    private LightMapInfo mCurLightMapInfo = null;

    /// <summary>
    /// 设置lightmap信息
    /// "Scenes/" + sceneID + "/lightmap/LightmapFar-" + i;;
    /// </summary>
    /// <param name="sceneID"></param>
    /// <param name="iCategory"></param>
    /// <param name="iLightmapCount"></param>
    public void SetLightmapInfo(string sceneID, int iCategory, int iLightmapCount)
    {
        listLightMap = new LightMapInfo[iCategory];
        LightMapInfo lmi = null;
        string strPathPrefix;
        for (int i = 0; i < iCategory; i++)
        {
            lmi = LightMapInfo.GetLightMapInfo();
            strPathPrefix = DelegateProxy.StringBuilder("Scenes/", sceneID, "/");
            lmi.SetInfo(i, iLightmapCount, strPathPrefix);
            listLightMap[i] = lmi;
        }
    }

    /// <summary>
    /// 设置lightmap
    /// </summary>
    /// <param name="index"></param>
    public void DisplayLightmap(int index)
    {
        if (index < 0 || index >= listLightMap.Length)
        {
            LogSystem.LogWarning("SceneLightMap::StartLoad list out range", index);
            return;
        }

        if (mCurLightMapInfo == listLightMap[index])
            return;

        if (mCurLightMapInfo != null)
            mCurLightMapInfo.ClearLightmap();

        mCurLightMapInfo = listLightMap[index];

        mCurLightMapInfo.StartLoad(OnLoadComplete);
    }

    /// <summary>
    /// lightmap图加载完成
    /// </summary>
    /// <param name="lightMapInfo"></param>
    private void OnLoadComplete(LightMapInfo lightMapInfo)
    {
        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        LightmapSettings.lightmaps = lightMapInfo.lightmapData;
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public void Dispose()
    {
        mCurLightMapInfo = null;

        if (listLightMap == null)
            return;

        for (int i = 0; i < listLightMap.Length; i++)
        {
            if (listLightMap[i] == null)
                continue;

            listLightMap[i].Dispose();
            listLightMap[i] = null;
        }
        listLightMap = null;

        CacheObjects.DespawnCache(this);
    }

    /// <summary>
    /// 加载lightmap
    /// </summary>
    public class LightMapInfo:CacheObject
    {
        public static LightMapInfo GetLightMapInfo()
        {
            return CacheObjects.SpawnCache<LightMapInfo>();
        }

        /// <summary>
        /// 路径 "Scenes/" + sceneID + "/lightmap(X)/LightmapFar-" + i;;
        /// </summary>
        private string[] mstrPath;

        /// <summary>
        /// 图片数据
        /// </summary>
        private LightmapData[] lmds;

        public LightmapData[] lightmapData
        {
            get
            {
                return lmds;
            }
        }

        /// <summary>
        /// 是否被删除
        /// </summary>
        private bool bDestroy = false;

        /// <summary>
        /// 加载计数
        /// </summary>
        private int iLoadedCount = 0;

        /// <summary>
        /// 加载完成回调
        /// </summary>
        private System.Action<LightMapInfo> mOnLoadComplete = null;
        /// <summary>
        /// 设置lightmap信息
        /// </summary>
        /// <param name="iCategory">种类一个场景有几种lightmap</param>
        /// <param name="iCount">几张lightmap图</param>
        /// <param name="strPrefix">前缀</param>
        public void SetInfo(int iCategory, int iCount, string strPrefix)
        {
            mstrPath = new string[iCount];
            string strFileName = "Lightmap";
            for (int i = 0; i < iCount; i++)
            {
                string strTemp = strFileName;
                if(iCategory != 0)
                    strTemp = DelegateProxy.StringBuilder(strFileName, iCategory);

                mstrPath[i] = DelegateProxy.StringBuilder(strPrefix, strTemp, "/LightmapFar-", i);
            }
        }

        /// <summary>
        /// 发起加载
        /// </summary>
        /// <param name="OnLoadComplete"></param>
        public void StartLoad(System.Action<LightMapInfo> OnLoadComplete)
        {
            bDestroy = false;

            if (mstrPath == null)
            {
                LogSystem.LogWarning("LightMapInfo Please call SetInfo first!!");
                return;
            }

            mOnLoadComplete = OnLoadComplete;
            if (lmds != null)
            {
                if (mOnLoadComplete != null)
                    mOnLoadComplete(this);
                return;
            }

            iLoadedCount = 0;
            lmds = new LightmapData[mstrPath.Length];
            for(int i = 0; i<mstrPath.Length; i++)
            {
                VarStore varStore = VarStore.CreateVarStore();
                varStore += i;
                DelegateProxy.LoadAsset(mstrPath[i], OnLoadLightCallback, varStore);
            }
        }

        /// <summary>
        /// 单图加载完成
        /// </summary>
        /// <param name="oAsset"></param>
        /// <param name="strPath"></param>
        /// <param name="varStore"></param>
        private void OnLoadLightCallback(UnityEngine.Object oAsset, string strPath, VarStore varStore)
        {
            int index = 0;
            if (varStore != null)
            {
                index = varStore.GetInt(0);
                varStore.Destroy();
            }
            if (oAsset == null)
            {
                LogSystem.LogWarning("SceneLightMap oAsset is null", strPath);
                return;
            }

            if (bDestroy)
            {
                CacheObjects.PopCache(oAsset);
                return;
            }
            iLoadedCount++;
            lmds[index] = new LightmapData();
            lmds[index].lightmapColor = oAsset as Texture2D;
            if (iLoadedCount == mstrPath.Length)
            {
                if (mOnLoadComplete != null)
                {
                    mOnLoadComplete(this);
                    mOnLoadComplete = null;
                }
            }
        }

        /// <summary>
        /// 清除lightmap
        /// </summary>
        public void ClearLightmap()
        {
            bDestroy = true;

            mOnLoadComplete = null;

            if(lmds == null)
                return;

            for (int i = 0; i < lmds.Length; i++)
            {
                if (lmds[i] == null)
                    continue;

                CacheObjects.PopCache(lmds[i].lightmapColor);
                lmds[i].lightmapColor = null;
                lmds[i] = null;
            }
            lmds = null;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Dispose()
        {
            bDestroy = true;

            ClearLightmap();

            mstrPath = null;

            mOnLoadComplete = null;

            CacheObjects.DespawnCache(this);
        }
    }
}
