using System;
using System.Diagnostics;
using System.Numerics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class CollisionObjectWrapper : BulletObject
{
    internal CollisionObjectWrapper(IntPtr native)
    {
        Initialize(native);
    }

    public CollisionObject CollisionObject
    {
        get
        {
            CollisionObject? managed = CollisionObject.GetManaged(btCollisionObjectWrapper_getCollisionObject(Native));

            Debug.Assert(managed is not null, $"{nameof(managed)} should not be null.");

            return managed;
        }

        set => btCollisionObjectWrapper_setCollisionObject(Native, value.Native);
    }

    public CollisionShape CollisionShape
    {
        get
        {
            CollisionShape? managed = CollisionShape.GetManaged(btCollisionObjectWrapper_getCollisionShape(Native));

            Debug.Assert(managed is not null, $"{nameof(managed)} should not be null.");

            return managed;
        }

        set => btCollisionObjectWrapper_setShape(Native, value.Native);
    }

    public int Index
    {
        get => btCollisionObjectWrapper_getIndex(Native);
        set => btCollisionObjectWrapper_setIndex(Native, value);
    }

    public CollisionObjectWrapper Parent
    {
        get => new CollisionObjectWrapper(btCollisionObjectWrapper_getParent(Native));
        set => btCollisionObjectWrapper_setParent(Native, value.Native);
    }

    public int PartId
    {
        get => btCollisionObjectWrapper_getPartId(Native);
        set => btCollisionObjectWrapper_setPartId(Native, value);
    }

    public Matrix4x4 WorldTransform
    {
        get
        {
            Matrix4x4 value;
            btCollisionObjectWrapper_getWorldTransform(Native, out value);
            return value;
        }
    }
}
