using System;
using static BulletSharp.UnsafeNativeMethods;
using static BulletSharp.Utils.ThrowHelper;

namespace BulletSharp;

public class SoftSoftCollisionAlgorithm : CollisionAlgorithm
{
    public SoftSoftCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
    {
        IntPtr native = btSoftSoftCollisionAlgorithm_new(ci.Native);
        InitializeUserOwned(native);
    }

    public SoftSoftCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
    {
        IntPtr native = btSoftSoftCollisionAlgorithm_new2(mf.Native, ci.Native, body0Wrap.Native, body1Wrap.Native);
        InitializeUserOwned(native);
    }

    internal SoftSoftCollisionAlgorithm(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSoftSoftCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
        {
            ThrowIfNull(__unnamed0.Dispatcher);

            return new SoftSoftCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
        }
    }
}
