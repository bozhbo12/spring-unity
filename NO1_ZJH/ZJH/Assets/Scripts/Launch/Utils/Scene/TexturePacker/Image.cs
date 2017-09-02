using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************************************************
 * 类 : 
 ****************************************************************************************************/
namespace SceneCore
{
    public class Image
    {
        public int id;
        public int width;
        public int height;
        public int x;
        public int y;
        public int atlasIndex = -1;
		public int index = -1;

        public Image(int id, int width, int height, int padding)
        {
            this.id = id;
            this.width = width + padding * 2;
            this.height = height + padding * 2;
        }
    }
}
