    using UnityEngine;
	using System.Collections;
	using System.Text; 

public class TypeConvert 
{
	
	 public TypeConvert()
	 {
		
	 }
	 public static byte[] shortToBytes(short data)
	 {
		 byte[] bytesRet = new byte[2];
		 bytesRet[0] = (byte) ((data >> 8) & 0xFF);
		 bytesRet[1] = (byte) (data & 0xFF);
		 return bytesRet;
	 }
	
	public static short bytesToShort(byte[] b)
	{
	    short sRet = 0;
	    sRet += (short)((b[0] & 0xFF) << 8);
	    sRet += (short)(b[1] & 0xFF);
	    return sRet;
	}
	
     public static int bytesToInt(byte[] b)
	 {
	     int value = 0;
	     for (int i = 0; i < b.Length; i++) {
	            int shift = (b.Length - 1 - i) * 8;
	            value += (b[i] & 0xFF) << shift;
	       }
	       return value;
	   } 
	 
	public static byte[] intToBytes(int n) 
	{
		 byte[] b = new byte[4];
		 for(int i = 0;i < 4;i++){
		 b[i] = (byte)(n >> (24 - i * 8)); 
		 }
		 return b;
	}
	 
	public static byte[] stringToBytes(string str) 
	{
		
		 
	  	if(str!=null&&str.Length>0)
	 	{
			byte [] m = null ;
			m = Encoding.UTF8.GetBytes(str);
			if(m!=null&&m.Length>0)
			{
				byte[] n  =new byte[m.Length+2];
					
				System.Array.Copy(m, 0, n,2, m.Length);
				byte[] k = shortToBytes((short) m.Length);
				System.Array.Copy(k, 0, n,0, 2);
					
				return n;
			}
				
		}
		return shortToBytes((short) 0);
	}
	 
	 
	public static long bytesToLong(byte[] b)
	{

     int mask = 0xff;
     int temp = 0;
     int res = 0;
     for (int i = 0; i < 8; i++) {
      res <<= 8;
      temp = b[i] & mask;
     res |= temp;
     }
     return res;
   }

 public static byte[] longToBytes( long lValue )

  {

  byte[] bData = new byte[8];

    

  bData[0] = (byte)( ( lValue >> 56 ) & 0xFF);

  bData[1] = (byte)( ( lValue >> 48 ) & 0xFF);

  bData[2] = (byte)( ( lValue >> 40 ) & 0xFF);

  bData[3] = (byte)( ( lValue >> 32 ) & 0xFF);

  bData[4] = (byte)( ( lValue >> 24 ) & 0xFF);

  bData[5] = (byte)( ( lValue >> 16 ) & 0xFF);

  bData[6] = (byte)( ( lValue >> 8 ) & 0xFF);

  bData[7] = (byte)(lValue & 0xFF);

  return bData;

  }

 
	
	
}
