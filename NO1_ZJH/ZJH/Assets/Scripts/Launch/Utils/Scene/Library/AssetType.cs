using System;
using System.Collections.Generic;
using System.Text;

/*******************************************************************************
 * 功能 ： 资源类型
 *******************************************************************************/
public enum  AssetType
{
    Mesh                   = 1,             // 网格数据
    Material                = 2,            // 材质资源
    Texture2D               = 3,            // 2D纹理资源
    Cubemap                 = 4,            // 立方体纹理
    Prefab                  = 5,            // 预定义资源， 用来实例化游戏对象的重要资源
    Terrain                 = 6,            // 地形数据资源
    Region                  = 7,            // 地域的基础数据
    GameScene               = 8,            // 游戏场景资源(包含场景和地形的相关配置)
    AssetBundle             = 9,            // 资源包资源
    GameObject              = 10,           // 游戏对象
    Model = 11                              // 模型

}