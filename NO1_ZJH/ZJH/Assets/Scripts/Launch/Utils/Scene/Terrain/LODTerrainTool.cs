using System;

/*********************************************************************************************
 * 功能 ： 地形数据的编辑
 *********************************************************************************************/
public enum LODTerrainTool
{
    None = -1,

    PaintHeight = 0,
    SetHeight = 1,
    SmoothHeight = 2,
	Slop = 8,

    PaintTexture = 3,
    CleanTexture = 4,

    PlaceTree = 5,  
    TerrainSettings = 6,
    TerrainToolCount = 7
}