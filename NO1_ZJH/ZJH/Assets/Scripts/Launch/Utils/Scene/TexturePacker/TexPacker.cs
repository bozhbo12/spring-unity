using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/****************************************************************************************************
 * 类 : 
 ****************************************************************************************************/
namespace SceneCore
{
    public class TexPacker
    {
        class ImgIDComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.id > y.id)
                    return 1;
                if (x.id == y.id)
                    return 0;
                return -1;
            }
        }

        class ImageHeightComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.height > y.height)
                    return -1;
                if (x.height == y.height)
                    return 0;
                return 1;
            }
        }

        class ImageWidthComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.width > y.width)
                    return -1;
                if (x.width == y.width)
                    return 0;
                return 1;
            }
        }

        class ImageAreaComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                int ax = x.width * x.height;
                int ay = y.width * y.height;
                if (ax > ay)
                    return -1;
                if (ax == ay)
                    return 0;
                return 1;
            }
        }

		class ImageIndexComparer : IComparer<Image>
		{
			public int Compare(Image x, Image y)
			{
				if (x.index > y.index)
					return 1;
				if (x.index == y.index)
					return 0;
				return -1;
			}
		}

        /*********************************************************************************************
         * 功能 : 获取范围
         *********************************************************************************************/
        void GetExtent(Node r, ref int x, ref int y)
        {
            if (r.img != null) {
                if (r.pixRect.x + r.img.width > x)
                    x = r.pixRect.x + r.img.width;

                if (r.pixRect.y + r.img.height > y)
                    y = r.pixRect.y + r.img.height;
            }

            if (r.child[0] != null)
                GetExtent(r.child[0], ref x, ref y);
            if (r.child[1] != null)
                GetExtent(r.child[1], ref x, ref y);
        }

        /*********************************************************************************************
         * 功能 : 探测
         * idealAtlasW 理想图集宽度
         * idealAtlasH 理想图集高度
         *********************************************************************************************/
        bool Probe(Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDimension, ProbeResult pr)
        {
            Node root = new Node();
            // 初始化根节点的范围
            root.pixRect = new PixRect(0, 0, idealAtlasW, idealAtlasH);
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                Node n = root.Insert(imgsToAdd[i], false);
                if (n == null) {
                    return false;
                }
                else if (i == imgsToAdd.Length - 1) {
                    int usedW = 0;
                    int usedH = 0;

                    // 获取使用范围
                    GetExtent(root, ref usedW, ref usedH);

                    // 计算利用率
                    float efficiency = 1f - (usedW * usedH - imgArea) / (usedW * usedH);

                    float squareness;
                    if (usedW < usedH) 
                        squareness = (float)usedW / (float)usedH;
                    else 
                        squareness = (float)usedH / (float)usedW;

                    // 是否在最大图集填充范围内
                    bool fitsInMaxDim = usedW <= maxAtlasDimension && usedH <= maxAtlasDimension;

                    pr.Set(usedW, usedH, root, fitsInMaxDim, efficiency, squareness);

                    return true;
                }
            }
            // 不应该执行到这里
            return false;
        }

        public int atlasCount = 0;

        public Image[] Pack(List<Vector2> imgWidthHeights, int maxDimension, int padding)
        {
            atlasCount = 1;
            List<Image> pImgs = new List<Image>();
            float area = 0;
            int maxW = 0;
            int maxH = 0;
            Image[] imgsToAdd = new Image[imgWidthHeights.Count];
            int maxArea = (int)(maxDimension * maxDimension * 0.9);
            int atlasIndex = 0;
            // 初始化图集
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                Image im = imgsToAdd[i] = new Image(i, (int)imgWidthHeights[i].x, (int)imgWidthHeights[i].y, padding);
				im.index = i;
                maxW = Mathf.Max(maxW, im.width);
                maxH = Mathf.Max(maxH, im.height);
            }

            // 对图集进行排序
            if ((float)maxH / (float)maxW > 2)
                Array.Sort(imgsToAdd, new ImageHeightComparer());
            else if ((float)maxH / (float)maxW < .5)
                Array.Sort(imgsToAdd, new ImageWidthComparer());
            else
                Array.Sort(imgsToAdd, new ImageAreaComparer());

            List<Image> curImgs = new List<Image>();

            Node root = new Node();
            root.pixRect = new PixRect(0, 0, maxDimension, maxDimension);
  
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                Image im = imgsToAdd[i];
                Node n = root.Insert(imgsToAdd[i], false);
                if (n == null)
                {
                    root = new Node();
                    root.pixRect = new PixRect(0, 0, maxDimension, maxDimension);
                    root.Insert(imgsToAdd[i], false);

                    atlasIndex++;
                    atlasCount++;
                    int w = 0;
                    int h = 0;
                    Image[] imgs = GetRects(curImgs.ToArray(), maxDimension, padding, out w, out h);
                    pImgs.AddRange(imgs);
                    curImgs.Clear();
                }
                im.atlasIndex = atlasIndex;
                curImgs.Add(im);
            }
            
            {
                int w = 0;
                int h = 0;
                Image[] imgs = GetRects(curImgs.ToArray(), maxDimension, padding, out w, out h);
                pImgs.AddRange(imgs);
            }

			Image[] res = pImgs.ToArray();
			Array.Sort(res, new ImageIndexComparer());
			return res;
        }

        /*********************************************************************************************
         * 功能 : 
         *********************************************************************************************/
        public Image[] GetRects(Image[] imgsToAdd, int maxDimension, int padding, out int outW, out int outH)
        {   
            ProbeResult bestRoot = null;
            float area = 0;
            int maxW = 0;
            int maxH = 0;

            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                Image im = imgsToAdd[i];
                area += im.width * im.height;
                maxW = Mathf.Max(maxW, im.width);
                maxH = Mathf.Max(maxH, im.height);
            }
            // Image[] imgsToAdd = new Image[imgWidthHeights.Count];

            // 初始化图集
            /**
            for (int i = 0; i < imgsToAdd.Length; i++)
            {
                Image im = imgsToAdd[i] = new Image(i, (int)imgWidthHeights[i].x, (int)imgWidthHeights[i].y, padding);
                area += im.width * im.height;
                maxW = Mathf.Max(maxW, im.width);
                maxH = Mathf.Max(maxH, im.height);
            }
            **/

            // 对图集进行排序
            /**
            if ((float)maxH / (float)maxW > 2)
                Array.Sort(imgsToAdd, new ImageHeightComparer());
            else if ((float)maxH / (float)maxW < .5)
                Array.Sort(imgsToAdd, new ImageWidthComparer());
            else
                Array.Sort(imgsToAdd, new ImageAreaComparer());
            **/

            // 寻找高效合理的填充
            int sqrtArea = (int)Mathf.Sqrt(area);
            int idealAtlasW = sqrtArea;
            int idealAtlasH = sqrtArea;
            if (maxW > sqrtArea)
            {
                idealAtlasW = maxW;
                idealAtlasH = Mathf.Max(Mathf.CeilToInt(area / maxW), maxH);
            }
            if (maxH > sqrtArea)
            {
                idealAtlasW = Mathf.Max(Mathf.CeilToInt(area / maxH), maxW);
                idealAtlasH = maxH;
            }
            if (idealAtlasW == 0) idealAtlasW = 1;
            if (idealAtlasH == 0) idealAtlasH = 1;

            int stepW = (int)(idealAtlasW * .15f);
            int stepH = (int)(idealAtlasH * .15f);

            if (stepW == 0) stepW = 1;
            if (stepH == 0) stepH = 1;

            // 计算理想图集总尺寸
            /**
            int numWIterations = 2;
            int steppedHeight = idealAtlasH;
            while (numWIterations > 1 && steppedHeight < sqrtArea * 1000)
            {
                bool successW = false;
                numWIterations = 0;
                int steppedWidth = idealAtlasW;
                while (!successW && steppedWidth < sqrtArea * 1000)
                {
                    ProbeResult pr = new ProbeResult();
                    if (Probe(imgsToAdd, steppedWidth, steppedHeight, area, maxDimension, pr))
                    {
                        successW = true;
                        if (bestRoot == null)
                            bestRoot = pr;
                        else if (pr.GetScore() > bestRoot.GetScore())
                            bestRoot = pr;
                    }
                    else
                    {
                        numWIterations++;
                        steppedWidth += stepW;
                    }
                }
                steppedHeight += stepH;
            }
            **/
            ProbeResult pr = new ProbeResult();
            if (Probe(imgsToAdd, maxDimension, maxDimension, area, maxDimension, pr))
                bestRoot = pr;

            outW = 0;
            outH = 0;
            if (bestRoot == null) 
                return null;

            outW = bestRoot.width;
            outH = bestRoot.height;

            List<Image> images = new List<Image>();
            flattenTree(bestRoot.root, images);
            images.Sort(new ImgIDComparer());

            // 打印images
            // for (int i = 0; i < images.Count; i++)
                // Debug.Log(images[i].x + " - " + images[i].y + " - " + images[i].width + " - " + images[i].height);

            if (images.Count != imgsToAdd.Length)
                LogSystem.LogWarning("Result images not the same lentgh as source");

            // 当图片太大将进行缩放
            /**
            float padX = (float)padding / (float)bestRoot.width;
            if (bestRoot.width > maxDimension)
            {
                padX = (float)padding / (float)maxDimension;
                float scaleFactor = 1f;
                for (int i = 0; i < images.Count; i++)
                {
                    Image im = images[i];
                    int right = (int)((im.x + im.width) * scaleFactor);
                    im.x = (int)(scaleFactor * im.x);
                    im.width = right - im.x;
                    if (im.width == 0) 
                        LogSystem.LogWarning("rounding scaled image w to zero");
                }
                outW = maxDimension;
            }

            float padY = (float)padding / (float)bestRoot.height;
            if (bestRoot.height > maxDimension)
            {
                padY = (float)padding / (float)maxDimension;
                float scaleFactor = 1f;// (float)maxDimension / (float)bestRoot.h;
                for (int i = 0; i < images.Count; i++)
                {
                    Image im = images[i];
                    int bottom = (int)((im.y + im.height) * scaleFactor);
                    im.y = (int)(scaleFactor * im.y);
                    im.height = bottom - im.y;
                    if (im.height == 0)
                        LogSystem.LogWarning("rounding scaled image h to zero");
                }
                outH = maxDimension;
            }

            Rect[] rs = new Rect[images.Count];
            for (int i = 0; i < images.Count; i++)
            {
                Image im = images[i];
                Rect r = rs[i] = new Rect((float)im.x / (float)outW + padX,
                                 (float)im.y / (float)outH + padY,
                                 (float)im.width / (float)outW - padX * 2,
                                 (float)im.height / (float)outH - padY * 2);

            }
            **/
            return images.ToArray();
        }

        static void printTree(Node r, string spc)
        {
            if (r.child[0] != null)
                printTree(r.child[0], spc + "  ");
            if (r.child[1] != null)
                printTree(r.child[1], spc + "  ");
        }

        static void flattenTree(Node r, List<Image> putHere)
        {
            if (r.img != null)
            {
                r.img.x = r.pixRect.x;
                r.img.y = r.pixRect.y;
                putHere.Add(r.img);
            }
            if (r.child[0] != null)
                flattenTree(r.child[0], putHere);
            if (r.child[1] != null)
                flattenTree(r.child[1], putHere);
        }

    }


}
