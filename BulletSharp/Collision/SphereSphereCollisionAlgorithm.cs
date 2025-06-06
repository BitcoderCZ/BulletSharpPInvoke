using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class SphereSphereCollisionAlgorithm : ActivatingCollisionAlgorithm
{
    public SphereSphereCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper col0Wrap, CollisionObjectWrapper col1Wrap)
    {
        IntPtr native = btSphereSphereCollisionAlgorithm_new(mf.Native, ci.Native, col0Wrap.Native, col1Wrap.Native);
        InitializeUserOwned(native);
    }

    public SphereSphereCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
    {
        IntPtr native = btSphereSphereCollisionAlgorithm_new2(ci.Native);
        InitializeUserOwned(native);
    }

    internal SphereSphereCollisionAlgorithm(IntPtr native, BulletObject? owner)
    {
        InitializeSubObject(native, owner);
    }

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSphereSphereCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new SphereSphereCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
