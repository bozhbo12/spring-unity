using UnityEngine;
using System;
using System.Collections;

public class TerrainIndexGenerator 
{
    static private int kPatchSize = 17;

    const int kDirectionLeft = 0, kDirectionRight = 1, kDirectionUp = 2, kDirectionDown = 3,
        kDirectionLeftUp = 0, kDirectionRightUp = 1, kDirectionLeftDown = 2, kDirectionRightDown = 3;

    /** 1111 周边LOD级别相等 */
    const int kDirectionLeftFlag = (1 << kDirectionLeft),
        kDirectionRightFlag = (1 << kDirectionRight),
        kDirectionUpFlag = (1 << kDirectionUp),
        kDirectionDownFlag = (1 << kDirectionDown),
        kDirectionDirectNeighbourMask = (kDirectionLeftFlag | kDirectionRightFlag | kDirectionUpFlag | kDirectionDownFlag);

    struct CachedStrip 
    {
	    public uint count;
	    public int[] triangles;
    };

    static CachedStrip[] gCachedStrips = new CachedStrip[16];

    /***************************************************************************************
     * 功能：从缓存中获取LOD三角形索引
     ***************************************************************************************/

    static public int[] GetOptimizedIndexStrip (int edgeMask, uint count)
    {
        /**
	    edgeMask &= kDirectionDirectNeighbourMask;
	    if (gCachedStrips[edgeMask].triangles == null)
	    {
		    int[] triangles = GetTriangles (edgeMask, count, 0);
		
		    gCachedStrips[edgeMask].count = count;
            gCachedStrips[edgeMask].triangles = triangles;
	    }
	
	    count = gCachedStrips[edgeMask].count;
	    return gCachedStrips[edgeMask].triangles;
         **/
        return null;
    }

    /***************************************************************************************
     * 功能：获取LOD三角形索引
     ***************************************************************************************/

    static public int[] GetTriangles(int edgeMask, int patchSize,  uint count, int stride)
    {
        kPatchSize = patchSize;

	    int[] triangles = new int[(kPatchSize) * (kPatchSize) * 6];
	    uint index = 0;

        // LOD三角形跨度
	    int size = kPatchSize;
	
	    int minX = 0;
	    int minY = 0;
	    int maxX = kPatchSize-1;
	    int maxY = kPatchSize-1;
	    
	    if((edgeMask & kDirectionLeftFlag) == 0)
	    {
		    minX+=1;
		    index = AddSliverTriangles (triangles, index, kDirectionLeft, edgeMask);
	    }
	    if((edgeMask & kDirectionRightFlag) == 0)
	    {
		    maxX-=1;
		    index = AddSliverTriangles (triangles, index, kDirectionRight, edgeMask);
	    }
	    if((edgeMask & kDirectionUpFlag) == 0)
	    {
		    maxY-=1;
		    index = AddSliverTriangles (triangles, index, kDirectionUp, edgeMask);
	    }
	    if((edgeMask & kDirectionDownFlag) == 0)
	    {
		    minY+=1;
		    index = AddSliverTriangles (triangles, index, kDirectionDown, edgeMask);
	    }

        // 左上角衔接修复
	    if((edgeMask & kDirectionLeftFlag) == 0 || (edgeMask & kDirectionUpFlag) == 0)
		    index = AddSliverCorner (triangles, index, kDirectionLeftUp, edgeMask);
        // 右上角衔接修复
	    if((edgeMask & kDirectionRightFlag) == 0 || (edgeMask & kDirectionUpFlag) == 0)
		    index = AddSliverCorner (triangles, index, kDirectionRightUp, edgeMask);
        // 左下角衔接修复
	    if((edgeMask & kDirectionLeftFlag) == 0 || (edgeMask & kDirectionDownFlag) == 0)
		    index = AddSliverCorner (triangles, index, kDirectionLeftDown, edgeMask);
        // 右下角衔接修复
	    if((edgeMask & kDirectionRightFlag) == 0 || (edgeMask & kDirectionDownFlag) == 0)
		    index = AddSliverCorner (triangles, index, kDirectionRightDown, edgeMask);
	
	    for (int y=minY;y<maxY;y++)
	    {
		    for (int x=minX;x<maxX;x++)
		    {
			    // 每个格子单元两个三角面
                
			    triangles[index++] = (y + 0) + (x + 0) * size;
                triangles[index++] = (y + 1) + (x + 0) * size;
                triangles[index++] = (y + 1) + (x + 1) * size;

                triangles[index++] = (y + 0) + (x + 0) * size;
                triangles[index++] = (y + 1) + (x + 1) * size;
                triangles[index++] = (y + 0) + (x + 1) * size;
		    }
	    }
	
	    count = index;		
	    return triangles;
    }

    static uint AddQuad (int[] triangles, uint index, int xBase, int yBase)
    {
	    triangles[index++] = (xBase +  0) * kPatchSize + (yBase + 0);
	    triangles[index++] = (xBase +  0) * kPatchSize + (yBase + 1);
	    triangles[index++] = (xBase +  1) * kPatchSize + (yBase + 1);

	    triangles[index++] = (xBase +  0) * kPatchSize + (yBase + 0);
	    triangles[index++] = (xBase +  1) * kPatchSize + (yBase + 1);
	    triangles[index++] = (xBase +  1) * kPatchSize + (yBase + 0);
	
	    return index;
    }

    /***************************************************************************************
     * 功能：添加衔接边三角形
     ***************************************************************************************/

    static uint AddSliverTriangles(int[] triangles, uint index, int direction, int edgeMask)
    {
	    int directionMask = 1 << direction;

	    if ((edgeMask & directionMask) != 0)
	    {
		    for (int y = 2; y < kPatchSize-3; y++)
		    {
			    if (direction == kDirectionLeft)
				    index = AddQuad(triangles, index, 0, y);	
			    else if (direction == kDirectionRight)
				    index = AddQuad(triangles, index, kPatchSize - 2, y);	
			    else if (direction == kDirectionUp)
				    index = AddQuad(triangles, index, y, kPatchSize - 2);	
			    else if (direction == kDirectionDown)
				    index = AddQuad(triangles, index, y, 0);	
		    }
	    }
	    else
	    {
		    for (int i = 2; i < kPatchSize - 3; i += 2)
		    {
			    if (direction == kDirectionLeft)
			    {
				    int x = 0;
				    int y = i;

				    // 创建底部三角形
				    triangles[index++] = (x + 1) * kPatchSize + (y + 0);
				    triangles[index++] = (x + 0) * kPatchSize + (y + 0);
				    triangles[index++] = (x + 1) * kPatchSize + (y + 1);
				
				    // 创建中间三角形
				    triangles[index++] = (x + 0) * kPatchSize + (y + 0);
				    triangles[index++] = (x + 0) * kPatchSize + (y + 2);
				    triangles[index++] = (x + 1) * kPatchSize + (y + 1);

				    // 创建上边三角形
				    triangles[index++] = (x + 0) * kPatchSize + (y + 2);
				    triangles[index++] = (x + 1) * kPatchSize + (y + 2);
				    triangles[index++] = (x + 1) * kPatchSize + (y + 1);
			    }

			    else if (direction == kDirectionRight)
			    {
				    int x = kPatchSize - 1;
				    int y = i;

                    // 创建底部三角形
				    triangles[index++] = (x -  1) * kPatchSize + (y + 0);
				    triangles[index++] = (x -  1) * kPatchSize + (y + 1);
				    triangles[index++] = (x -  0) * kPatchSize + (y + 0);

                    // 创建中间三角形
				    triangles[index++] = (x -  0) * kPatchSize + (y + 0);
				    triangles[index++] = (x -  1) * kPatchSize + (y + 1);
				    triangles[index++] = (x -  0) * kPatchSize + (y + 2);

                    // 创建上边三角形
				    triangles[index++] = (x -  0) * kPatchSize + (y + 2);
				    triangles[index++] = (x -  1) * kPatchSize + (y + 1);
				    triangles[index++] = (x -  1) * kPatchSize + (y + 2);
			    }				
			    else if (direction == kDirectionDown)
			    {
				    int x = i;
				    int y = 0;

                    // 创建底部三角形
				    triangles[index++] = (x +  0) * kPatchSize + (y + 0);
				    triangles[index++] = (x +  0) * kPatchSize + (y +  1);
				    triangles[index++] = (x +  1) * kPatchSize + (y + 1);

                    // 创建中间三角形
				    triangles[index++] = (x +  1) * kPatchSize + (y + 1);
				    triangles[index++] = (x +  2) * kPatchSize + (y + 0);
				    triangles[index++] = (x +  0) * kPatchSize + (y + 0);

                    // 创建上边三角形
				    triangles[index++] = (x +  2) * kPatchSize + (y + 0);
				    triangles[index++] = (x +  1) * kPatchSize + (y + 1);
				    triangles[index++] = (x +  2) * kPatchSize + (y + 1);
			    }
			    else
			    {
				    int x = i;
				    int y = kPatchSize - 1;

                    // 创建底部三角形
				    triangles[index++] = (x +  0) * kPatchSize + (y - 0);
				    triangles[index++] = (x +  1) * kPatchSize + (y - 1);
				    triangles[index++] = (x +  0) * kPatchSize + (y - 1);

                    // 创建中间三角形
				    triangles[index++] = (x +  1) * kPatchSize + (y - 1);
				    triangles[index++] = (x +  0) * kPatchSize + (y - 0);
				    triangles[index++] = (x +  2) * kPatchSize + (y - 0);

                    // 创建上边三角形
				    triangles[index++] = (x +  2) * kPatchSize + (y - 0);
				    triangles[index++] = (x +  2) * kPatchSize + (y - 1);
				    triangles[index++] = (x +  1) * kPatchSize + (y - 1);
			    }
		    }
	    }
	    return index;
    }

    /***************************************************************************************
     * 功能：翻转三角形
     ***************************************************************************************/

    static void FlipTriangle (int[] triangles, uint index)
    {
	    int temp = triangles[index];
	    triangles[index] = triangles[index+1];
	    triangles[index+1] = temp;
    }

    /***************************************************************************************
     * 功能：衔接拐角
     ***************************************************************************************/

    static uint AddSliverCorner (int[] triangles, uint index, int direction, int edgeMask)
    {
	    int xBase, yBase, ox, oy;
	    bool flip = false;
	
	    int vMask = 0;
	    int hMask = 0;

        // 衔接左下角拐点
	    if (direction == kDirectionLeftDown)
	    {
		    xBase = 1;
		    yBase = 1;
		    ox = 1;
		    oy = 1;
		    flip = false;

		    hMask = 1 << kDirectionLeft;
		    vMask = 1 << kDirectionDown;
	    }
        // 衔接右下角拐点
	    else if (direction == kDirectionRightDown)
	    {
		    xBase = kPatchSize-2;
		    yBase = 1;
		    ox = -1;
		    oy = 1;
		    flip = true;

		    hMask = 1 << kDirectionRight;
		    vMask = 1 << kDirectionDown;
	    }
        // 衔接左上角拐点
	    else if (direction == kDirectionLeftUp)
	    {
		    xBase = 1;
		    yBase = kPatchSize-2;
		    ox = 1;
		    oy = -1;
		    flip = true;

		    hMask = 1 << kDirectionLeft;
		    vMask = 1 << kDirectionUp;
	    }
	    else
	    {
		    xBase = kPatchSize-2;
		    yBase = kPatchSize-2;
		    ox = -1;
		    oy = -1;
		    flip = false;

		    hMask = 1 << kDirectionRight;
		    vMask = 1 << kDirectionUp;
	    }

	    int mask = 0;
	    if ((hMask & edgeMask) != 0)
		    mask |= 1;
	    if ((vMask & edgeMask) != 0)
		    mask |= 2;

	
	    if (mask == 1)
	    {
		    
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase + 0);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase + 0);
		
		    
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase + 0);
		
		    
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase - oy);
		
		    if (flip)
		    {
			    FlipTriangle(triangles, index - 9);
			    FlipTriangle(triangles, index - 6);
			    FlipTriangle(triangles, index - 3);
		    }
	    }
	    
	    else if (mask == 2) 
	    {
		    
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase + oy);
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase + oy);

		    
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase + oy);

		    
		    triangles[index++] = (xBase - ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase +  0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase +  0) * kPatchSize + (yBase - oy);
		
		    if (flip)
		    {
			    FlipTriangle(triangles, index - 9);
			    FlipTriangle(triangles, index - 6);
			    FlipTriangle(triangles, index - 3);
		    }

	    }
	    
	    else
	    {
		  
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase + oy);
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase + oy);
		   
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase + 0);
		   
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase + oy);
		   
		    triangles[index++] = (xBase +   0) * kPatchSize + (yBase +  0);
		    triangles[index++] = (xBase +  ox) * kPatchSize + (yBase - oy);
		    triangles[index++] = (xBase -  ox) * kPatchSize + (yBase - oy);
		
		    if (flip)
		    {
			    FlipTriangle(triangles, index - 12);
			    FlipTriangle(triangles, index - 9);
			    FlipTriangle(triangles, index - 6);
			    FlipTriangle(triangles, index - 3);
		    }
	    }

	    return index;		
    }
}
