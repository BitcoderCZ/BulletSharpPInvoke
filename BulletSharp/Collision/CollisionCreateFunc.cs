using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class CollisionAlgorithmCreateFunc : BulletDisposableObject
{
    public CollisionAlgorithmCreateFunc()
    {
        IntPtr native = btCollisionAlgorithmCreateFunc_new();
        InitializeUserOwned(native);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    internal CollisionAlgorithmCreateFunc(ConstructionInfo? info)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    public bool Swapped
    {
        get => btCollisionAlgorithmCreateFunc_getSwapped(Native);
        set => btCollisionAlgorithmCreateFunc_setSwapped(Native, value);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsUserOwned)
        {
            btCollisionAlgorithmCreateFunc_delete(Native);
        }
    }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public virtual CollisionAlgorithm? CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        => null;
}
