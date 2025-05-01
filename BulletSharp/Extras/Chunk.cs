using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BulletSharp;

[StructLayout(LayoutKind.Sequential)]
public class Chunk4
{
    public int Code;
    public int Length;
    public int UniqueInt1;
    public int StructIndex;
    public int NumBlocks;

    public Chunk4()
    {
    }

    public Chunk4(BinaryReader reader)
    {
        Code = reader.ReadInt32();
        Length = reader.ReadInt32();
        UniqueInt1 = reader.ReadInt32();
        StructIndex = reader.ReadInt32();
        NumBlocks = reader.ReadInt32();
    }
}

[StructLayout(LayoutKind.Sequential)]
public class Chunk8
{
    public int Code;
    public int Length;
    public int UniqueInt1;
    public int UniqueInt2;
    public int StructIndex;
    public int NumBlocks;

    public Chunk8()
    {
    }

    public Chunk8(BinaryReader reader)
    {
        Code = reader.ReadInt32();
        Length = reader.ReadInt32();
        UniqueInt1 = reader.ReadInt32();
        UniqueInt2 = reader.ReadInt32();
        StructIndex = reader.ReadInt32();
        NumBlocks = reader.ReadInt32();
    }
}

public class ChunkInd
{
    public DnaID Code;
    public int Length;
    public long OldPtr;
    public int StructIndex;
    public int NumBlocks;

    public ChunkInd(Chunk4 c)
    {
        Code = (DnaID)c.Code;
        Length = c.Length;
        OldPtr = c.UniqueInt1;
        StructIndex = c.StructIndex;
        NumBlocks = c.NumBlocks;
    }

    public ChunkInd(Chunk8 c)
    {
        Code = (DnaID)c.Code;
        Length = c.Length;
        OldPtr = c.UniqueInt1 + ((long)c.UniqueInt2 << 32);
        StructIndex = c.StructIndex;
        NumBlocks = c.NumBlocks;
    }

    public static int Size => Marshal.SizeOf((IntPtr.Size == 8) ? typeof(Chunk8) : typeof(Chunk4));

    public override string ToString()
        => "Chunk: " + Code.ToString();
}

#pragma warning disable SA1204 // Static elements should appear before instance elements
public static class ChunkUtils
#pragma warning restore SA1204 // Static elements should appear before instance elements
{
    // Get the file's chunk size
    public static int GetChunkSize(FileFlags flags)
    {
        bool bitsVaries = (flags & FileFlags.BitsVaries) != 0;
        return bitsVaries
            ? Marshal.SizeOf(IntPtr.Size == 8 ? typeof(Chunk4) : typeof(Chunk8))
            : Marshal.SizeOf(IntPtr.Size == 8 ? typeof(Chunk8) : typeof(Chunk4));
    }
}
