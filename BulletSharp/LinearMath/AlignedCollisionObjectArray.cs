﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class AlignedCollisionObjectArrayDebugView
{
    private readonly AlignedCollisionObjectArray _array;

    public AlignedCollisionObjectArrayDebugView(AlignedCollisionObjectArray array)
    {
        _array = array;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public CollisionObject[] Items
    {
        get
        {
            CollisionObject[] array = new CollisionObject[_array.Count];
            for (int i = 0; i < _array.Count; i++)
            {
                array[i] = _array[i];
            }

            return array;
        }
    }
}

public class AlignedCollisionObjectArrayEnumerator : IEnumerator<CollisionObject>
{
    private readonly int _count;
    private readonly IReadOnlyList<CollisionObject> _array;
    private int _i = -1;

    public AlignedCollisionObjectArrayEnumerator(IReadOnlyList<CollisionObject> array)
    {
        _array = array;
        _count = array.Count;
    }

    public CollisionObject Current => _array[_i];

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
[DebuggerTypeProxy(typeof(AlignedCollisionObjectArrayDebugView))]
[DebuggerDisplay("Count = {Count}")]
public class AlignedCollisionObjectArray : BulletObject, IList<CollisionObject>, IReadOnlyList<CollisionObject>
{
    private readonly CollisionWorld? _collisionWorld;
    private readonly List<CollisionObject>? _backingList;

    internal AlignedCollisionObjectArray(IntPtr native)
    {
        Initialize(native);
    }

    internal AlignedCollisionObjectArray(IntPtr native, CollisionWorld collisionWorld)
    {
        Initialize(native);
        if (collisionWorld != null)
        {
            _collisionWorld = collisionWorld;
            _backingList = [];
        }
    }

    public int Count => btAlignedObjectArray_btCollisionObjectPtr_size(Native);

    public bool IsReadOnly => false;

    public CollisionObject this[int index]
    {
        get => _backingList != null
              ? _backingList[index]
              : (uint)index >= (uint)Count
              ? throw new ArgumentOutOfRangeException(nameof(index))
              : CollisionObject.GetManaged(btAlignedObjectArray_btCollisionObjectPtr_at(Native, index))!;

        set => throw new NotImplementedException();
    }

    public void Add(CollisionObject item)
    {
        if (_collisionWorld != null)
        {
            if (item is RigidBody)
            {
                if (item.CollisionShape == null)
                {
                    return;
                }

                btDynamicsWorld_addRigidBody(_collisionWorld.Native, item.Native);
            }
            else if (item is SoftBody.SoftBody)
            {
                btSoftRigidDynamicsWorld_addSoftBody(_collisionWorld.Native, item.Native);
            }
            else
            {
                btCollisionWorld_addCollisionObject(_collisionWorld.Native, item.Native);
            }

            SetBodyBroadphaseHandle(item, _collisionWorld.Broadphase);
            Debug.Assert(_backingList is not null, $"{nameof(_backingList)} should not be null.");
            _backingList.Add(item);
        }
        else
        {
            btAlignedObjectArray_btCollisionObjectPtr_push_back(Native, item.Native);
        }
    }

    public void Clear()
    {
        for (int i = Count - 1; i >= 0; i--)
        {
            RemoveAt(i);
        }
    }

    public bool Contains(CollisionObject item)
        => IndexOf(item) != -1;

    public void CopyTo(CollisionObject[] array, int arrayIndex)
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

    public int IndexOf(CollisionObject item)
        => item == null
        ? -1
        : btAlignedObjectArray_btCollisionObjectPtr_findLinearSearch2(Native, item.Native);

    public void Insert(int index, CollisionObject item)
        => throw new NotImplementedException();

    public bool Remove(CollisionObject item)
    {
        int index = IndexOf(item);
        if (index == -1)
        {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (_backingList == null)
        {
            throw new NotImplementedException();
            //btAlignedObjectArray_btCollisionObjectPtr_remove(itemPtr);
        }

        CollisionObject item = this[index];
        IntPtr itemPtr = item.Native;

        Debug.Assert(_collisionWorld is not null, $"{nameof(_collisionWorld)} should not be null.");
        if (item is SoftBody.SoftBody)
        {
            btSoftRigidDynamicsWorld_removeSoftBody(_collisionWorld.Native, itemPtr);
        }
        else if (item is RigidBody)
        {
            btDynamicsWorld_removeRigidBody(_collisionWorld.Native, itemPtr);
        }
        else
        {
            btCollisionWorld_removeCollisionObject(_collisionWorld.Native, itemPtr);
        }

        item.BroadphaseHandle = null;

        // Swap the last item with the item to be removed like Bullet does.
        int count = _backingList.Count - 1;
        if (index != count)
        {
            _backingList[index] = _backingList[count];
        }

        _backingList.RemoveAt(count);
    }

    public IEnumerator<CollisionObject> GetEnumerator()
        => _backingList != null
        ? _backingList.GetEnumerator()
        : new AlignedCollisionObjectArrayEnumerator(this);

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        => _backingList != null
        ? _backingList.GetEnumerator()
        : new AlignedCollisionObjectArrayEnumerator(this);

    internal void Add(CollisionObject item, int group, int mask)
    {
        if (item is RigidBody)
        {
            if (item.CollisionShape == null)
            {
                return;
            }

            Debug.Assert(_collisionWorld is not null, $"{nameof(_collisionWorld)} should not be null.");
            btDynamicsWorld_addRigidBody2(_collisionWorld.Native, item.Native, group, mask);
        }
        else if (item is SoftBody.SoftBody)
        {
            if (_collisionWorld is SoftBody.DeformableMultiBodyDynamicsWorld)
            {
                btDeformableMultiBodyDynamicsWorld_addSoftBody(_collisionWorld.Native, item.Native, group, mask);
            }
            else
            {
                Debug.Assert(_collisionWorld is not null, $"{nameof(_collisionWorld)} should not be null.");
                btSoftRigidDynamicsWorld_addSoftBody3(_collisionWorld.Native, item.Native, group, mask);
            }
        }
        else
        {
            Debug.Assert(_collisionWorld is not null, $"{nameof(_collisionWorld)} should not be null.");
            btCollisionWorld_addCollisionObject3(_collisionWorld.Native, item.Native, group, mask);
        }

        SetBodyBroadphaseHandle(item, _collisionWorld.Broadphase);
        Debug.Assert(_backingList is not null, $"{nameof(_backingList)} should not be null.");
        _backingList.Add(item);
    }

    private void SetBodyBroadphaseHandle(CollisionObject item, BroadphaseInterface broadphase)
    {
        IntPtr broadphaseHandle = btCollisionObject_getBroadphaseHandle(item.Native);
        if (broadphase is DbvtBroadphase)
        {
            item.BroadphaseHandle = new DbvtProxy(broadphaseHandle);
        }

        // TODO: implement AxisSweep3::Handle
        /*
			else if (broadphase is AxisSweep3)
			{
				item.BroadphaseHandle = new AxisSweep3::Handle(broadphaseHandle);
			}
			else if (broadphase is AxisSweep3_32Bit)
			{
				item.BroadphaseHandle = new AxisSweep3_32Bit::Handle(broadphaseHandle);
			}
			*/
        else
        {
            item.BroadphaseHandle = new BroadphaseProxy(broadphaseHandle);
        }
    }
}
