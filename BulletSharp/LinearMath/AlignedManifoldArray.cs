﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class AlignedManifoldArrayDebugView
{
    private readonly AlignedManifoldArray _array;

    public AlignedManifoldArrayDebugView(AlignedManifoldArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public PersistentManifold[] Items
    {
        get
        {
            PersistentManifold[] array = new PersistentManifold[_array.Count];
            for (int i = 0; i < _array.Count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedManifoldArrayEnumerator : IEnumerator<PersistentManifold>
{
    private readonly int _count;
    private readonly AlignedManifoldArray _array;
    private int _i;

    public AlignedManifoldArrayEnumerator(AlignedManifoldArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public PersistentManifold Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedManifoldArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedManifoldArray : BulletDisposableObject, IList<PersistentManifold>, IReadOnlyList<PersistentManifold>
{
    public AlignedManifoldArray()
    {
        IntPtr native = btAlignedObjectArray_btPersistentManifoldPtr_new();
        InitializeUserOwned(native);
    }

    public int Count => btAlignedObjectArray_btPersistentManifoldPtr_size(Native);

    public bool IsReadOnly => false;

    public PersistentManifold this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new PersistentManifold(btAlignedObjectArray_btPersistentManifoldPtr_at(Native, index), this);

        set => throw new NotImplementedException();
    }

    public int IndexOf(PersistentManifold item)
        => throw new NotImplementedException();

    public void Insert(int index, PersistentManifold item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(PersistentManifold item) => btAlignedObjectArray_btPersistentManifoldPtr_push_back(Native, item.Native);

    public void Clear() => btAlignedObjectArray_btPersistentManifoldPtr_resizeNoInitialize(Native, 0);

    public bool Contains(PersistentManifold item) => throw new NotImplementedException();

    public void CopyTo(PersistentManifold[] array, int arrayIndex)
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

    public bool Remove(PersistentManifold item)
        => throw new NotImplementedException();

    public IEnumerator<PersistentManifold> GetEnumerator()
        => new AlignedManifoldArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedManifoldArrayEnumerator(this);

    protected override void Dispose(bool disposing)
        => btAlignedObjectArray_btPersistentManifoldPtr_delete(Native);
}
