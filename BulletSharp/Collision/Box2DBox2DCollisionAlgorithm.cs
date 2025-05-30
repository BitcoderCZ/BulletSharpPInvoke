using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class Box2DBox2DCollisionAlgorithm : ActivatingCollisionAlgorithm
{
    public Box2DBox2DCollisionAlgorithm(CollisionAlgorithmConstructionInfo ci)
    {
        IntPtr native = btBox2dBox2dCollisionAlgorithm_new(ci.Native);
        InitializeUserOwned(native);
    }

    public Box2DBox2DCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
    {
        IntPtr native = btBox2dBox2dCollisionAlgorithm_new2(mf.Native, ci.Native, body0Wrap.Native, body1Wrap.Native);
        InitializeUserOwned(native);
    }

    internal Box2DBox2DCollisionAlgorithm(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btBox2dBox2dCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new Box2DBox2DCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
