using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class CompoundShapeChildArrayEnumerator : IEnumerator<CompoundShapeChild>
{
    private readonly int _count;
    private readonly CompoundShapeChild[] _array;
    private int _i;

    public CompoundShapeChildArrayEnumerator(CompoundShapeChildArray array)
    {
        _array = array._backingArray;
        _count = array.Count;
        _i = -1;
    }

    public CompoundShapeChild Current => _array[_i];

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

public class UIntArrayEnumerator : IEnumerator<uint>
{
    private readonly int _count;
    private readonly IReadOnlyList<uint> _array;
    private int _i;

    public UIntArrayEnumerator(IReadOnlyList<uint> array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public uint Current => _array[_i];

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

public class CompoundShapeChildArray : FixedSizeArray<CompoundShapeChild>, IList<CompoundShapeChild>, IReadOnlyList<CompoundShapeChild>
{
    internal CompoundShapeChild[] _backingArray = [];

    internal CompoundShapeChildArray(IntPtr compoundShape)
        : base(compoundShape, 0)
    {
    }

    public CompoundShapeChild this[int index]
    {
        get => _backingArray[index];
        set => throw new NotImplementedException();
    }

    public void AddChildShape(ref Matrix4x4 localTransform, CollisionShape shape)
    {
        IntPtr childListOld = (Count != 0) ? btCompoundShape_getChildList(Native) : IntPtr.Zero;
        btCompoundShape_addChildShape(Native, ref localTransform, shape.Native);
        IntPtr childList = btCompoundShape_getChildList(Native);

        // Adjust the native pointer of existing children if the array was reallocated.
        if (childListOld != childList)
        {
            for (int i = 0; i < Count; i++)
            {
                _backingArray[i].Native = btCompoundShapeChild_array_at(childList, i);
            }
        }

        // Add the child to the backing store.
        int childIndex = Count;
        Count++;
        Array.Resize(ref _backingArray, Count);
        _backingArray[childIndex] = new CompoundShapeChild(btCompoundShapeChild_array_at(childList, childIndex), shape);
    }

    public int IndexOf(CompoundShapeChild item)
        => throw new NotImplementedException();

    public bool Contains(CompoundShapeChild item)
        => throw new NotImplementedException();

    public void CopyTo(CompoundShapeChild[] array, int arrayIndex)
        => throw new NotImplementedException();

    public IEnumerator<CompoundShapeChild> GetEnumerator()
        => new CompoundShapeChildArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new CompoundShapeChildArrayEnumerator(this);

    public void RemoveChildShape(CollisionShape shape)
    {
        IntPtr shapePtr = shape.Native;
        for (int i = 0; i < Count; i++)
        {
            if (_backingArray[i].ChildShape.Native == shapePtr)
            {
                RemoveChildShapeByIndex(i);
            }
        }
    }

    internal void RemoveChildShapeByIndex(int childShapeIndex)
    {
        btCompoundShape_removeChildShapeByIndex(Native, childShapeIndex);
        Count--;

        // Swap the last item with the item to be removed like Bullet does.
        if (childShapeIndex != Count)
        {
            CompoundShapeChild lastItem = _backingArray[Count];
            lastItem.Native = _backingArray[childShapeIndex].Native;
            _backingArray[childShapeIndex] = lastItem;
        }

        _backingArray[Count] = null!;
    }
}

[DebuggerTypeProxy(typeof(ListDebugView))]
public class UIntArray : FixedSizeArray<uint>, IList<uint>, IReadOnlyList<uint>
{
    internal UIntArray(IntPtr native, int count)
        : base(native, count)
    {
    }

    public uint this[int index]
    {
        get => (uint)index >= (uint)Count
                ? throw new ArgumentOutOfRangeException(nameof(index))
                : (uint)Marshal.ReadInt32(Native, index * sizeof(uint));
        set
        {
            if ((uint)index >= (uint)Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Marshal.WriteInt32(Native, index * sizeof(uint), (int)value);
        }
    }

    public int IndexOf(uint item)
        => throw new NotImplementedException();

    public bool Contains(uint item)
        => throw new NotImplementedException();

    public void CopyTo(uint[] array, int arrayIndex)
        => throw new NotImplementedException();

    public IEnumerator<uint> GetEnumerator()
        => new UIntArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new UIntArrayEnumerator(this);
}

internal class ListDebugView
{
    private readonly System.Collections.IEnumerable _list;

    public ListDebugView(System.Collections.IEnumerable list)
    {
        _list = list;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public System.Collections.ArrayList Items
    {
        get
        {
            System.Collections.ArrayList list = [.. _list];

            return list;
        }
    }
}
