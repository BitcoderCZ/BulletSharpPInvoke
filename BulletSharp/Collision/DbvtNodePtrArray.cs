using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class DbvtNodePtrArrayEnumerator : IEnumerator<DbvtNode?>
{
    private readonly int _count;
    private readonly IReadOnlyList<DbvtNode?> _array;
    private int _i;

    public DbvtNodePtrArrayEnumerator(IReadOnlyList<DbvtNode?> array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public DbvtNode? Current => _array[_i];

    object? System.Collections.IEnumerator.Current => _array[_i];

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

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ListDebugView))]
public class DbvtNodePtrArray : FixedSizeArray<DbvtNode?>, IList<DbvtNode?>, IReadOnlyList<DbvtNode?>
{
    internal DbvtNodePtrArray(IntPtr native, int count)
        : base(native, count)
    {
    }

    public DbvtNode? this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            IntPtr ptr = btDbvtNodePtr_array_at(Native, index);
            return (ptr != IntPtr.Zero) ? new DbvtNode(ptr) : null;
        }

        set => throw new NotSupportedException();
    }

    public int IndexOf(DbvtNode? item)
        => btDbvtNodePtr_array_index_of(Native, item != null ? item.Native : IntPtr.Zero, Count);

    public bool Contains(DbvtNode? item)
        => IndexOf(item) != -1;

    public void CopyTo(DbvtNode?[] array, int arrayIndex)
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
            throw new ArgumentException("Array too small.", "array");
        }

        for (int i = 0; i < count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public IEnumerator<DbvtNode?> GetEnumerator()
        => new DbvtNodePtrArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new DbvtNodePtrArrayEnumerator(this);
}
