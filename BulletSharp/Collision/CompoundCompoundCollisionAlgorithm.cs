using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class CompoundCompoundCollisionAlgorithm : CompoundCollisionAlgorithm
{
    public CompoundCompoundCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap, bool isSwapped)
    {
        IntPtr native = btCompoundCompoundCollisionAlgorithm_new(ci.Native, body0Wrap.Native, body1Wrap.Native, isSwapped);
        InitializeUserOwned(native);
    }

    internal CompoundCompoundCollisionAlgorithm(IntPtr native, BulletObject? owner)
    {
        InitializeSubObject(native, owner);
    }

    public new class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btCompoundCompoundCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new CompoundCompoundCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }

    public new class SwappedCreateFunc : CollisionAlgorithmCreateFunc
    {
        public SwappedCreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btCompoundCompoundCollisionAlgorithm_SwappedCreateFunc_new();
            InitializeUserOwned(native);
        }

        internal SwappedCreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new CompoundCompoundCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
