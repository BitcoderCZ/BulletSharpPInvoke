using System;
using static BulletSharp.UnsafeNativeMethods;
using static BulletSharp.Utils.ThrowHelper;

namespace BulletSharp;

public class SoftRigidCollisionAlgorithm : CollisionAlgorithm
{
    public SoftRigidCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper col0, CollisionObjectWrapper col1Wrap, bool isSwapped)
    {
        IntPtr native = btSoftRigidCollisionAlgorithm_new(mf.Native, ci.Native, col0.Native, col1Wrap.Native, isSwapped);
        InitializeUserOwned(native);
    }

    internal SoftRigidCollisionAlgorithm(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSoftRigidCollisionAlgorithm_CreateFunc_new();
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

            return new SoftRigidCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
        }
    }
}
