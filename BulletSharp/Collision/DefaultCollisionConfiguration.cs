using System;
using System.Diagnostics.CodeAnalysis;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class DefaultCollisionConstructionInfo : BulletDisposableObject
{
    private PoolAllocator? _collisionAlgorithmPool;
    private PoolAllocator? _persistentManifoldPool;

    public DefaultCollisionConstructionInfo()
    {
        IntPtr native = btDefaultCollisionConstructionInfo_new();
        InitializeUserOwned(native);
    }

    [DisallowNull]
    public PoolAllocator? CollisionAlgorithmPool
    {
        get => _collisionAlgorithmPool;
        set
        {
            btDefaultCollisionConstructionInfo_setCollisionAlgorithmPool(Native, value.Native);
            _collisionAlgorithmPool = value;
        }
    }

    public int CustomCollisionAlgorithmMaxElementSize
    {
        get => btDefaultCollisionConstructionInfo_getCustomCollisionAlgorithmMaxElementSize(Native);
        set => btDefaultCollisionConstructionInfo_setCustomCollisionAlgorithmMaxElementSize(Native, value);
    }

    public int DefaultMaxCollisionAlgorithmPoolSize
    {
        get => btDefaultCollisionConstructionInfo_getDefaultMaxCollisionAlgorithmPoolSize(Native);
        set => btDefaultCollisionConstructionInfo_setDefaultMaxCollisionAlgorithmPoolSize(Native, value);
    }

    public int DefaultMaxPersistentManifoldPoolSize
    {
        get => btDefaultCollisionConstructionInfo_getDefaultMaxPersistentManifoldPoolSize(Native);
        set => btDefaultCollisionConstructionInfo_setDefaultMaxPersistentManifoldPoolSize(Native, value);
    }

    [DisallowNull]
    public PoolAllocator? PersistentManifoldPool
    {
        get => _persistentManifoldPool;
        set
        {
            btDefaultCollisionConstructionInfo_setPersistentManifoldPool(Native, value.Native);
            _persistentManifoldPool = value;
        }
    }

    public int UseEpaPenetrationAlgorithm
    {
        get => btDefaultCollisionConstructionInfo_getUseEpaPenetrationAlgorithm(Native);
        set => btDefaultCollisionConstructionInfo_setUseEpaPenetrationAlgorithm(Native, value);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed == false)
        {
            btDefaultCollisionConstructionInfo_delete(Native);
        }
    }
}

public class DefaultCollisionConfiguration : CollisionConfiguration
{
    protected PoolAllocator? _collisionAlgorithmPool;
    protected PoolAllocator? _persistentManifoldPool;

    public DefaultCollisionConfiguration()
    {
        IntPtr native = btDefaultCollisionConfiguration_new();
        InitializeUserOwned(native);
    }

    public DefaultCollisionConfiguration(DefaultCollisionConstructionInfo constructionInfo)
    {
        if (constructionInfo == null)
        {
            throw new ArgumentNullException(nameof(constructionInfo));
        }

        _collisionAlgorithmPool = constructionInfo.CollisionAlgorithmPool;
        _persistentManifoldPool = constructionInfo.PersistentManifoldPool;

        IntPtr native = btDefaultCollisionConfiguration_new2(constructionInfo.Native);
        InitializeUserOwned(native);
    }

#pragma warning disable IDE0060
    internal DefaultCollisionConfiguration(ConstructionInfo info)
#pragma warning restore IDE0060
    {
    }

    public override PoolAllocator CollisionAlgorithmPool
    {
        get
        {
            if (_collisionAlgorithmPool == null)
            {
                _collisionAlgorithmPool = new PoolAllocator(btCollisionConfiguration_getCollisionAlgorithmPool(Native), this);
            }

            return _collisionAlgorithmPool;
        }
    }

    public override PoolAllocator PersistentManifoldPool
    {
        get
        {
            if (_persistentManifoldPool == null)
            {
                _persistentManifoldPool = new PoolAllocator(btCollisionConfiguration_getPersistentManifoldPool(Native), this);
            }

            return _persistentManifoldPool;
        }
    }

    public override CollisionAlgorithmCreateFunc GetClosestPointsAlgorithmCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1)
    {
        IntPtr createFunc = btCollisionConfiguration_getClosestPointsAlgorithmCreateFunc(Native, (int)proxyType0, (int)proxyType1);

        return proxyType0 == BroadphaseNativeType.BoxShape && proxyType1 == BroadphaseNativeType.BoxShape
            ? new BoxBoxCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SphereShape && proxyType1 == BroadphaseNativeType.SphereShape
            ? new SphereSphereCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SphereShape && proxyType1 == BroadphaseNativeType.TriangleShape
            ? new SphereTriangleCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.TriangleShape && proxyType1 == BroadphaseNativeType.SphereShape
            ? new SphereTriangleCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.StaticPlaneShape && BroadphaseProxy.IsConvex(proxyType1)
            ? new ConvexPlaneCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType1 == BroadphaseNativeType.StaticPlaneShape && BroadphaseProxy.IsConvex(proxyType0)
            ? new ConvexPlaneCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConvex(proxyType1)
            ? new ConvexConvexAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConcave(proxyType1)
            ? new ConvexConcaveCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType1) && BroadphaseProxy.IsConcave(proxyType0)
            ? new ConvexConcaveCollisionAlgorithm.SwappedCreateFunc(createFunc, this)
            : BroadphaseProxy.IsCompound(proxyType0)
            ? new CompoundCompoundCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsCompound(proxyType1)
            ? new CompoundCompoundCollisionAlgorithm.SwappedCreateFunc(createFunc, this)
            : new EmptyAlgorithm.CreateFunc(createFunc, this);
    }

    public override CollisionAlgorithmCreateFunc GetCollisionAlgorithmCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1)
    {
        IntPtr createFunc = btCollisionConfiguration_getCollisionAlgorithmCreateFunc(Native, (int)proxyType0, (int)proxyType1);

        return proxyType0 == BroadphaseNativeType.BoxShape && proxyType1 == BroadphaseNativeType.BoxShape
            ? new BoxBoxCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SphereShape && proxyType1 == BroadphaseNativeType.SphereShape
            ? new SphereSphereCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.SphereShape && proxyType1 == BroadphaseNativeType.TriangleShape
            ? new SphereTriangleCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.TriangleShape && proxyType1 == BroadphaseNativeType.SphereShape
            ? new SphereTriangleCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType0 == BroadphaseNativeType.StaticPlaneShape && BroadphaseProxy.IsConvex(proxyType1)
            ? new ConvexPlaneCollisionAlgorithm.CreateFunc(createFunc, this)
            : proxyType1 == BroadphaseNativeType.StaticPlaneShape && BroadphaseProxy.IsConvex(proxyType0)
            ? new ConvexPlaneCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConvex(proxyType1)
            ? new ConvexConvexAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType0) && BroadphaseProxy.IsConcave(proxyType1)
            ? new ConvexConcaveCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsConvex(proxyType1) && BroadphaseProxy.IsConcave(proxyType0)
            ? new ConvexConcaveCollisionAlgorithm.SwappedCreateFunc(createFunc, this)
            : BroadphaseProxy.IsCompound(proxyType0)
            ? new CompoundCompoundCollisionAlgorithm.CreateFunc(createFunc, this)
            : BroadphaseProxy.IsCompound(proxyType1)
            ? new CompoundCompoundCollisionAlgorithm.SwappedCreateFunc(createFunc, this)
            : new EmptyAlgorithm.CreateFunc(createFunc, this);
    }

    public void SetConvexConvexMultipointIterations(int numPerturbationIterations = 3, int minimumPointsPerturbationThreshold = 3)
        => btDefaultCollisionConfiguration_setConvexConvexMultipointIterations(Native, numPerturbationIterations, minimumPointsPerturbationThreshold);

    public void SetPlaneConvexMultipointIterations(int numPerturbationIterations = 3, int minimumPointsPerturbationThreshold = 3)
        => btDefaultCollisionConfiguration_setPlaneConvexMultipointIterations(Native, numPerturbationIterations, minimumPointsPerturbationThreshold);
}
