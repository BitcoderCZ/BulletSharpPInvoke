﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp.SoftBody;

public class AlignedMaterialArrayDebugView
{
    private readonly AlignedMaterialArray _array;

    public AlignedMaterialArrayDebugView(AlignedMaterialArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Material[] Items
    {
        get
        {
            int count = _array.Count;
            Material[] array = new Material[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedMaterialArrayEnumerator : IEnumerator<Material>
{
    private readonly int _count;
    private readonly AlignedMaterialArray _array;
    private int _i;

    public AlignedMaterialArrayEnumerator(AlignedMaterialArray array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Material Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedMaterialArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedMaterialArray : BulletObject, IList<Material>, IReadOnlyList<Material>
{
    internal AlignedMaterialArray(IntPtr native)
    {
        Initialize(native);
    }

    public int Count => btAlignedObjectArray_btSoftBody_MaterialPtr_size(Native);

    public bool IsReadOnly => false;

    public Material this[int index]
    {
        get => (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : new Material(btAlignedObjectArray_btSoftBody_MaterialPtr_at(Native, index));

        set => throw new NotImplementedException();
    }

    public int IndexOf(Material item)
        => throw new NotImplementedException();

    public void Insert(int index, Material item)
        => throw new NotImplementedException();

    public void RemoveAt(int index)
        => throw new NotImplementedException();

    public void Add(Material item)
        => btAlignedObjectArray_btSoftBody_MaterialPtr_push_back(Native, item.Native);

    public void Clear()
        => btAlignedObjectArray_btSoftBody_MaterialPtr_resizeNoInitialize(Native, 0);

    public bool Contains(Material item)
        => throw new NotImplementedException();

    public void CopyTo(Material[] array, int arrayIndex)
        => throw new NotImplementedException();

    public bool Remove(Material item)
        => throw new NotImplementedException();

    public IEnumerator<Material> GetEnumerator()
        => new AlignedMaterialArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new AlignedMaterialArrayEnumerator(this);
}
