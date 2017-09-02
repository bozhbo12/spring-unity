using System;

/*********************************************************************************************
 * 功能 ： 地形数据的编辑
 *********************************************************************************************/
public enum TerrainTool
{
    None = -1,
    PaintDetail = 5,
    PaintTexture = 3,
    PlaceTree = 4,

    PaintHeight = 0,
    SetHeight = 1,
    SmoothHeight = 2,

    TerrainSettings = 6,
    TerrainToolCount = 7
}