using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Rand 
{
	private UInt32 x, y, z, w;

	public Rand (UInt32 seed = 0)
	{
		SetSeed (seed);
	}

    public UInt32 Get()
	{
		UInt32 t;
		t = x ^ (x << 11);
		x = y; y = z; z = w;
		return w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
	}

    public static float GetFloatFromInt(UInt32 value)
	{
		// take 23 bits of integer, and divide by 2^23-1
		return (float)(value & 0x007fffff) * (1.0f / 8388607.0f);
	}

    public static byte GetByteFromInt(UInt32 value)
	{
		// take the most significant byte from the 23-bit value
		return (byte)(value >> (23 - 8));
	}
	
	// random number between 0.0 and 1.0
    public float GetFloat()
	{
		return GetFloatFromInt (Get ());
	}

	// random number between -1.0 and 1.0
    public float GetSignedFloat()
	{
	    return GetFloat() * 2.0f - 1.0f;
	}

	public void SetSeed (UInt32 seed)
	{
		x = seed;
		y = x * 1812433253U + 1;
		z = y * 1812433253U + 1;
		w = z * 1812433253U + 1;
	}
	
	UInt32 GetSeed ()  { return x; }
	

};

