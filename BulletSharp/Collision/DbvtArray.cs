using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class DbvtArrayEnumerator : IEnumerator<Dbvt>
{
    private readonly int _count;
    private readonly IReadOnlyList<Dbvt> _array;
    private int _i;

    public DbvtArrayEnumerator(IReadOnlyList<Dbvt> array)
    {
        _array = array;
        _count = array.Count;
        _i = -1;
    }

    public Dbvt Current => _array[_i];

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

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ListDebugView))]
public class DbvtArray : FixedSizeArray<Dbvt>, IList<Dbvt>, IReadOnlyList<Dbvt>
{
    internal DbvtArray(IntPtr native, int count)
        : base(native, count)
    {
    }

    public Dbvt this[int index]
    {
        get
        {
            if ((uint)index >= (uint)Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            IntPtr ptr = btDbvt_array_at(Native, index);
            return new Dbvt(ptr);
        }

        set => throw new NotSupportedException();
    }

    public int IndexOf(Dbvt item)
        => btDbvt_array_index_of(Native, item != null ? item.Native : IntPtr.Zero, Count);

    public bool Contains(Dbvt item)
        => IndexOf(item) != -1;

    public void CopyTo(Dbvt[] array, int arrayIndex)
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

    public IEnumerator<Dbvt> GetEnumerator()
        => new DbvtArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => new DbvtArrayEnumerator(this);
}
