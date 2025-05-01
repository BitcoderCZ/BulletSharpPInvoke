using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class PoolAllocator : BulletDisposableObject
{
    public PoolAllocator(int elemSize, int maxElements)
    {
        IntPtr native = btPoolAllocator_new(elemSize, maxElements);
        InitializeUserOwned(native);
    }

    internal PoolAllocator(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public int ElementSize => btPoolAllocator_getElementSize(Native);

    public int FreeCount => btPoolAllocator_getFreeCount(Native);

    public int MaxCount => btPoolAllocator_getMaxCount(Native);

    public IntPtr PoolAddress => btPoolAllocator_getPoolAddress(Native);

    public int UsedCount => btPoolAllocator_getUsedCount(Native);

    public IntPtr Allocate(int size)
        => btPoolAllocator_allocate(Native, size);

    public void FreeMemory(IntPtr ptr)
        => btPoolAllocator_freeMemory(Native, ptr);

    public bool ValidPtr(IntPtr ptr)
        => btPoolAllocator_validPtr(Native, ptr);

    protected override void Dispose(bool disposing)
    {
        if (IsUserOwned)
        {
            btPoolAllocator_delete(Native);
        }
    }
}
