using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***********************************************************************************************
 * 类 : Npc组件
 ***********************************************************************************************/
[ExecuteInEditMode]
public class Npc : MonoBehaviour
{
    public string npcName = "";
    public float radius = 1f;

    void Update()
    {
        GameObjectUnit unit = GameScene.mainScene.FindUnit(this.gameObject.name);
        
        if (Application.isEditor == true)
        {
            unit.localScale = new Vector3(radius, radius, radius);
            if (unit.Ins)
                unit.Ins.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}