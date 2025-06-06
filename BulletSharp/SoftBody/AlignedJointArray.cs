﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedJointArrayDebugView
{
    private readonly AlignedJointArray _array;

    public AlignedJointArrayDebugView(AlignedJointArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Joint[] Items
    {
        get
        {
            int count = _array.Count;
            Joint[] array = new Joint[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedJointArrayEnumerator : IEnumerator<Joint>
{
    private readonly int _count;
    private readonly AlignedJointArray _array;
    private int _i;

    public AlignedJointArrayEnumerator(AlignedJointArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Joint Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedJointArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedJointArray : BulletObject, IList<Joint>, IReadOnlyList<Joint>
{
    internal AlignedJointArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_JointPtr_size(Native);

    public bool IsReadOnly => false;

    public Joint this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : Joint.GetManaged(btAlignedObjectArray_btSoftBody_JointPtr_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Joint item)
        => throw new NotImplementedException();

    public void Insert(int index, Joint item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Joint item)
        => btAlignedObjectArray_btSoftBody_JointPtr_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_JointPtr_resizeNoInitialize(Native, 0);

    public bool Contains(Joint item)
        => throw new NotImplementedException();

    public void CopyTo(Joint[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Joint item)
        => throw new NotImplementedException();

    public IEnumerator<Joint> GetEnumerator()
        => new AlignedJointArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedJointArrayEnumerator(this);
}
