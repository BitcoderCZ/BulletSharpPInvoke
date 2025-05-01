using System;
using static BulletSharp.UnsafeNativeMethods;
using static BulletSharp.Utils.ThrowHelper;

namespace BulletSharp;

public class SoftBodyConcaveCollisionAlgorithm : CollisionAlgorithm
{
    public SoftBodyConcaveCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap, bool isSwapped)
    {
        IntPtr native = btSoftBodyConcaveCollisionAlgorithm_new(ci.Native, body0Wrap.Native, body1Wrap.Native, isSwapped);
        InitializeUserOwned(native);
    }

    internal SoftBodyConcaveCollisionAlgorithm(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public void ClearCache()
        => btSoftBodyConcaveCollisionAlgorithm_clearCache(Native);

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSoftBodyConcaveCollisionAlgorithm_CreateFunc_new();
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

            return new SoftBodyConcaveCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
        }
    }

    public class SwappedCreateFunc : CollisionAlgorithmCreateFunc
    {
        public SwappedCreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSoftBodyConcaveCollisionAlgorithm_SwappedCreateFunc_new();
            InitializeUserOwned(native);
        }

        internal SwappedCreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
        {
            ThrowIfNull(__unnamed0.Dispatcher);

            return new SoftBodyConcaveCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
        }
    }
}
