using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************************************************
 * 类 : 图集区域
 ****************************************************************************************************/
namespace SceneCore
{
    public class PixRect
    {
        public int x;                           // 偏移x
        public int y;                           // 偏移y
        public int width;                       // 区域宽度
        public int height;                      // 区域高度

        public PixRect() 
        {
            
        }

        public PixRect(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.width = w;
            this.height = h;
        }
    }
}
