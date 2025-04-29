using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public delegate void ContactDestroyedEventHandler(object userPersistantData);

public delegate void ContactProcessedEventHandler(ManifoldPoint cp, CollisionObject? body0, CollisionObject? body1);

#pragma warning disable IDE0044
#pragma warning disable SA1306 // Field names should begin with lower-case letter
#pragma warning disable SX1309 // Field names should begin with underscore
[StructLayout(LayoutKind.Sequential)]
internal struct PersistentManifoldDoubleData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCacheLocalPointA;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCacheLocalPointB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCachePositionWorldOnA;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCachePositionWorldOnB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCacheNormalWorldOnB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCacheLateralFrictionDir1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3DoubleData[] PointCacheLateralFrictionDir2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheDistance;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheAppliedImpulse;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedRollingFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedSpinningFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedRestitution;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCachePartId0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCachePartId1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheIndex0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheIndex1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheContactPointFlags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheAppliedImpulseLateral1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheAppliedImpulseLateral2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheContactMotion1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheContactMotion2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheContactCfm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedContactStiffness1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheContactErp;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheCombinedContactDamping1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private double[] PointCacheFrictionCfm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheLifeTime;
    private int NumCachedPoints;
    private int CompanionIdA;
    private int CompanionIdB;
    private int Index1a;
    private int ObjectType;
    private double ContactBreakingThreshold;
    private double ContactProcessingThreshold;
    private int Padding;
    private IntPtr Body0;
    private IntPtr Body1;
}

[StructLayout(LayoutKind.Sequential)]
internal struct PersistentManifoldFloatData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCacheLocalPointA;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCacheLocalPointB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCachePositionWorldOnA;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCachePositionWorldOnB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCacheNormalWorldOnB;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCacheLateralFrictionDir1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private Vector3FloatData[] PointCacheLateralFrictionDir2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheDistance;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheAppliedImpulse;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedRollingFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedSpinningFriction;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedRestitution;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCachePartId0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCachePartId1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheIndex0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheIndex1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheContactPointFlags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheAppliedImpulseLateral1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheAppliedImpulseLateral2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheContactMotion1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheContactMotion2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheContactCfm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedContactStiffness1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheContactErp;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheCombinedContactDamping1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private float[] PointCacheFrictionCfm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    private int[] PointCacheLifeTime;
    private int NumCachedPoints;
    private int CompanionIdA;
    private int CompanionIdB;
    private int Index1a;
    private int ObjectType;
    private float ContactBreakingThreshold;
    private float ContactProcessingThreshold;
    private int Padding;
    private IntPtr Body0;
    private IntPtr Body1;
}
#pragma warning restore SX1309 // Field names should begin with underscore
#pragma warning restore SA1306 // Field names should begin with lower-case letter
#pragma warning restore IDE0044

public class PersistentManifold : BulletDisposableObject //: TypedObject
{
    private static ContactDestroyedEventHandler? _contactDestroyed;
    private static ContactProcessedEventHandler? _contactProcessed;
    private static ContactDestroyedUnmanagedDelegate? _contactDestroyedUnmanaged;
    private static ContactProcessedUnmanagedDelegate? _contactProcessedUnmanaged;
    private static IntPtr _contactDestroyedUnmanagedPtr;
    private static IntPtr _contactProcessedUnmanagedPtr;

    public PersistentManifold()
    {
        IntPtr native = btPersistentManifold_new();
        InitializeUserOwned(native);
    }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public PersistentManifold(CollisionObject body0, CollisionObject body1, int __unnamed2, float contactBreakingThreshold, float contactProcessingThreshold)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
        IntPtr native = btPersistentManifold_new2(body0.Native, body1.Native, __unnamed2, contactBreakingThreshold, contactProcessingThreshold);
        InitializeUserOwned(native);
    }

    internal PersistentManifold(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool ContactDestroyedUnmanagedDelegate(IntPtr userPersistantData);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool ContactProcessedUnmanagedDelegate(IntPtr cp, IntPtr body0, IntPtr body1);

    public static event ContactDestroyedEventHandler ContactDestroyed
    {
        add
        {
            if (_contactDestroyedUnmanaged == null)
            {
                _contactDestroyedUnmanaged = new ContactDestroyedUnmanagedDelegate(ContactDestroyedUnmanaged);
                _contactDestroyedUnmanagedPtr = Marshal.GetFunctionPointerForDelegate(_contactDestroyedUnmanaged);
            }

            setGContactDestroyedCallback(_contactDestroyedUnmanagedPtr);
            _contactDestroyed += value;
        }

        remove
        {
            _contactDestroyed -= value;
            if (_contactDestroyed == null)
            {
                setGContactDestroyedCallback(IntPtr.Zero);
            }
        }
    }

    public static event ContactProcessedEventHandler ContactProcessed
    {
        add
        {
            if (_contactProcessedUnmanaged == null)
            {
                _contactProcessedUnmanaged = new ContactProcessedUnmanagedDelegate(ContactProcessedUnmanaged);
                _contactProcessedUnmanagedPtr = Marshal.GetFunctionPointerForDelegate(_contactProcessedUnmanaged);
            }

            setGContactProcessedCallback(_contactProcessedUnmanagedPtr);
            _contactProcessed += value;
        }

        remove
        {
            _contactProcessed -= value;
            if (_contactProcessed == null)
            {
                setGContactProcessedCallback(IntPtr.Zero);
            }
        }
    }

    public CollisionObject? Body0 => CollisionObject.GetManaged(btPersistentManifold_getBody0(Native));

    public CollisionObject? Body1 => CollisionObject.GetManaged(btPersistentManifold_getBody1(Native));

    public int CompanionIdA
    {
        get => btPersistentManifold_getCompanionIdA(Native);
        set => btPersistentManifold_setCompanionIdA(Native, value);
    }

    public int CompanionIdB
    {
        get => btPersistentManifold_getCompanionIdB(Native);
        set => btPersistentManifold_setCompanionIdB(Native, value);
    }

    public float ContactBreakingThreshold
    {
        get => btPersistentManifold_getContactBreakingThreshold(Native);
        set => btPersistentManifold_setContactBreakingThreshold(Native, value);
    }

    public float ContactProcessingThreshold
    {
        get => btPersistentManifold_getContactProcessingThreshold(Native);
        set => btPersistentManifold_setContactProcessingThreshold(Native, value);
    }

    public int Index1A
    {
        get => btPersistentManifold_getIndex1a(Native);
        set => btPersistentManifold_setIndex1a(Native, value);
    }

    public int NumContacts
    {
        get => btPersistentManifold_getNumContacts(Native);
        set => btPersistentManifold_setNumContacts(Native, value);
    }

    public int AddManifoldPoint(ManifoldPoint newPoint, bool isPredictive = false)
        => btPersistentManifold_addManifoldPoint(Native, newPoint.Native, isPredictive);

    public void ClearManifold()
        => btPersistentManifold_clearManifold(Native);

    public void ClearUserCache(ManifoldPoint pt)
        => btPersistentManifold_clearUserCache(Native, pt.Native);

    public int GetCacheEntry(ManifoldPoint newPoint)
        => btPersistentManifold_getCacheEntry(Native, newPoint.Native);

    public ManifoldPoint GetContactPoint(int index)
        => new ManifoldPoint(btPersistentManifold_getContactPoint(Native, index));

    public void RefreshContactPointsRef(ref Matrix4x4 trA, ref Matrix4x4 trB)
        => btPersistentManifold_refreshContactPoints(Native, ref trA, ref trB);

    public void RefreshContactPoints(Matrix4x4 trA, Matrix4x4 trB)
        => btPersistentManifold_refreshContactPoints(Native, ref trA, ref trB);

    public void RemoveContactPoint(int index)
        => btPersistentManifold_removeContactPoint(Native, index);

    public void ReplaceContactPoint(ManifoldPoint newPoint, int insertIndex)
        => btPersistentManifold_replaceContactPoint(Native, newPoint.Native, insertIndex);

    public void SetBodies(CollisionObject body0, CollisionObject body1)
        => btPersistentManifold_setBodies(Native, body0.Native, body1.Native);

    public bool ValidContactDistance(ManifoldPoint pt)
        => btPersistentManifold_validContactDistance(Native, pt.Native);

    protected override void Dispose(bool disposing)
    {
        if (IsUserOwned)
        {
            btPersistentManifold_delete(Native);
        }
    }

    private static bool ContactDestroyedUnmanaged(IntPtr userPersistentData)
    {
        _contactDestroyed?.Invoke(GCHandle.FromIntPtr(userPersistentData).Target);
        return false;
    }

    private static bool ContactProcessedUnmanaged(IntPtr cp, IntPtr body0, IntPtr body1)
    {
        _contactProcessed?.Invoke(new ManifoldPoint(cp), CollisionObject.GetManaged(body0), CollisionObject.GetManaged(body1));
        return false;
    }
}
