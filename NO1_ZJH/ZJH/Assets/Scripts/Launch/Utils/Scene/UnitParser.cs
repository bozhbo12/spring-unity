using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/***************************************************************************************
 * 功能 : 单位解析
 ****************************************************************************************/
public class UnitParser
{
    public GameObjectUnit unit = null;

    virtual public void Read(BinaryReader br)
    {
        
    }

    virtual public void Write(BinaryWriter bw)
    { 
        
    }

    virtual public void Update(GameObject ins)
    {

    }

    virtual public void Destroy()
    {
        unit = null;
    }


    /******************************************************************************
     * 功能 : 单位是否支持特殊存储, 部分策划配置对象,如NPC需要XML存储
     *******************************************************************************/
    virtual public bool SpecialStorage()
    {
        return false;
    }

    virtual public UnitParser Clone()
    {
        return new UnitParser();
    }

    virtual public UnityEngine.Object Instantiate(UnityEngine.Object prefab)
    {
		if(GameScene.isPlaying)
		{
            return GameObject.Instantiate(prefab) as GameObject;
		}else
		{
			#if UNITY_EDITOR
			return PrefabUtility.InstantiatePrefab(prefab);
			#endif
		}
        return null;
    }
}