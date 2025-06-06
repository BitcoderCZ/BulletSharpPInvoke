using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public struct LocalConvexResult
{
    private readonly IntPtr _native;
    private CollisionObject? _hitCollisionObject;

    internal LocalConvexResult(IntPtr native)
    {
        _native = native;
        _hitCollisionObject = null;
    }

    public CollisionObject HitCollisionObject
    {
        get
        {
            if (_hitCollisionObject == null)
            {
                _hitCollisionObject = CollisionObject.GetManaged(btCollisionWorld_LocalConvexResult_getHitCollisionObject(_native));
            }

            Debug.Assert(_hitCollisionObject is not null, $"{nameof(_hitCollisionObject)} shoudn't be null.");

            return _hitCollisionObject;
        }

        set
        {
            btCollisionWorld_LocalConvexResult_setHitCollisionObject(_native, value?.Native ?? IntPtr.Zero);
            _hitCollisionObject = value;
        }
    }

    public readonly float HitFraction
    {
        get => btCollisionWorld_LocalConvexResult_getHitFraction(_native);
        set => btCollisionWorld_LocalConvexResult_setHitFraction(_native, value);
    }

    public readonly Vector3 HitNormalLocal
    {
        get
        {
            Vector3 value;
            btCollisionWorld_LocalConvexResult_getHitNormalLocal(_native, out value);
            return value;
        }
        set => btCollisionWorld_LocalConvexResult_setHitNormalLocal(_native, ref value);
    }

    public readonly Vector3 HitPointLocal
    {
        get
        {
            Vector3 value;
            btCollisionWorld_LocalConvexResult_getHitPointLocal(_native, out value);
            return value;
        }
        set => btCollisionWorld_LocalConvexResult_setHitPointLocal(_native, ref value);
    }

    public readonly LocalShapeInfo? LocalShapeInfo
    {
        get
        {
            IntPtr localShapeInfoPtr = btCollisionWorld_LocalConvexResult_getLocalShapeInfo(_native);
            return localShapeInfoPtr != IntPtr.Zero
                ? new LocalShapeInfo?(new LocalShapeInfo(localShapeInfoPtr))
                : null;
        }
    }
}

public struct LocalRayResult
{
    private readonly IntPtr _native;
    private CollisionObject? _collisionObject;

    internal LocalRayResult(IntPtr native)
    {
        _native = native;
        _collisionObject = null;
    }

    public CollisionObject CollisionObject
    {
        get
        {
            if (_collisionObject == null)
            {
                _collisionObject = CollisionObject.GetManaged(btCollisionWorld_LocalRayResult_getCollisionObject(_native));
            }

            Debug.Assert(_collisionObject is not null, $"{nameof(_collisionObject)} shoudn't be null.");

            return _collisionObject;
        }

        set
        {
            btCollisionWorld_LocalRayResult_setCollisionObject(_native, value?.Native ?? IntPtr.Zero);
            _collisionObject = value;
        }
    }

    public readonly float HitFraction
    {
        get => btCollisionWorld_LocalRayResult_getHitFraction(_native);
        set => btCollisionWorld_LocalRayResult_setHitFraction(_native, value);
    }

    public readonly Vector3 HitNormalLocal
    {
        get
        {
            Vector3 value;
            btCollisionWorld_LocalRayResult_getHitNormalLocal(_native, out value);
            return value;
        }
        set => btCollisionWorld_LocalRayResult_setHitNormalLocal(_native, ref value);
    }

    public readonly LocalShapeInfo? LocalShapeInfo
    {
        get
        {
            IntPtr localShapeInfoPtr = btCollisionWorld_LocalRayResult_getLocalShapeInfo(_native);
            return localShapeInfoPtr != IntPtr.Zero
                ? new LocalShapeInfo?(new LocalShapeInfo(localShapeInfoPtr))
                : null;
        }
    }
}

public struct LocalShapeInfo
{
    public int ShapePart;
    public int TriangleIndex;

    internal LocalShapeInfo(IntPtr native)
    {
        ShapePart = btCollisionWorld_LocalShapeInfo_getShapePart(native);
        TriangleIndex = btCollisionWorld_LocalShapeInfo_getTriangleIndex(native);
    }
}

public class AllHitsRayResultCallback : RayResultCallback
{
    public AllHitsRayResultCallback(Vector3 rayFromWorld, Vector3 rayToWorld)
    {
        RayFromWorld = rayFromWorld;
        RayToWorld = rayToWorld;

        CollisionObjects = [];
        HitFractions = [];
        HitNormalWorld = [];
        HitPointWorld = [];
    }

    public List<CollisionObject> CollisionObjects { get; set; }

    public List<float> HitFractions { get; set; }

    public List<Vector3> HitNormalWorld { get; set; }

    public List<Vector3> HitPointWorld { get; set; }

    public Vector3 RayFromWorld { get; set; }

    public Vector3 RayToWorld { get; set; }

    public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
    {
        CollisionObject = rayResult.CollisionObject;
        CollisionObjects.Add(rayResult.CollisionObject);
        if (normalInWorldSpace)
        {
            HitNormalWorld.Add(rayResult.HitNormalLocal);
        }
        else
        {
            // need to transform normal into worldspace
            Matrix4x4 transform = CollisionObject.WorldTransform;
            HitNormalWorld.Add(Vector3.Transform(rayResult.HitNormalLocal, transform.GetBasis()));
        }

        HitPointWorld.Add(Vector3.Lerp(RayFromWorld, RayToWorld, rayResult.HitFraction));
        HitFractions.Add(rayResult.HitFraction);
        return ClosestHitFraction;
    }
}

public class ClosestConvexResultCallback : ConvexResultCallback
{
    public ClosestConvexResultCallback()
    {
    }

    public ClosestConvexResultCallback(ref Vector3 convexFromWorld, ref Vector3 convexToWorld)
    {
        ConvexFromWorld = convexFromWorld;
        ConvexToWorld = convexToWorld;
    }

    public Vector3 ConvexFromWorld { get; set; }

    public Vector3 ConvexToWorld { get; set; }

    public CollisionObject? HitCollisionObject { get; set; }

    public Vector3 HitNormalWorld { get; set; }

    public Vector3 HitPointWorld { get; set; }

    public override float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace)
    {
        //caller already does the filter on the m_closestHitFraction
        Debug.Assert(convexResult.HitFraction <= ClosestHitFraction);

        ClosestHitFraction = convexResult.HitFraction;
        HitCollisionObject = convexResult.HitCollisionObject;
        if (normalInWorldSpace)
        {
            HitNormalWorld = convexResult.HitNormalLocal;
        }
        else
        {
            // need to transform normal into worldspace
            Matrix4x4 transform = HitCollisionObject.WorldTransform;
            HitNormalWorld = Vector3.Transform(convexResult.HitNormalLocal, transform.GetBasis());
        }

        HitPointWorld = convexResult.HitPointLocal;
        return convexResult.HitFraction;
    }
}

public class ClosestRayResultCallback : RayResultCallback
{
    public ClosestRayResultCallback(ref Vector3 rayFromWorld, ref Vector3 rayToWorld)
    {
        RayFromWorld = rayFromWorld;
        RayToWorld = rayToWorld;
    }

    public Vector3 RayFromWorld { get; set; } //used to calculate hitPointWorld from hitFraction

    public Vector3 RayToWorld { get; set; }

    public Vector3 HitNormalWorld { get; set; }

    public Vector3 HitPointWorld { get; set; }

    public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
    {
        //caller already does the filter on the m_closestHitFraction
        Debug.Assert(rayResult.HitFraction <= ClosestHitFraction);

        ClosestHitFraction = rayResult.HitFraction;
        CollisionObject = rayResult.CollisionObject;
        if (normalInWorldSpace)
        {
            HitNormalWorld = rayResult.HitNormalLocal;
        }
        else
        {
            // need to transform normal into worldspace
            Matrix4x4 transform = CollisionObject.WorldTransform;
            HitNormalWorld = Vector3.Transform(rayResult.HitNormalLocal, transform.GetBasis());
        }

        HitPointWorld = Vector3.Lerp(RayFromWorld, RayToWorld, rayResult.HitFraction);
        return rayResult.HitFraction;
    }
}

public abstract class ContactResultCallback : BulletDisposableObject
{
    private readonly AddSingleResultUnmanagedDelegate _addSingleResult;
    private readonly NeedsCollisionUnmanagedDelegate _needsCollision;

    public ContactResultCallback()
    {
        _addSingleResult = AddSingleResultUnmanaged;
        _needsCollision = NeedsCollisionUnmanaged;
        IntPtr native = btCollisionWorld_ContactResultCallbackWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_addSingleResult),
            Marshal.GetFunctionPointerForDelegate(_needsCollision));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate float AddSingleResultUnmanagedDelegate(IntPtr cp, IntPtr colObj0Wrap, int partId0, int index0, IntPtr colObj1Wrap, int partId1, int index1);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool NeedsCollisionUnmanagedDelegate(IntPtr proxy0);

    public float ClosestDistanceThreshold
    {
        get => btCollisionWorld_ContactResultCallback_getClosestDistanceThreshold(Native);
        set => btCollisionWorld_ContactResultCallback_setClosestDistanceThreshold(Native, value);
    }

    public int CollisionFilterGroup
    {
        get => btCollisionWorld_ContactResultCallback_getCollisionFilterGroup(Native);
        set => btCollisionWorld_ContactResultCallback_setCollisionFilterGroup(Native, value);
    }

    public int CollisionFilterMask
    {
        get => btCollisionWorld_ContactResultCallback_getCollisionFilterMask(Native);
        set => btCollisionWorld_ContactResultCallback_setCollisionFilterMask(Native, value);
    }

    public abstract float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1);

    public virtual bool NeedsCollision(BroadphaseProxy proxy0)
        => btCollisionWorld_ContactResultCallbackWrapper_needsCollision(Native, proxy0.Native);

    protected override void Dispose(bool disposing)
        => btCollisionWorld_ContactResultCallback_delete(Native);

    private bool NeedsCollisionUnmanaged(IntPtr proxy0)
    {
        BroadphaseProxy? managed = BroadphaseProxy.GetManaged(proxy0);

        Debug.Assert(managed is not null, $"{nameof(managed)} shoud not be null.");

        return NeedsCollision(managed);
    }

    private float AddSingleResultUnmanaged(IntPtr cp, IntPtr colObj0Wrap, int partId0, int index0, IntPtr colObj1Wrap, int partId1, int index1)
        => AddSingleResult(new ManifoldPoint(cp), new CollisionObjectWrapper(colObj0Wrap), partId0, index0, new CollisionObjectWrapper(colObj1Wrap), partId1, index1);
}

public abstract class ConvexResultCallback : BulletDisposableObject
{
    private readonly AddSingleResultUnmanagedDelegate _addSingleResult;
    private readonly NeedsCollisionUnmanagedDelegate _needsCollision;

    protected ConvexResultCallback()
    {
        _addSingleResult = AddSingleResultUnmanaged;
        _needsCollision = NeedsCollisionUnmanaged;
        IntPtr native = btCollisionWorld_ConvexResultCallbackWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_addSingleResult),
            Marshal.GetFunctionPointerForDelegate(_needsCollision));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate float AddSingleResultUnmanagedDelegate(IntPtr convexResult, bool normalInWorldSpace);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool NeedsCollisionUnmanagedDelegate(IntPtr proxy0);

    public float ClosestHitFraction
    {
        get => btCollisionWorld_ConvexResultCallback_getClosestHitFraction(Native);
        set => btCollisionWorld_ConvexResultCallback_setClosestHitFraction(Native, value);
    }

    public int CollisionFilterGroup
    {
        get => btCollisionWorld_ConvexResultCallback_getCollisionFilterGroup(Native);
        set => btCollisionWorld_ConvexResultCallback_setCollisionFilterGroup(Native, value);
    }

    public int CollisionFilterMask
    {
        get => btCollisionWorld_ConvexResultCallback_getCollisionFilterMask(Native);
        set => btCollisionWorld_ConvexResultCallback_setCollisionFilterMask(Native, value);
    }

    public bool HasHit => btCollisionWorld_ConvexResultCallback_hasHit(Native);

    public abstract float AddSingleResult(ref LocalConvexResult convexResult, bool normalInWorldSpace);

    public virtual bool NeedsCollision(BroadphaseProxy proxy0)
        => btCollisionWorld_ConvexResultCallbackWrapper_needsCollision(Native, proxy0.Native);

    protected override void Dispose(bool disposing)
        => btCollisionWorld_ConvexResultCallback_delete(Native);

    private float AddSingleResultUnmanaged(IntPtr convexResult, bool normalInWorldSpace)
    {
        LocalConvexResult convexResultManaged = new LocalConvexResult(convexResult);
        return AddSingleResult(ref convexResultManaged, normalInWorldSpace);
    }

    private bool NeedsCollisionUnmanaged(IntPtr proxy0)
    {
        BroadphaseProxy? managed = BroadphaseProxy.GetManaged(proxy0);

        Debug.Assert(managed is not null, $"{nameof(managed)} shoud not be null.");

        return NeedsCollision(managed);
    }
}

public abstract class RayResultCallback : BulletDisposableObject
{
    private readonly AddSingleResultUnmanagedDelegate _addSingleResult;
    private readonly NeedsCollisionUnmanagedDelegate _needsCollision;

    protected RayResultCallback()
    {
        _addSingleResult = AddSingleResultUnmanaged;
        _needsCollision = NeedsCollisionUnmanaged;

        IntPtr native = btCollisionWorld_RayResultCallbackWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_addSingleResult),
            Marshal.GetFunctionPointerForDelegate(_needsCollision));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate float AddSingleResultUnmanagedDelegate(IntPtr rayResult, bool normalInWorldSpace);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool NeedsCollisionUnmanagedDelegate(IntPtr proxy0);

    public float ClosestHitFraction
    {
        get => btCollisionWorld_RayResultCallback_getClosestHitFraction(Native);
        set => btCollisionWorld_RayResultCallback_setClosestHitFraction(Native, value);
    }

    public int CollisionFilterGroup
    {
        get => btCollisionWorld_RayResultCallback_getCollisionFilterGroup(Native);
        set => btCollisionWorld_RayResultCallback_setCollisionFilterGroup(Native, value);
    }

    public int CollisionFilterMask
    {
        get => btCollisionWorld_RayResultCallback_getCollisionFilterMask(Native);
        set => btCollisionWorld_RayResultCallback_setCollisionFilterMask(Native, value);
    }

    public CollisionObject? CollisionObject
    {
        get => CollisionObject.GetManaged(btCollisionWorld_RayResultCallback_getCollisionObject(Native));
        set => btCollisionWorld_RayResultCallback_setCollisionObject(Native, (value != null) ? value.Native : IntPtr.Zero);
    }

    public uint Flags
    {
        get => btCollisionWorld_RayResultCallback_getFlags(Native);
        set => btCollisionWorld_RayResultCallback_setFlags(Native, value);
    }

    [MemberNotNullWhen(true, nameof(CollisionObject))]
    public bool HasHit => btCollisionWorld_RayResultCallback_hasHit(Native);

    public abstract float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace);

    public virtual bool NeedsCollision(BroadphaseProxy proxy0)
        => btCollisionWorld_RayResultCallbackWrapper_needsCollision(Native, proxy0.Native);

    protected override void Dispose(bool disposing)
        => btCollisionWorld_RayResultCallback_delete(Native);

    private float AddSingleResultUnmanaged(IntPtr rayResult, bool normalInWorldSpace)
    {
        LocalRayResult localRayResult = new LocalRayResult(rayResult);
        return AddSingleResult(ref localRayResult, normalInWorldSpace);
    }

    private bool NeedsCollisionUnmanaged(IntPtr proxy0)
    {
        BroadphaseProxy? managed = BroadphaseProxy.GetManaged(proxy0);

        Debug.Assert(managed is not null, $"{nameof(managed)} shoud not be null.");

        return NeedsCollision(managed);
    }
}

public class CollisionWorld : BulletDisposableObject
{
    private DebugDraw? _debugDrawer;
    private BroadphaseInterface _broadphase;
    private DispatcherInfo? _dispatchInfo;

    public CollisionWorld(Dispatcher dispatcher, BroadphaseInterface broadphasePairCache, CollisionConfiguration collisionConfiguration)
    {
        IntPtr native = btCollisionWorld_new(dispatcher.Native, broadphasePairCache.Native, collisionConfiguration.Native);
        InitializeUserOwned(native);
        InitializeMembers(dispatcher, broadphasePairCache);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected internal CollisionWorld()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public BroadphaseInterface Broadphase
    {
        get
        {
            Debug.Assert(_broadphase is not null, $"{_broadphase} should not be null.");

            return _broadphase;
        }

        [MemberNotNull(nameof(_broadphase))]
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            btCollisionWorld_setBroadphase(Native, value.Native);
            _broadphase = value;
        }
    }

    public AlignedCollisionObjectArray CollisionObjectArray { get; protected set; }

    public DebugDraw? DebugDrawer
    {
        get => _debugDrawer;
        set
        {
            if (_debugDrawer != value)
            {
                _debugDrawer = value;
                btCollisionWorld_setDebugDrawer(Native, value != null ? value.Native : IntPtr.Zero);
            }
        }
    }

    public Dispatcher Dispatcher { get; private set; }

    public DispatcherInfo DispatchInfo
    {
        get
        {
            if (_dispatchInfo == null)
            {
                _dispatchInfo = new DispatcherInfo(btCollisionWorld_getDispatchInfo(Native));
            }

            return _dispatchInfo;
        }
    }

    public bool ForceUpdateAllAabbs
    {
        get => btCollisionWorld_getForceUpdateAllAabbs(Native);
        set => btCollisionWorld_setForceUpdateAllAabbs(Native, value);
    }

    public int NumCollisionObjects => btCollisionWorld_getNumCollisionObjects(Native);

    public OverlappingPairCache? PairCache => Broadphase?.OverlappingPairCache;

    public static void ObjectQuerySingleRef(ConvexShape castShape, ref Matrix4x4 rayFromTrans, ref Matrix4x4 rayToTrans, CollisionObject collisionObject, CollisionShape collisionShape, ref Matrix4x4 colObjWorldTransform, ConvexResultCallback resultCallback, float allowedPenetration)
        => btCollisionWorld_objectQuerySingle(castShape.Native, ref rayFromTrans, ref rayToTrans, collisionObject.Native, collisionShape.Native, ref colObjWorldTransform, resultCallback.Native, allowedPenetration);

    public static void ObjectQuerySingle(ConvexShape castShape, Matrix4x4 rayFromTrans, Matrix4x4 rayToTrans, CollisionObject collisionObject, CollisionShape collisionShape, Matrix4x4 colObjWorldTransform, ConvexResultCallback resultCallback, float allowedPenetration)
        => btCollisionWorld_objectQuerySingle(castShape.Native, ref rayFromTrans, ref rayToTrans, collisionObject.Native, collisionShape.Native, ref colObjWorldTransform, resultCallback.Native, allowedPenetration);

    public static void ObjectQuerySingleInternalRef(ConvexShape castShape, ref Matrix4x4 convexFromTrans, ref Matrix4x4 convexToTrans, CollisionObjectWrapper colObjWrap, ConvexResultCallback resultCallback, float allowedPenetration)
        => btCollisionWorld_objectQuerySingleInternal(castShape.Native, ref convexFromTrans, ref convexToTrans, colObjWrap.Native, resultCallback.Native, allowedPenetration);

    public static void ObjectQuerySingleInternal(ConvexShape castShape, Matrix4x4 convexFromTrans, Matrix4x4 convexToTrans, CollisionObjectWrapper colObjWrap, ConvexResultCallback resultCallback, float allowedPenetration)
        => btCollisionWorld_objectQuerySingleInternal(castShape.Native, ref convexFromTrans, ref convexToTrans, colObjWrap.Native, resultCallback.Native, allowedPenetration);

    public static void RayTestSingleRef(ref Matrix4x4 rayFromTrans, ref Matrix4x4 rayToTrans, CollisionObject collisionObject, CollisionShape collisionShape, ref Matrix4x4 colObjWorldTransform, RayResultCallback resultCallback)
        => btCollisionWorld_rayTestSingle(ref rayFromTrans, ref rayToTrans, collisionObject.Native, collisionShape.Native, ref colObjWorldTransform, resultCallback.Native);

    public static void RayTestSingle(Matrix4x4 rayFromTrans, Matrix4x4 rayToTrans, CollisionObject collisionObject, CollisionShape collisionShape, Matrix4x4 colObjWorldTransform, RayResultCallback resultCallback)
        => btCollisionWorld_rayTestSingle(ref rayFromTrans, ref rayToTrans, collisionObject.Native, collisionShape.Native, ref colObjWorldTransform, resultCallback.Native);

    public static void RayTestSingleInternalRef(ref Matrix4x4 rayFromTrans, ref Matrix4x4 rayToTrans, CollisionObjectWrapper collisionObjectWrap, RayResultCallback resultCallback)
        => btCollisionWorld_rayTestSingleInternal(ref rayFromTrans, ref rayToTrans, collisionObjectWrap.Native, resultCallback.Native);

    public static void RayTestSingleInternal(Matrix4x4 rayFromTrans, Matrix4x4 rayToTrans, CollisionObjectWrapper collisionObjectWrap, RayResultCallback resultCallback)
        => btCollisionWorld_rayTestSingleInternal(ref rayFromTrans, ref rayToTrans, collisionObjectWrap.Native, resultCallback.Native);

    public void AddCollisionObject(CollisionObject collisionObject)
        => CollisionObjectArray.Add(collisionObject);

    public void AddCollisionObject(CollisionObject collisionObject, CollisionFilterGroups collisionFilterGroup, CollisionFilterGroups collisionFilterMask)
        => CollisionObjectArray.Add(collisionObject, (int)collisionFilterGroup, (int)collisionFilterMask);

    public void AddCollisionObject(CollisionObject collisionObject, int collisionFilterGroup, int collisionFilterMask)
        => CollisionObjectArray.Add(collisionObject, collisionFilterGroup, collisionFilterMask);

    public void ComputeOverlappingPairs()
        => btCollisionWorld_computeOverlappingPairs(Native);

    public void ContactPairTest(CollisionObject colObjA, CollisionObject colObjB, ContactResultCallback resultCallback)
        => btCollisionWorld_contactPairTest(Native, colObjA.Native, colObjB.Native, resultCallback.Native);

    public void ContactTest(CollisionObject colObj, ContactResultCallback resultCallback)
        => btCollisionWorld_contactTest(Native, colObj.Native, resultCallback.Native);

    public void ConvexSweepTestRef(ConvexShape castShape, ref Matrix4x4 from, ref Matrix4x4 to, ConvexResultCallback resultCallback, float allowedCcdPenetration = 0)
        => btCollisionWorld_convexSweepTest(Native, castShape.Native, ref from, ref to, resultCallback.Native, allowedCcdPenetration);

    public void ConvexSweepTest(ConvexShape castShape, Matrix4x4 from, Matrix4x4 to, ConvexResultCallback resultCallback, float allowedCcdPenetration = 0)
        => btCollisionWorld_convexSweepTest(Native, castShape.Native, ref from, ref to, resultCallback.Native, allowedCcdPenetration);

    public void DebugDrawObjectRef(ref Matrix4x4 worldTransform, CollisionShape shape, ref Vector3 color)
        => btCollisionWorld_debugDrawObject(Native, ref worldTransform, shape.Native, ref color);

    public void DebugDrawObject(Matrix4x4 worldTransform, CollisionShape shape, Vector3 color)
        => btCollisionWorld_debugDrawObject(Native, ref worldTransform, shape.Native, ref color);

    public void DebugDrawWorld()
        => btCollisionWorld_debugDrawWorld(Native);

    public void PerformDiscreteCollisionDetection()
        => btCollisionWorld_performDiscreteCollisionDetection(Native);

    public void RayTestRef(ref Vector3 rayFromWorld, ref Vector3 rayToWorld, RayResultCallback resultCallback)
        => btCollisionWorld_rayTest(Native, ref rayFromWorld, ref rayToWorld, resultCallback.Native);

    public void RayTest(Vector3 rayFromWorld, Vector3 rayToWorld, RayResultCallback resultCallback)
        => btCollisionWorld_rayTest(Native, ref rayFromWorld, ref rayToWorld, resultCallback.Native);

    public void RemoveCollisionObject(CollisionObject collisionObject)
        => CollisionObjectArray.Remove(collisionObject);

    public virtual void Serialize(Serializer serializer)
    {
        serializer.StartSerialization();
        SerializeCollisionObjects(serializer);
        serializer.FinishSerialization();
    }

    public void UpdateAabbs() => btCollisionWorld_updateAabbs(Native);

    public void UpdateSingleAabb(CollisionObject colObj) => btCollisionWorld_updateSingleAabb(Native, colObj.Native);

    [MemberNotNull(nameof(Dispatcher), nameof(Broadphase), nameof(_broadphase), nameof(CollisionObjectArray))]
    protected internal void InitializeMembers(Dispatcher dispatcher, BroadphaseInterface broadphasePairCache)
    {
        Dispatcher = dispatcher;
        Broadphase = broadphasePairCache;
        CollisionObjectArray = new AlignedCollisionObjectArray(btCollisionWorld_getCollisionObjectArray(Native), this);
    }

    protected void SerializeCollisionObjects(Serializer serializer)
    {
        // keep track of shapes already serialized
        Dictionary<CollisionShape, int> serializedShapes = [];

        foreach (CollisionObject colObj in CollisionObjectArray)
        {
            CollisionShape? shape = colObj.CollisionShape;

            Debug.Assert(shape is not null, $"{nameof(shape)} shoudn't be null.");

            if (!serializedShapes.ContainsKey(shape))
            {
                serializedShapes.Add(shape, 0);
                shape.SerializeSingleShape(serializer);
            }
        }

        // serialize all collision objects
        foreach (CollisionObject colObj in CollisionObjectArray)
        {
            if (colObj.InternalType == CollisionObjectTypes.CollisionObject)
            {
                colObj.SerializeSingleObject(serializer);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        // The btCollisionWorld will try to clean up remaining objects from the
        // broadphase and the dispatcher. If either of them have been deleted and
        // there are objects in the world, then deleting the btCollisionWorld or 
        // removing the objects from the world will cause an AccessViolationException.
        if (CollisionObjectArray.Count != 0 && ((_broadphase is not null && _broadphase.IsDisposed) || Dispatcher.IsDisposed))
        {
            if (disposing)
            {
                throw new Exception(
                    "To ensure proper resource cleanup, " +
                    "remove all objects from the world before disposing the world.");
            }
            else
            {
                // Do not throw an exception in the GC finalizer thread
            }
        }
        else
        {
            btCollisionWorld_delete(Native);
        }
    }
}
