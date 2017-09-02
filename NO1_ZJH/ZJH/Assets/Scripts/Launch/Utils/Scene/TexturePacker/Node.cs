using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneCore;

/****************************************************************************************************
 * 类 : 
 ****************************************************************************************************/
namespace SceneCore
{
    public class Node
    {
        public Node[] child = new Node[2];                      // 左边节点的空间永远大于右边的节点
        public PixRect pixRect;
        public Image img;

        /*********************************************************************************************
         * 功能 : 是否是节叶
         *********************************************************************************************/
        bool isLeaf()
        {
            if (child[0] == null || child[1] == null)
            {
                return true;
            }
            return false;
        }


        /*********************************************************************************************
         * 功能 : 插入节点
         *********************************************************************************************/
        public Node Insert(Image img, bool handed)
        {
            int a = 0, b = 0;
            if (handed == true) {
                a = 0;
                b = 1;
            }
            else if (handed == false) {
                a = 1;
                b = 0;
            }

            if (isLeaf() == false) {
                // 尝试插入到子集
                Node newNode = child[a].Insert(img, handed);
                if (newNode != null)
                    return newNode;
                // 当左边节点没有空间则插入右边节点
                return child[b].Insert(img, handed);
            }
            else {

                // 如果当前节点已经有图集则返回
                if (this.img != null)
                    return null;

                // 范围不够返回
                if (pixRect.width < img.width || pixRect.height < img.height)
                    return null;

                // 如果刚好范围可以插入一直图片则直接返回
                if (pixRect.width == img.width && pixRect.height == img.height)
                {
                    this.img = img;
                    return this;
                }

                // 当前节点范围大于图片则进行进一步分割
                child[a] = new Node();
                child[b] = new Node();

                // 决定以哪种方式分割
                int dw = pixRect.width - img.width;
                int dh = pixRect.height - img.height;

                if (dw > dh)
                {
                    child[a].pixRect = new PixRect(pixRect.x, pixRect.y, img.width, pixRect.height);
                    child[b].pixRect = new PixRect(pixRect.x + img.width, pixRect.y, pixRect.width - img.width, pixRect.height);
                }
                else
                {
                    child[a].pixRect = new PixRect(pixRect.x, pixRect.y, pixRect.width, img.height);
                    child[b].pixRect = new PixRect(pixRect.x, pixRect.y + img.height, pixRect.width, pixRect.height - img.height);
                }
                return child[a].Insert(img, handed);
            }
        }
    }
}
