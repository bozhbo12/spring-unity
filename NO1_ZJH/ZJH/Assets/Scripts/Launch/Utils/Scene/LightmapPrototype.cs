using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**************************************************************************
 * 功能 ： 渲染器的光照贴图属性
 **************************************************************************/
public class LightmapPrototype
{
    /** 渲染器在父级中的索引 */
    public int rendererChildIndex = -1;

    /** 在光照贴图中的缩放, 用于烘焙设置 */
    public float scale = 1f;

    /** 光照贴图索引 */
    public int lightmapIndex = -1;

    /** 光照贴图的缩放和偏移 */
    public Vector4 lightmapTilingOffset;

    /** 光照贴图尺寸 */
    public int size = 16;   
}