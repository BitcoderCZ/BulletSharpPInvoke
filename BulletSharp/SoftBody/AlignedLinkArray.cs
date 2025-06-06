﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedLinkArrayDebugView
{
    private readonly AlignedLinkArray _array;

    public AlignedLinkArrayDebugView(AlignedLinkArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Link[] Items
    {
        get
        {
            int count = _array.Count;
            Link[] array = new Link[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedLinkArrayEnumerator : IEnumerator<Link>
{
    private readonly int _count;
    private readonly AlignedLinkArray _array;
    private int _i;

    public AlignedLinkArrayEnumerator(AlignedLinkArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Link Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedLinkArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedLinkArray : BulletObject, IList<Link>, IReadOnlyList<Link>
{
    internal AlignedLinkArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_Link_size(Native);

    public bool IsReadOnly => false;

    public Link this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Link(btAlignedObjectArray_btSoftBody_Link_at(Native, index));

        set => btAlignedObjectArray_btSoftBody_Link_set(Native, value.Native, index);
    }

    public int IndexOf(Link item)
        => throw new NotImplementedException();

    public void Insert(int index, Link item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Link item)
        => btAlignedObjectArray_btSoftBody_Link_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_Link_resizeNoInitialize(Native, 0);

    public bool Contains(Link item)
        => throw new NotImplementedException();

    public void CopyTo(Link[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException("arrayIndex");
        }

        int count = Count;
        if (array.Length - arrayIndex < count)
        {
            throw new ArgumentException("The number of elements in the source is greater than the available space from arrayIndex to the end of the destination array.");
        }

        for (int i = 0; i < count; i++)
        {
            array.SetValue(new Link(btAlignedObjectArray_btSoftBody_Link_at(Native, i)), i + arrayIndex);
        }
    }

    public bool Remove(Link item)
        => throw new NotImplementedException();

    public IEnumerator<Link> GetEnumerator()
        => new AlignedLinkArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedLinkArrayEnumerator(this);
}
