﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class AlignedBroadphasePairArrayDebugView
{
    private readonly AlignedBroadphasePairArray _array;

    public AlignedBroadphasePairArrayDebugView(AlignedBroadphasePairArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public BroadphasePair[] Items
    {
        get
        {
            BroadphasePair[] array = new BroadphasePair[_array.Count];
            for (int i = 0; i < _array.Count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedBroadphasePairArrayEnumerator : IEnumerator<BroadphasePair>
{
    private readonly int _count;
    private readonly AlignedBroadphasePairArray _array;
    private int _i = -1;

    public AlignedBroadphasePairArrayEnumerator(AlignedBroadphasePairArray array)
    {
        _array = array;
        _count = array.Count;
    }

    public BroadphasePair Current => _array[_i];

    object System.Collections.IEnumerator.Current => _array[_i];

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        _i++;
        return _i != _count;
    }

    public void Reset() => _i = 0;
}

[Serializable]
[DebuggerTypeProxy(typeof(AlignedBroadphasePairArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedBroadphasePairArray : IList<BroadphasePair>, IReadOnlyList<BroadphasePair>
{
    internal IntPtr Native;

    internal AlignedBroadphasePairArray(IntPtr native)
    {
        Native = native;
    }

    public int Count => btAlignedObjectArray_btBroadphasePair_size(Native);

    public bool IsReadOnly => false;

    public BroadphasePair this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new BroadphasePair(btAlignedObjectArray_btBroadphasePair_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(BroadphasePair item) => throw new NotImplementedException();

    public void Insert(int index, BroadphasePair item) => throw new NotImplementedException();

    public void RemoveAt(int index) => throw new NotImplementedException();

    public void Add(BroadphasePair item) => btAlignedObjectArray_btBroadphasePair_push_back(Native, item.Native);

    public void Clear() => btAlignedObjectArray_btBroadphasePair_resizeNoInitialize(Native, 0);

    public bool Contains(BroadphasePair item) => throw new NotImplementedException();

    public void CopyTo(BroadphasePair[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(array));
        }

        int count = Count;
        if (arrayIndex + count > array.Length)
        {
            throw new ArgumentException("Array too small.", nameof(array));
        }

        for (int i = 0; i < count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public bool Remove(BroadphasePair item)
        => throw new NotImplementedException();

    public IEnumerator<BroadphasePair> GetEnumerator()
        => new AlignedBroadphasePairArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedBroadphasePairArrayEnumerator(this);
}
