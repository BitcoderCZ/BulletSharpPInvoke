using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

[Flags]
public enum SerializationFlags
{
    NoBvh = 1,
    NoTriangleInfoMap = 2,
    NoDuplicateAssert = 4,
}

public class Chunk : BulletDisposableObject
{
    public Chunk()
    {
        IntPtr native = btChunk_new();
        InitializeUserOwned(native);
    }

    internal Chunk(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public int ChunkCode
    {
        get => btChunk_getChunkCode(Native);
        set => btChunk_setChunkCode(Native, value);
    }

    public int DnaNr
    {
        get => btChunk_getDna_nr(Native);
        set => btChunk_setDna_nr(Native, value);
    }

    public int Length
    {
        get => btChunk_getLength(Native);
        set => btChunk_setLength(Native, value);
    }

    public int Number
    {
        get => btChunk_getNumber(Native);
        set => btChunk_setNumber(Native, value);
    }

    public IntPtr OldPtr
    {
        get => btChunk_getOldPtr(Native);
        set => btChunk_setOldPtr(Native, value);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsUserOwned)
        {
            btChunk_delete(Native);
        }
    }
}

public abstract class Serializer : BulletDisposableObject
{
    private static byte[]? dna;
    private static byte[]? dna64;

    private readonly AllocateUnmanagedDelegate _allocate;
    private readonly FinalizeChunkUnmanagedDelegate _finalizeChunk;
    private readonly FindNameForPointerUnmanagedDelegate _findNameForPointer;
    private readonly FindPointerUnmanagedDelegate _findPointer;
    private readonly FinishSerializationUnmanagedDelegate _finishSerialization;
    private readonly GetBufferPointerUnmanagedDelegate _getBufferPointer;
    private readonly GetChunkUnmanagedDelegate _getChunk;
    private readonly GetCurrentBufferSizeUnmanagedDelegate _getCurrentBufferSize;
    private readonly GetNumChunksUnmanagedDelegate _getNumChunks;
    private readonly GetSerializationFlagsUnmanagedDelegate _getSerializationFlags;
    private readonly GetUniquePointerUnmanagedDelegate _getuniquePointer;
    private readonly RegisterNameForPointerUnmanagedDelegate _registernameForPointer;
    private readonly SerializeNameUnmanagedDelegate _serializeName;
    private readonly SetSerializationFlagsUnmanagedDelegate _setSerializationFlags;
    private readonly StartSerializationUnmanagedDelegate _startSerialization;

    public Serializer()
    {
        _allocate = new AllocateUnmanagedDelegate(AllocateUnmanaged);
        _finalizeChunk = new FinalizeChunkUnmanagedDelegate(FinalizeChunk);
        _findNameForPointer = new FindNameForPointerUnmanagedDelegate(FindNameForPointer);
        _findPointer = new FindPointerUnmanagedDelegate(FindPointer);
        _finishSerialization = new FinishSerializationUnmanagedDelegate(FinishSerialization);
        _getBufferPointer = new GetBufferPointerUnmanagedDelegate(GetBufferPointer);
        _getChunk = new GetChunkUnmanagedDelegate(GetChunk);
        _getCurrentBufferSize = new GetCurrentBufferSizeUnmanagedDelegate(GetCurrentBufferSize);
        _getNumChunks = new GetNumChunksUnmanagedDelegate(GetNumChunks);
        _getSerializationFlags = new GetSerializationFlagsUnmanagedDelegate(GetSerializationFlags);
        _getuniquePointer = new GetUniquePointerUnmanagedDelegate(GetUniquePointer);
        _registernameForPointer = new RegisterNameForPointerUnmanagedDelegate(RegisterNameForPointer);
        _serializeName = new SerializeNameUnmanagedDelegate(SerializeName);
        _setSerializationFlags = new SetSerializationFlagsUnmanagedDelegate(SetSerializationFlags);
        _startSerialization = new StartSerializationUnmanagedDelegate(StartSerialization);

        IntPtr native = btSerializerWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_allocate),
            Marshal.GetFunctionPointerForDelegate(_finalizeChunk),
            Marshal.GetFunctionPointerForDelegate(_findNameForPointer),
            Marshal.GetFunctionPointerForDelegate(_findPointer),
            Marshal.GetFunctionPointerForDelegate(_finishSerialization),
            Marshal.GetFunctionPointerForDelegate(_getBufferPointer),
            Marshal.GetFunctionPointerForDelegate(_getChunk),
            Marshal.GetFunctionPointerForDelegate(_getCurrentBufferSize),
            Marshal.GetFunctionPointerForDelegate(_getNumChunks),
            Marshal.GetFunctionPointerForDelegate(_getSerializationFlags),
            Marshal.GetFunctionPointerForDelegate(_getuniquePointer),
            Marshal.GetFunctionPointerForDelegate(_registernameForPointer),
            Marshal.GetFunctionPointerForDelegate(_serializeName),
            Marshal.GetFunctionPointerForDelegate(_setSerializationFlags),
            Marshal.GetFunctionPointerForDelegate(_startSerialization));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr AllocateUnmanagedDelegate(uint size, int numElements);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void FinalizeChunkUnmanagedDelegate(IntPtr chunk, string structType, DnaID chunkCode, IntPtr oldPtr);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr FindNameForPointerUnmanagedDelegate(IntPtr ptr);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr FindPointerUnmanagedDelegate(IntPtr ptr);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void FinishSerializationUnmanagedDelegate();

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr GetBufferPointerUnmanagedDelegate();

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr GetChunkUnmanagedDelegate(int chunkIndex);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int GetCurrentBufferSizeUnmanagedDelegate();

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int GetNumChunksUnmanagedDelegate();

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int GetSerializationFlagsUnmanagedDelegate();

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr GetUniquePointerUnmanagedDelegate(IntPtr oldPtr);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void RegisterNameForPointerUnmanagedDelegate(IntPtr ptr, string name);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void SerializeNameUnmanagedDelegate(IntPtr ptr);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void SetSerializationFlagsUnmanagedDelegate(int flags);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void StartSerializationUnmanagedDelegate();

    public abstract IntPtr BufferPointer { get; }

    public abstract int CurrentBufferSize { get; }

    public abstract SerializationFlags SerializationFlags { get; set; }

    public static byte[] GetBulletDna()
    {
        if (dna == null)
        {
            int length = getBulletDNAlen();
            dna = new byte[length];
            Marshal.Copy(getBulletDNAstr(), dna, 0, length);
        }

        return dna;
    }

    public static byte[] GetBulletDna64()
    {
        if (dna64 == null)
        {
            int length = getBulletDNAlen64();
            dna64 = new byte[length];
            Marshal.Copy(getBulletDNAstr64(), dna64, 0, length);
        }

        return dna64;
    }

    public abstract Chunk Allocate(uint size, int numElements);

    public abstract void FinalizeChunk(Chunk chunkPtr, string structType, DnaID chunkCode, IntPtr oldPtr);

    public abstract IntPtr FindNameForPointer(IntPtr ptr);

    public abstract IntPtr FindPointer(IntPtr oldPtr);

    public abstract void FinishSerialization();

    public abstract IntPtr GetChunk(int chunkIndex);

    public abstract IntPtr GetUniquePointer(IntPtr oldPtr);

    public abstract int GetNumChunks();

    public abstract void RegisterNameForObject(object obj, string name);

    public abstract void SerializeName(IntPtr ptr);

    public abstract void StartSerialization();

    protected override void Dispose(bool disposing) => btSerializer_delete(Native);

    private IntPtr AllocateUnmanaged(uint size, int numElements)
        => Allocate(size, numElements).Native;

    private void FinalizeChunk(IntPtr chunkPtr, string structType, DnaID chunkCode, IntPtr oldPtr)
        => FinalizeChunk(new Chunk(chunkPtr, this), structType, chunkCode, oldPtr);

    private IntPtr GetBufferPointer()
        => throw new NotImplementedException();

    private int GetCurrentBufferSize()
        => CurrentBufferSize;

    private int GetSerializationFlags()
        => (int)SerializationFlags;

    private void RegisterNameForPointer(IntPtr ptr, string name)
        => throw new NotImplementedException();

    private void SetSerializationFlags(int flags)
        => SerializationFlags = (SerializationFlags)flags;
}

public class DefaultSerializer : Serializer
{
    private readonly int _totalSize;
    private readonly Dictionary<IntPtr, IntPtr> _chunkP = [];
    private readonly Dictionary<IntPtr, IntPtr> _uniquePointers = [];
    private readonly Dictionary<object, IntPtr> _nameMap = [];
    private readonly List<Chunk> _chunks = [];

    private IntPtr _buffer;
    private int _currentSize;
    private IntPtr _uniqueIdGenerator;
    private SerializationFlags _serializationFlags;

    private byte[] _dnaData;
    private Dna _dna;

    public DefaultSerializer()
        : this(0)
    {
    }

    public DefaultSerializer(int totalSize)
    {
        _currentSize = 0;
        _totalSize = totalSize;

        _buffer = (_totalSize != 0) ? Marshal.AllocHGlobal(_totalSize) : IntPtr.Zero;

        InitDna();
    }

    public override IntPtr BufferPointer => _buffer;

    public override int CurrentBufferSize => _currentSize;

    public override SerializationFlags SerializationFlags
    {
        get => _serializationFlags;
        set => _serializationFlags = value;
    }

    public override Chunk Allocate(uint size, int numElements)
    {
        int length = (int)size * numElements;
        IntPtr ptr = InternalAlloc(length + ChunkInd.Size);
        IntPtr data = ptr + ChunkInd.Size;
        Chunk chunk = new Chunk(ptr, this)
        {
            ChunkCode = 0,
            OldPtr = data,
            Length = length,
            Number = numElements,
        };
        _chunks.Add(chunk);
        return chunk;
    }

    public override void FinalizeChunk(Chunk chunk, string structType, DnaID chunkCode, IntPtr oldPtr)
    {
        if ((SerializationFlags & SerializationFlags.NoDuplicateAssert) == 0)
        {
            Debug.Assert(FindPointer(oldPtr) == IntPtr.Zero);
        }

        Dna.StructDecl? structDecl = _dna.GetStruct(structType);
        for (int i = 0; i < _dna.NumStructs; i++)
        {
            if (_dna.GetStruct(i) == structDecl)
            {
                chunk.DnaNr = i;
                break;
            }
        }

        chunk.ChunkCode = (int)chunkCode;
        IntPtr uniquePtr = GetUniquePointer(oldPtr);

        _chunkP.Add(oldPtr, uniquePtr);//chunk->m_oldPtr);
        chunk.OldPtr = uniquePtr;//oldPtr;
    }

    public override IntPtr FindNameForPointer(IntPtr ptr)
    {
        IntPtr name;
        _nameMap.TryGetValue(ptr, out name);
        return name;
    }

    public override IntPtr FindPointer(IntPtr oldPtr)
    {
        IntPtr ptr;
        _chunkP.TryGetValue(oldPtr, out ptr);
        return ptr;
    }

    public override void FinishSerialization()
    {
        WriteDna();

        //if we didn't pre-allocate a buffer, we need to create a contiguous buffer now
        if (_totalSize == 0)
        {
            if (_buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_buffer);
            }

            _currentSize += 12; // header
            _buffer = Marshal.AllocHGlobal(_currentSize);

            IntPtr currentPtr = _buffer;
            WriteHeader(_buffer);
            currentPtr += 12;
            foreach (Chunk chunk in _chunks)
            {
                if (IntPtr.Size == 8)
                {
                    Chunk8 chunkPtr = new Chunk8();
                    Marshal.PtrToStructure(chunk.Native, chunkPtr);
                    Marshal.StructureToPtr(chunkPtr, currentPtr, false);
                }
                else
                {
                    Chunk4 chunkPtr = new Chunk4();
                    Marshal.PtrToStructure(chunk.Native, chunkPtr);
                    Marshal.StructureToPtr(chunkPtr, currentPtr, false);
                }

                currentPtr += ChunkInd.Size + chunk.Length;
            }
        }

        foreach (IntPtr ptr in _nameMap.Values)
        {
            Marshal.FreeHGlobal(ptr);
        }

        _chunkP.Clear();
        _nameMap.Clear();
        _uniquePointers.Clear();
        _chunks.Clear();
    }

    public override IntPtr GetChunk(int chunkIndex) => _chunks[chunkIndex].Native;

    public override int GetNumChunks() => _chunks.Count;

    public override IntPtr GetUniquePointer(IntPtr oldPtr)
    {
        if (oldPtr == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        IntPtr uniquePtr;
        if (_uniquePointers.TryGetValue(oldPtr, out uniquePtr))
        {
            return uniquePtr;
        }

        _uniqueIdGenerator = IntPtr.Add(_uniqueIdGenerator, 1);
        _uniquePointers.Add(oldPtr, _uniqueIdGenerator);

        return _uniqueIdGenerator;
    }

    public override void RegisterNameForObject(object obj, string name)
    {
        IntPtr ptr;
        if (obj is CollisionObject)
        {
            ptr = ((CollisionObject)obj).Native;
        }
        else if (obj is CollisionShape)
        {
            ptr = ((CollisionShape)obj).Native;
        }
        else if (obj is TypedConstraint)
        {
            ptr = ((TypedConstraint)obj).Native;
        }
        else
        {
            throw new NotImplementedException();
        }

        IntPtr namePtr = Marshal.StringToHGlobalAnsi(name);
        _nameMap.Add(ptr, namePtr);
    }

    public override void SerializeName(IntPtr namePtr)
    {
        if (namePtr == IntPtr.Zero)
        {
            return;
        }

        //don't serialize name twice
        if (FindPointer(namePtr) != IntPtr.Zero)
        {
            return;
        }

        string name = Marshal.PtrToStringAnsi(namePtr);
        int length = name.Length;
        if (length == 0)
        {
            return;
        }

        int newLen = length + 1;
        int padding = ((newLen + 3) & ~3) - newLen;
        newLen += padding;

        //serialize name string now
        Chunk chunk = Allocate(sizeof(char), newLen);
        IntPtr destPtr = chunk.OldPtr;
        for (int i = 0; i < length; i++)
        {
            Marshal.WriteByte(destPtr, i, (byte)name[i]);
        }

        FinalizeChunk(chunk, "char", DnaID.Array, namePtr);
    }

    public override void StartSerialization()
    {
        _uniqueIdGenerator = new IntPtr(1);
        if (_totalSize != 0)
        {
            IntPtr buffer = InternalAlloc(12);
            WriteHeader(buffer);
        }
    }

    public void WriteDna()
    {
        Chunk dnaChunk = Allocate((uint)_dnaData.Length, 1);
        Marshal.Copy(_dnaData, 0, dnaChunk.OldPtr, _dnaData.Length);
        GCHandle dnaHandle = GCHandle.Alloc(_dnaData, GCHandleType.Pinned);
        FinalizeChunk(dnaChunk, "DNA1", DnaID.Dna, dnaHandle.AddrOfPinnedObject());
        dnaHandle.Free();
    }

    public void WriteHeader(IntPtr buffer)
    {
        byte[] header = Encoding.ASCII.GetBytes("BULLETf_v286");
        if (IntPtr.Size == 8)
        {
            header[7] = (byte)'-';
        }

        if (!BitConverter.IsLittleEndian)
        {
            header[8] = (byte)'V';
        }

        Marshal.Copy(header, 0, buffer, header.Length);
    }

    protected override void Dispose(bool disposing)
    {
        if (_buffer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_buffer);
            _buffer = IntPtr.Zero;
        }

        base.Dispose(disposing);
    }

    [MemberNotNull(nameof(_dna), nameof(_dnaData))]
    private void InitDna()
    {
        _dnaData = IntPtr.Size == 8 ? GetBulletDna64() : GetBulletDna();
        bool swap = !BitConverter.IsLittleEndian;
        using (MemoryStream stream = new MemoryStream(_dnaData))
        {
            using (BulletReader reader = new BulletReader(stream))
            {
                _dna = Dna.Load(reader, swap);
            }
        }
    }

    private IntPtr InternalAlloc(int size)
    {
        IntPtr ptr;
        if (_totalSize != 0)
        {
            ptr = _buffer + _currentSize;
            _currentSize += size;
            Debug.Assert(_currentSize < _totalSize);
        }
        else
        {
            ptr = Marshal.AllocHGlobal(size);
            _currentSize += size;
        }

        return ptr;
    }
}
