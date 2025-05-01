using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public sealed class SoftBodyRigidBodyCollisionConfiguration : DefaultCollisionConfiguration
{
    public SoftBodyRigidBodyCollisionConfiguration()
        : base(ConstructionInfo.Null)
    {
        IntPtr native = btSoftBodyRigidBodyCollisionConfiguration_new();
        InitializeUserOwned(native);
    }

    public SoftBodyRigidBodyCollisionConfiguration(DefaultCollisionConstructionInfo constructionInfo)
        : base(ConstructionInfo.Null)
    {
        if (constructionInfo == null)
        {
            throw new ArgumentNullException(nameof(constructionInfo));
        }

        IntPtr native = btSoftBodyRigidBodyCollisionConfiguration_new2(constructionInfo.Native);
        InitializeUserOwned(native);

        _collisionAlgorithmPool = constructionInfo.CollisionAlgorithmPool;
        _persistentManifoldPool = constructionInfo.PersistentManifoldPool;
    }

    public override CollisionAlgorithmCreateFunc GetCollisionAlgorithmCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1)
    {
        IntPtr createFunc = btCollisionConfiguration_getCollisionAlgorithmCreateFunc(Native, (int)proxyType0, (int)proxyType1);
        return proxyType0 == BroadphaseNativeType.SoftBodyShape && proxyType1 == BroadphaseNativeType.SoftBodyShape
            ? new SoftSoftCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SoftBodyShape && BroadphaseProxy.IsConvex(proxyType1)
            ? new SoftRigidCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType0) && proxyType1 == BroadphaseNativeType.SoftBodyShape
            ? new SoftRigidCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SoftBodyShape && BroadphaseProxy.IsConcave(proxyType1)
            ? new SoftBodyConcaveCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConcave(proxyType0) && proxyType1 == BroadphaseNativeType.SoftBodyShape
            ? new SoftBodyConcaveCollisionAlgorithm.SwappedCreateFunc(createFunc, this)
            : base.GetCollisionAlgorithmCreateFunc(proxyType0, proxyType1);
    }
}
