using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class BoxBoxCollisionAlgorithm : ActivatingCollisionAlgorithm
{
    public BoxBoxCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
    {
        IntPtr native = btBoxBoxCollisionAlgorithm_new(ci.Native);
        InitializeUserOwned(native);
    }

    public BoxBoxCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
    {
        IntPtr native = btBoxBoxCollisionAlgorithm_new2(mf.Native, ci.Native, body0Wrap.Native, body1Wrap.Native);
        InitializeUserOwned(native);
    }

    internal BoxBoxCollisionAlgorithm(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btBoxBoxCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new BoxBoxCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
