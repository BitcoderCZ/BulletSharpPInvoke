using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public abstract class CollisionConfiguration : BulletDisposableObject
{
    protected internal CollisionConfiguration()
    {
    }

    public abstract PoolAllocator CollisionAlgorithmPool { get; }

    public abstract PoolAllocator PersistentManifoldPool { get; }

    public abstract CollisionAlgorithmCreateFunc GetCollisionAlgorithmCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1);

    public abstract CollisionAlgorithmCreateFunc GetClosestPointsAlgorithmCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1);

    protected override void Dispose(bool disposing)
        => btCollisionConfiguration_delete(Native);
}
