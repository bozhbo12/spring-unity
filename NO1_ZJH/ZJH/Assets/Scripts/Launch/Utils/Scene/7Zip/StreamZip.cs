using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SevenZip;


/**************************************************************************************************
 * 类 : 压缩程序
 **************************************************************************************************/
public class StreamZip
{
    /*******************************************************************************************
     * 功能 : 压缩多文件
     *******************************************************************************************/
    public static void MultiZip(List<string> filePaths, BinaryWriter outStream)
    {
       // long pos = 0;
        for (int i = 0; i < filePaths.Count; i++)
        {
            MemoryStream ms = new MemoryStream();
            FileStream fileStream = File.Open(filePaths[i], FileMode.Open);
            // 压缩文件
            Zip(fileStream, ms);

            byte[] bytes = ms.ToArray();

            string fileName = filePaths[i].Substring(filePaths[i].LastIndexOf("\\") + 1);

            outStream.Write(fileName);
            //pos = outStream.BaseStream.Position;
            outStream.Write(bytes.Length);
            outStream.Write(bytes);
        }
    }

    /*******************************************************************************************
     * 功能 : 多文件解压
     *******************************************************************************************/
    public static void MultiUnzip(byte[] zipbytes, string dir)
    {
        // byte[] zipbytes = QFileUtils.ReadBinary(zipFilePath);
        BinaryReader br = new BinaryReader(new MemoryStream(zipbytes));

        int len;
        string fileName;
        while (br.BaseStream.Position < br.BaseStream.Length)
        {
			fileName = br.ReadString();
            len = br.ReadInt32();
            byte[] bytes = br.ReadBytes(len);

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            ms.Position = 0;

            MemoryStream sms = new MemoryStream();

            // 解压文件
            Unzip(ms, sms);

            if (QFileUtils.ExistsDir(dir) == false) 
                QFileUtils.CreateDir(dir);

            string filePath = dir + "//" + fileName;

            if (QFileUtils.Exists(filePath) == false)
                QFileUtils.CreateFile(filePath);

            QFileUtils.WriteBytes(filePath, sms.ToArray());
        }
    }


    /*******************************************************************************************
     * 功能 : 压缩字节流
     *******************************************************************************************/
    public static void Zip(Stream inStream, Stream outStream)
    {
        bool dictionaryIsDefined = false;
        Int32 dictionary = 1 << 21;

        if (!dictionaryIsDefined)
            dictionary = 1 << 23;

        Int32 posStateBits = 2;
        Int32 litContextBits = 3; // for normal files
        // UInt32 litContextBits = 0; // for 32-bit data
        Int32 litPosBits = 0;
        // UInt32 litPosBits = 2; // for 32-bit data
        Int32 algorithm = 2;
        Int32 numFastBytes = 128;
        string mf = "bt4";
        bool eos = false;

        CoderPropID[] propIDs = 
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};
        object[] properties = 
				{
					(Int32)(dictionary),
					(Int32)(posStateBits),
					(Int32)(litContextBits),
					(Int32)(litPosBits),
					(Int32)(algorithm),
					(Int32)(numFastBytes),
					mf,
					eos
				};

        SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
        encoder.SetCoderProperties(propIDs, properties);
        encoder.WriteCoderProperties(outStream);
        Int64 fileSize;

        fileSize = inStream.Length;
        for (int i = 0; i < 8; i++)
            outStream.WriteByte((Byte)(fileSize >> (8 * i)));

        encoder.Code(inStream, outStream, -1, -1, null);
    }

    /*******************************************************************************************
     * 功能 : 解压字节流
     *******************************************************************************************/
    public static void Unzip(Stream inStream, Stream outStream)
    {
        byte[] properties = new byte[5];
        if (inStream.Read(properties, 0, 5) != 5)
            throw (new Exception("input .lzma is too short"));
        SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
        decoder.SetDecoderProperties(properties);

        long outSize = 0;
        for (int i = 0; i < 8; i++)
        {
            int v = inStream.ReadByte();
            if (v < 0)
                throw (new Exception("Can't Read 1"));
            outSize |= ((long)(byte)v) << (8 * i);
        }
        long compressedSize = inStream.Length - inStream.Position;
        decoder.Code(inStream, outStream, compressedSize, outSize, null);
    }
}