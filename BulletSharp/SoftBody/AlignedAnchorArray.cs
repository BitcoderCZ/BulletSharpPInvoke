﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedAnchorArrayDebugView
{
    private readonly AlignedAnchorArray _array;

    public AlignedAnchorArrayDebugView(AlignedAnchorArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Anchor[] Items
    {
        get
        {
            int count = _array.Count;
            Anchor[] array = new Anchor[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedAnchorArrayEnumerator : IEnumerator<Anchor>
{
    private readonly int _count;
    private readonly AlignedAnchorArray _array;
    private int _i;

    public AlignedAnchorArrayEnumerator(AlignedAnchorArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Anchor Current => _array[_i];

    object System.Collections.IEnumerator.Current => _array[_i];

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        _i++;
        return _i != _count;
    }

    public void Reset()
        => _i = 0;
}

[Serializable]
[DebuggerTypeProxy(typeof(AlignedAnchorArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedAnchorArray : BulletObject, IList<Anchor>, IReadOnlyList<Anchor>
{
    internal AlignedAnchorArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_Anchor_size(Native);

    public bool IsReadOnly => false;

    public Anchor this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Anchor(btAlignedObjectArray_btSoftBody_Anchor_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Anchor item)
        => throw new NotImplementedException();

    public void Insert(int index, Anchor item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Anchor item)
        => btAlignedObjectArray_btSoftBody_Anchor_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_Anchor_resizeNoInitialize(Native, 0);

    public bool Contains(Anchor item)
        => throw new NotImplementedException();

    public void CopyTo(Anchor[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Anchor item)
        => throw new NotImplementedException();

    public IEnumerator<Anchor> GetEnumerator()
        => new AlignedAnchorArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedAnchorArrayEnumerator(this);
}
