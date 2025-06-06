﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedSoftBodyArrayDebugView
{
    private readonly AlignedSoftBodyArray _array;

    public AlignedSoftBodyArrayDebugView(AlignedSoftBodyArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public SoftBody[] Items
    {
        get
        {
            int count = _array.Count;
            SoftBody[] array = new SoftBody[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedSoftBodyArrayEnumerator : IEnumerator<SoftBody>
{
    private readonly int _count;
    private readonly AlignedSoftBodyArray _array;
    private int _i;

    public AlignedSoftBodyArrayEnumerator(AlignedSoftBodyArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public SoftBody Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedSoftBodyArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedSoftBodyArray : BulletObject, IList<SoftBody>, IReadOnlyList<SoftBody>
{
    internal AlignedSoftBodyArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBodyPtr_size(Native);

    public bool IsReadOnly => false;

    public SoftBody this[int index]
    {
        // TODO: should this be nullable?
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : CollisionObject.GetManaged(btAlignedObjectArray_btSoftBodyPtr_at(Native, index)) as SoftBody;

        set => throw new NotImplementedException();
    }

    public int IndexOf(SoftBody item)
        => throw new NotImplementedException();

    public void Insert(int index, SoftBody item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(SoftBody item)
        => btAlignedObjectArray_btSoftBodyPtr_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBodyPtr_resizeNoInitialize(Native, 0);

    public bool Contains(SoftBody item)
        => throw new NotImplementedException();

    public void CopyTo(SoftBody[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(SoftBody item)
        => throw new NotImplementedException();

    public IEnumerator<SoftBody> GetEnumerator()
        => new AlignedSoftBodyArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedSoftBodyArrayEnumerator(this);
}
