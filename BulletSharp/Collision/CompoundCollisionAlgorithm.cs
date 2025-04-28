using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class CompoundCollisionAlgorithm : ActivatingCollisionAlgorithm
{
    public CompoundCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap, bool isSwapped)
    {
        IntPtr native = btCompoundCollisionAlgorithm_new(ci.Native, body0Wrap.Native, body1Wrap.Native, isSwapped);
        InitializeUserOwned(native);
    }

    internal CompoundCollisionAlgorithm(IntPtr native, BulletObject? owner)
    {
        InitializeSubObject(native, owner);
    }

    protected internal CompoundCollisionAlgorithm()
    {
    }

    public CollisionAlgorithm GetChildAlgorithm(int n) => new CollisionAlgorithm(btCompoundCollisionAlgorithm_getChildAlgorithm(Native, n), this);

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btCompoundCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(ConstructionInfo info)
            : base(info)
        {
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new CompoundCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }

    public class SwappedCreateFunc : CollisionAlgorithmCreateFunc
    {
        public SwappedCreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btCompoundCollisionAlgorithm_SwappedCreateFunc_new();
            InitializeUserOwned(native);
        }

        internal SwappedCreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new CompoundCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
