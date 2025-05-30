using System;
using System.Numerics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class SphereBoxCollisionAlgorithm : ActivatingCollisionAlgorithm
{
    public SphereBoxCollisionAlgorithm(PersistentManifold mf, CollisionAlgorithmConstructionInfo ci, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap, bool isSwapped)
    {
        IntPtr native = btSphereBoxCollisionAlgorithm_new(mf.Native, ci.Native, body0Wrap.Native, body1Wrap.Native, isSwapped);
        InitializeUserOwned(native);
    }

    internal SphereBoxCollisionAlgorithm(IntPtr native, BulletObject? owner)
    {
        InitializeSubObject(native, owner);
    }

    public bool GetSphereDistanceRef(CollisionObjectWrapper boxObjWrap, out Vector3 v3PointOnBox, out Vector3 normal, out float penetrationDepth, Vector3 v3SphereCenter, float fRadius, float maxContactDistance)
        => btSphereBoxCollisionAlgorithm_getSphereDistance(Native, boxObjWrap.Native, out v3PointOnBox, out normal, out penetrationDepth, ref v3SphereCenter, fRadius, maxContactDistance);

    public bool GetSphereDistance(CollisionObjectWrapper boxObjWrap, out Vector3 v3PointOnBox, out Vector3 normal, out float penetrationDepth, Vector3 v3SphereCenter, float fRadius, float maxContactDistance)
        => btSphereBoxCollisionAlgorithm_getSphereDistance(Native, boxObjWrap.Native, out v3PointOnBox, out normal, out penetrationDepth, ref v3SphereCenter, fRadius, maxContactDistance);

    public float GetSpherePenetrationRef(ref Vector3 boxHalfExtent, ref Vector3 sphereRelPos, out Vector3 closestPoint, out Vector3 normal)
        => btSphereBoxCollisionAlgorithm_getSpherePenetration(Native, ref boxHalfExtent, ref sphereRelPos, out closestPoint, out normal);

    public float GetSpherePenetration(Vector3 boxHalfExtent, Vector3 sphereRelPos, out Vector3 closestPoint, out Vector3 normal)
        => btSphereBoxCollisionAlgorithm_getSpherePenetration(Native, ref boxHalfExtent, ref sphereRelPos, out closestPoint, out normal);

    public class CreateFunc : CollisionAlgorithmCreateFunc
    {
        public CreateFunc()
            : base(ConstructionInfo.Null)
        {
            IntPtr native = btSphereBoxCollisionAlgorithm_CreateFunc_new();
            InitializeUserOwned(native);
        }

        internal CreateFunc(IntPtr native, BulletObject owner)
            : base(ConstructionInfo.Null)
        {
            InitializeSubObject(native, owner);
        }

        public override CollisionAlgorithm CreateCollisionAlgorithm(CollisionAlgorithmConstructionInfo __unnamed0, CollisionObjectWrapper body0Wrap, CollisionObjectWrapper body1Wrap)
            => new SphereBoxCollisionAlgorithm(btCollisionAlgorithmCreateFunc_CreateCollisionAlgorithm(Native, __unnamed0.Native, body0Wrap.Native, body1Wrap.Native), __unnamed0.Dispatcher);
    }
}
