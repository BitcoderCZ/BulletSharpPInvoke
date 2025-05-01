using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class AlignedIndexedMeshArrayDebugView
{
    private readonly AlignedIndexedMeshArray _array;

    public AlignedIndexedMeshArrayDebugView(AlignedIndexedMeshArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public IndexedMesh[] Items
    {
        get
        {
            IndexedMesh[] array = new IndexedMesh[_array.Count];
            for (int i = 0; i < _array.Count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedIndexedMeshArrayEnumerator : IEnumerator<IndexedMesh>
{
    private readonly int _count;
    private readonly AlignedIndexedMeshArray _array;
    private int _i;

    public AlignedIndexedMeshArrayEnumerator(AlignedIndexedMeshArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public IndexedMesh Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedIndexedMeshArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedIndexedMeshArray : BulletObject, IList<IndexedMesh>, IReadOnlyList<IndexedMesh>
{
    private readonly List<IndexedMesh> _backingList = [];
    private readonly TriangleIndexVertexArray _triangleIndexVertexArray;

    internal AlignedIndexedMeshArray(IntPtr native, TriangleIndexVertexArray triangleIndexVertexArray)
    {
        Initialize(native);
        _triangleIndexVertexArray = triangleIndexVertexArray;

        int count = btAlignedObjectArray_btIndexedMesh_size(Native);
        for (int i = 0; i < count; i++)
        {
            IndexedMesh mesh = new IndexedMesh(btAlignedObjectArray_btIndexedMesh_at(native, i), this);
            _backingList.Add(mesh);
        }
    }

    public int Count => _backingList.Count;

    public bool IsReadOnly => false;

    public IndexedMesh this[int index]
    {
        get => _backingList[index]; set => throw new NotImplementedException();
    }

    public int IndexOf(IndexedMesh item)
        => item == null
        ? -1
        : _backingList.IndexOf(item);

    public void Insert(int index, IndexedMesh item) => throw new NotImplementedException();

    public void RemoveAt(int index) => throw new NotImplementedException();

    public void Add(IndexedMesh item)
    {
        btTriangleIndexVertexArray_addIndexedMesh(_triangleIndexVertexArray.Native, item.Native, item.IndexType);
        _backingList.Add(item);
    }

    public void Clear()
    {
        btAlignedObjectArray_btIndexedMesh_resizeNoInitialize(Native, 0);
        _backingList.Clear();
    }

    public bool Contains(IndexedMesh item) => _backingList.Contains(item);

    public void CopyTo(IndexedMesh[] array, int arrayIndex)
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
            array[arrayIndex + i] = _backingList[i];
        }
    }

    public bool Remove(IndexedMesh item)
        => throw new NotImplementedException();

    public IEnumerator<IndexedMesh> GetEnumerator()
        => _backingList.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => _backingList.GetEnumerator();
}
