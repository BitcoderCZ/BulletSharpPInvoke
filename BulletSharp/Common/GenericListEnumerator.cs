using System.Collections;
using System.Collections.Generic;

namespace BulletSharp;

public sealed class GenericListEnumerator<T> : IEnumerator<T>
{
    private readonly int _count;
    private readonly IReadOnlyList<T> _list;
    private int _i;

    public GenericListEnumerator(IReadOnlyList<T> list)
    {
        _list = list;
        _count = list.Count;
        _i = -1;
    }

    public T Current => _list[_i];

    object? IEnumerator.Current => _list[_i];

    public bool MoveNext()
    {
        _i++;
        return _i != _count;
    }

    public void Reset()
        => _i = 0;

    public void Dispose()
    {
    }
}
