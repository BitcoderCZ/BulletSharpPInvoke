using System;
using System.Collections.Generic;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class NodePtrArrayEnumerator : IEnumerator<Node>
{
    private readonly int _count;
    private readonly IList<Node> _array;
    private int _i;

    public NodePtrArrayEnumerator(IList<Node> array)
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

public class NodePtrArray : FixedSizeArray<Node>, IList<Node>, IReadOnlyList<Node>
{
    internal NodePtrArray(IntPtr native, int count)
        : base(native, count)
    {
    }

    public Node this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Node(btSoftBodyNodePtrArray_at(Native, index));

        set => btSoftBodyNodePtrArray_set(Native, value.Native, index);
    }

    public int IndexOf(Node item)
        => throw new NotImplementedException();

    public bool Contains(Node item)
        => throw new NotImplementedException();

    public void CopyTo(Node[] array, int arrayIndex)
        => throw new NotImplementedException();

    public IEnumerator<Node> GetEnumerator()
        => new NodePtrArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new NodePtrArrayEnumerator(this);
}
