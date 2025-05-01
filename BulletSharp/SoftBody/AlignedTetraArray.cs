using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedTetraArrayDebugView
{
    private readonly AlignedTetraArray _array;

    public AlignedTetraArrayDebugView(AlignedTetraArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Tetra[] Items
    {
        get
        {
            int count = _array.Count;
            Tetra[] array = new Tetra[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedTetraArrayEnumerator : IEnumerator<Tetra>
{
    private readonly int _count;
    private readonly AlignedTetraArray _array;
    private int _i;

    public AlignedTetraArrayEnumerator(AlignedTetraArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Tetra Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedTetraArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedTetraArray : BulletObject, IList<Tetra>, IReadOnlyList<Tetra>
{
    internal AlignedTetraArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_Tetra_size(Native);

    public bool IsReadOnly => false;

    public Tetra this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Tetra(btAlignedObjectArray_btSoftBody_Tetra_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Tetra item)
        => throw new NotImplementedException();

    public void Insert(int index, Tetra item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Tetra item)
        => btAlignedObjectArray_btSoftBody_Tetra_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_Tetra_resizeNoInitialize(Native, 0);

    public bool Contains(Tetra item)
        => throw new NotImplementedException();

    public void CopyTo(Tetra[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Tetra item)
        => throw new NotImplementedException();

    public IEnumerator<Tetra> GetEnumerator()
        => new AlignedTetraArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedTetraArrayEnumerator(this);
}
