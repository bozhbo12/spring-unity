using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************************************************
 * 类 : 图集探测结果
 ****************************************************************************************************/
namespace SceneCore
{
    public class ProbeResult
    {
        public int width;                               // 图集实际占用宽
        public int height;                              // 图集实际占用高
        public Node root;                               // 根节点
        public bool fitsInMaxSize;                      // 有没超出最大范围 (如1255的区域放入1024图片中)
        public float efficiency;                        // 图集利用率
        public float squareness;                        // 

        public void Set(int width, int height, Node root, bool fits, float efficiency, float squareness)
        {
            this.width = width;
            this.height = height;
            this.root = root;
            this.fitsInMaxSize = fits;
            this.efficiency = efficiency;
            this.squareness = squareness;
        }

        /*********************************************************************************************
         * 功能 : 
         *********************************************************************************************/
        public float GetScore()
        {
            float fitsScore = fitsInMaxSize ? 1f : 0f;
            return squareness + 2 * efficiency + fitsScore;
        }
    }
}
