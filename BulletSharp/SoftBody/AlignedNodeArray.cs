using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedNodeArrayDebugView
{
    private readonly AlignedNodeArray _array;

    public AlignedNodeArrayDebugView(AlignedNodeArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Node[] Items
    {
        get
        {
            int count = _array.Count;
            Node[] array = new Node[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedNodeArrayEnumerator : IEnumerator<Node>
{
    private readonly int _count;
    private readonly AlignedNodeArray _array;
    private int _i;

    public AlignedNodeArrayEnumerator(AlignedNodeArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Node Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedNodeArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedNodeArray : BulletObject, IList<Node>, IReadOnlyList<Node>
{
    internal AlignedNodeArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_Node_size(Native);

    public bool IsReadOnly => false;

    public Node this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Node(btAlignedObjectArray_btSoftBody_Node_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Node item)
        => btAlignedObjectArray_btSoftBody_Node_index_of(Native, item.Native);

    public void Insert(int index, Node item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Node item)
        => btAlignedObjectArray_btSoftBody_Node_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_Node_resizeNoInitialize(Native, 0);

    public bool Contains(Node item)
        => throw new NotImplementedException();

    public void CopyTo(Node[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Node item)
        => throw new NotImplementedException();

    public IEnumerator<Node> GetEnumerator()
        => new AlignedNodeArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedNodeArrayEnumerator(this);
}
