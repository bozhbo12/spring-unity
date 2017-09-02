using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************************************************
 * 功能 : 载入资源的方式
 ******************************************************************************************************/
public enum LoadType
{
    Type_AssetDatabase = 0,
    Type_Resources = 1,
    Type_AssetBundle = 2,
    Type_Auto = 3,                              // 自适应载入,判断资源库与资源包中是否有该资源(性能比较消耗的方式)
    Type_AppData = 4,                           // 用户文件流操作
    Type_WWW = 5,
    Type_ThirdPart = 6                          // 第三方资源加载
}
