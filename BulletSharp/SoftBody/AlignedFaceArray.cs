using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedFaceArrayDebugView
{
    private readonly AlignedFaceArray _array;

    public AlignedFaceArrayDebugView(AlignedFaceArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Face[] Items
    {
        get
        {
            int count = _array.Count;
            Face[] array = new Face[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedFaceArrayEnumerator : IEnumerator<Face>
{
    private readonly int _count;
    private readonly AlignedFaceArray _array;
    private int _i;

    public AlignedFaceArrayEnumerator(AlignedFaceArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Face Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedFaceArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedFaceArray : BulletObject, IList<Face>, IReadOnlyList<Face>
{
    internal AlignedFaceArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_Face_size(Native);

    public bool IsReadOnly => false;

    public Face this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Face(btAlignedObjectArray_btSoftBody_Face_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Face item)
        => throw new NotImplementedException();

    public void Insert(int index, Face item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Face item)
        => btAlignedObjectArray_btSoftBody_Face_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_Face_resizeNoInitialize(Native, 0);

    public bool Contains(Face item)
        => throw new NotImplementedException();

    public void CopyTo(Face[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Face item)
        => throw new NotImplementedException();

    public IEnumerator<Face> GetEnumerator()
        => new AlignedFaceArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedFaceArrayEnumerator(this);
}
