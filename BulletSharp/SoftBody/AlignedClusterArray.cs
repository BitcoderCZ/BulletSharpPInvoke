using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedClusterArrayDebugView
{
    private readonly AlignedClusterArray _array;

    public AlignedClusterArrayDebugView(AlignedClusterArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Cluster[] Items
    {
        get
        {
            int count = _array.Count;
            Cluster[] array = new Cluster[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedClusterArrayEnumerator : IEnumerator<Cluster>
{
    private readonly int _count;
    private readonly AlignedClusterArray _array;
    private int _i;

    public AlignedClusterArrayEnumerator(AlignedClusterArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Cluster Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedClusterArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedClusterArray : BulletObject, IList<Cluster>, IReadOnlyList<Cluster>
{
    internal AlignedClusterArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_ClusterPtr_size(Native);

    public bool IsReadOnly => false;

    public Cluster this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Cluster(btAlignedObjectArray_btSoftBody_ClusterPtr_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Cluster item)
        => throw new NotImplementedException();

    public void Insert(int index, Cluster item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Cluster item)
        => btAlignedObjectArray_btSoftBody_ClusterPtr_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_ClusterPtr_resizeNoInitialize(Native, 0);

    public bool Contains(Cluster item)
        => throw new NotImplementedException();

    public void CopyTo(Cluster[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Cluster item)
        => throw new NotImplementedException();

    public IEnumerator<Cluster> GetEnumerator()
        => new AlignedClusterArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedClusterArrayEnumerator(this);
}
