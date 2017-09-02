using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/*********************************************************************************************
 * 类 : 场景包压缩及解压
 *********************************************************************************************/
public class SceneZip
{
    static public int startRegX = -1;
    static public int startRegY = -1;
    static public int endRegX = 1;
    static public int endRegY = 1;

    static public string zipPath;
    static public string unZipPath = "D:\\test";

    public SceneZip()
    {
       
    }

    static private MemoryStream sceneZipStream;
    static private BinaryWriter sceneZipWriter;
    static private BinaryReader sceneZipReader; 

    /*************************************************************************************
     * 功能 : 压缩
     *************************************************************************************/
    static public void Zip(string scenePath, string zipfilePath)
    {
        zipPath = scenePath;

        string path = zipfilePath;

        sceneZipStream = new MemoryStream();
        sceneZipWriter = new BinaryWriter(sceneZipStream);

        // 压缩场景文件
        ZipScene();

        // 压缩光照贴图
        ZipLightmap();

        // 压缩地域数据
        for (int regX = startRegX; regX <= endRegX; regX++)
        {
            for (int regY = startRegY; regY <= endRegY; regY++)
            {
                ZipRegion(regX, regY);
            }
        }

        // 写压缩包
        if (QFileUtils.Exists(path) == false)
            QFileUtils.CreateFile(path);
        QFileUtils.WriteBytes(path, sceneZipStream.ToArray());
    }

    /*************************************************************************************
     * 功能 : 解压
     *************************************************************************************/
    static public void UnZip(string zipfile, int sceneID)
    {
        // 场景释放目录
        unZipPath = Application.persistentDataPath + "/Resources/Scenes/" + sceneID;
        
        LogSystem.Log("unzip scene file into ->" + unZipPath);

        string path = zipfile;//zipPath + "/Scene.package";
        byte[] bytes = QFileUtils.ReadBinary(path);
        sceneZipReader = new BinaryReader(new MemoryStream(bytes));

        // 解压场景
        UnZipScene();

        // 解压光照贴图
        UnZipLightmap();

        while (sceneZipReader.BaseStream.Position < sceneZipReader.BaseStream.Length)
        {
            UnZipRegion();
        }
    }

    /*************************************************************************************
     * 功能 : 压缩场景
     *************************************************************************************/
    static public void ZipScene()
    {
        string path = zipPath + "/Scene.bytes";

        byte[] bytes = QFileUtils.ReadBinary(path);
        MemoryStream ms = new MemoryStream();
        ms.Write(bytes, 0, bytes.Length);
        ms.Position = 0;

        MemoryStream outms = new MemoryStream();

        StreamZip.Zip(ms, outms);

        byte[] scenebytes = outms.ToArray();

        // 写入地域压缩文件
        sceneZipWriter.Write(scenebytes.Length);
        sceneZipWriter.Write(scenebytes);
    }

    /*************************************************************************************
     * 功能 : 解压场景
     *************************************************************************************/
    static public void UnZipScene()
    {
        string path = unZipPath + "/Scene.bytes"; ;
        int len = sceneZipReader.ReadInt32();
        byte[] bytes = sceneZipReader.ReadBytes(len);

        MemoryStream instream = new MemoryStream();
        instream.Write(bytes, 0, bytes.Length);
        instream.Position = 0;

        MemoryStream outstream = new MemoryStream();

        StreamZip.Unzip(instream, outstream);

        // 写入文件
        string dir = path.Substring(0, path.LastIndexOf("/"));

        if (QFileUtils.ExistsDir(dir) == false)
            QFileUtils.CreateDir(dir);

        if (QFileUtils.Exists(path) == false)
            QFileUtils.CreateFile(path);

        QFileUtils.WriteBytes(path, outstream.ToArray());
    }

    /*************************************************************************************
     * 功能 : 压缩光照贴图
     *************************************************************************************/
    static public void ZipLightmap()
    {
        string path = zipPath + "/Lightmap";

        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] files = di.GetFiles();

        List<string> filePaths = new List<string>();

        //string fileName;
        //string lastName;
        for (int k = 0; k < files.Length; k++)
        {
            //fileName = files[k].Name;
            filePaths.Add(files[k].DirectoryName + "\\" + files[k].Name);
        }

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        StreamZip.MultiZip(filePaths, bw);

        byte[] bytes = ms.ToArray();

        // 写入地域压缩文件
        sceneZipWriter.Write(bytes.Length);
        sceneZipWriter.Write(bytes);
    }

    /*************************************************************************************
     * 功能 : 解压光照贴图
     *************************************************************************************/
    static public void UnZipLightmap()
    {
        int len = sceneZipReader.ReadInt32();
        byte[] bytes = sceneZipReader.ReadBytes(len);

        string path = unZipPath + "\\" + "Lightmap";

        StreamZip.MultiUnzip(bytes, path);
    }

    /*************************************************************************************
     * 功能 : 压缩地域文件
     *************************************************************************************/
    static public void ZipRegion(int regX, int regY)
    {
        string path = zipPath + "/" + regX + "_" + regY;

        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] files = di.GetFiles();

        List<string> filePaths = new List<string>();

        //string fileName;
        //string lastName;
        for (int k = 0; k < files.Length; k++)
        {
            //fileName = files[k].Name;
            //lastName = fileName.Substring(fileName.LastIndexOf(".") + 1);
            //if (lastName != "meta")
                filePaths.Add(files[k].DirectoryName + "\\" + files[k].Name);
        }

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        StreamZip.MultiZip(filePaths, bw);

        byte[] bytes = ms.ToArray();

        // 写入地域压缩文件
        sceneZipWriter.Write(regX);
        sceneZipWriter.Write(regY);
        sceneZipWriter.Write(bytes.Length);
        sceneZipWriter.Write(bytes);
    }


    /*************************************************************************************
     * 功能 : 解压地域文件
     *************************************************************************************/
    static public void UnZipRegion()
    {
        int regX = sceneZipReader.ReadInt32();
        int regY = sceneZipReader.ReadInt32();

        int len = sceneZipReader.ReadInt32();
        byte[] bytes = sceneZipReader.ReadBytes(len);

        string path = unZipPath + "\\" + regX + "_" + regY;

        StreamZip.MultiUnzip(bytes, path);
    }

    
}