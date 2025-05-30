using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public abstract class OverlapCallback : BulletDisposableObject
{
    internal OverlapCallback() // public
    {
    }

    public bool ProcessOverlap(BroadphasePair pair) => btOverlapCallback_processOverlap(Native, pair.Native);

    protected override void Dispose(bool disposing) => btOverlapCallback_delete(Native);
}

public abstract class OverlapFilterCallback : BulletDisposableObject
{
    private readonly NeedBroadphaseCollisionUnmanagedDelegate _needBroadphaseCollision;

    public OverlapFilterCallback()
    {
        _needBroadphaseCollision = NeedBroadphaseCollisionUnmanaged;
        IntPtr native = btOverlapFilterCallbackWrapper_new(Marshal.GetFunctionPointerForDelegate(_needBroadphaseCollision));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate bool NeedBroadphaseCollisionUnmanagedDelegate(IntPtr proxy0, IntPtr proxy1);

    public abstract bool NeedBroadphaseCollision(BroadphaseProxy? proxy0, BroadphaseProxy? proxy1);

    protected override void Dispose(bool disposing)
        => btOverlapFilterCallback_delete(Native);

    private bool NeedBroadphaseCollisionUnmanaged(IntPtr proxy0, IntPtr proxy1)
        => NeedBroadphaseCollision(BroadphaseProxy.GetManaged(proxy0), BroadphaseProxy.GetManaged(proxy1));
}

public abstract class OverlappingPairCache : OverlappingPairCallback
{
    private OverlappingPairCallback? _ghostPairCallback;
    private AlignedBroadphasePairArray? _overlappingPairArray;

    protected internal OverlappingPairCache()
        : base(ConstructionInfo.Null)
    {
    }

    public bool HasDeferredRemoval => btOverlappingPairCache_hasDeferredRemoval(Native);

    public int NumOverlappingPairs => btOverlappingPairCache_getNumOverlappingPairs(Native);

    public AlignedBroadphasePairArray OverlappingPairArray
    {
        get
        {
            IntPtr pairArrayPtr = btOverlappingPairCache_getOverlappingPairArray(Native);
            if (_overlappingPairArray == null || _overlappingPairArray.Native != pairArrayPtr)
            {
                _overlappingPairArray = new AlignedBroadphasePairArray(pairArrayPtr);
            }

            return _overlappingPairArray;
        }
    }

    public void CleanOverlappingPair(BroadphasePair pair, Dispatcher dispatcher)
        => btOverlappingPairCache_cleanOverlappingPair(Native, pair.Native, dispatcher.Native);

    public void CleanProxyFromPairs(BroadphaseProxy proxy, Dispatcher dispatcher)
        => btOverlappingPairCache_cleanProxyFromPairs(Native, proxy.Native, dispatcher.Native);

    public BroadphasePair FindPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => new BroadphasePair(btOverlappingPairCache_findPair(Native, proxy0.Native, proxy1.Native));

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public void ProcessAllOverlappingPairs(OverlapCallback __unnamed0, Dispatcher dispatcher)
        => btOverlappingPairCache_processAllOverlappingPairs(Native, __unnamed0.Native, dispatcher.Native);

    public void ProcessAllOverlappingPairs(OverlapCallback __unnamed0, Dispatcher dispatcher, DispatcherInfo dispatcherInfo)
        => btOverlappingPairCache_processAllOverlappingPairs2(Native, __unnamed0.Native, dispatcher.Native, dispatcherInfo.Native);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

    public void SetInternalGhostPairCallback(OverlappingPairCallback ghostPairCallback)
    {
        _ghostPairCallback = ghostPairCallback;
        btOverlappingPairCache_setInternalGhostPairCallback(Native, ghostPairCallback?.Native ?? IntPtr.Zero);
    }

    public void SetOverlapFilterCallback(OverlapFilterCallback callback)
        => btOverlappingPairCache_setOverlapFilterCallback(Native, callback.Native);

    public void SortOverlappingPairs(Dispatcher dispatcher)
        => btOverlappingPairCache_sortOverlappingPairs(Native, dispatcher.Native);
}

public class HashedOverlappingPairCache : OverlappingPairCache
{
    private OverlapFilterCallback? _overlapFilterCallback;

    public HashedOverlappingPairCache()
    {
        IntPtr native = btHashedOverlappingPairCache_new();
        InitializeUserOwned(native);
    }

    internal HashedOverlappingPairCache(IntPtr native, BulletDisposableObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public int Count => btHashedOverlappingPairCache_GetCount(Native);

    [DisallowNull]
    public OverlapFilterCallback? OverlapFilterCallback
    {
        get => _overlapFilterCallback;
        set
        {
            _overlapFilterCallback = value;
            SetOverlapFilterCallback(value);
        }
    }

    public override BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => new BroadphasePair(btOverlappingPairCallback_addOverlappingPair(Native, proxy0.Native, proxy1.Native));

    public bool NeedsBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => btHashedOverlappingPairCache_needsBroadphaseCollision(Native, proxy0.Native, proxy1.Native);

    public override IntPtr RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPair(Native, proxy0.Native, proxy1.Native, dispatcher.Native);

    public override void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPairsContainingProxy(Native, proxy0.Native, dispatcher.Native);
}

public class SortedOverlappingPairCache : OverlappingPairCache
{
    private OverlapFilterCallback? _overlapFilterCallback;

    public SortedOverlappingPairCache()
    {
        IntPtr native = btSortedOverlappingPairCache_new();
        InitializeUserOwned(native);
    }

    [DisallowNull]
    public OverlapFilterCallback? OverlapFilterCallback
    {
        get => _overlapFilterCallback;
        set
        {
            _overlapFilterCallback = value;
            SetOverlapFilterCallback(value);
        }
    }

    public override BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => new BroadphasePair(btOverlappingPairCallback_addOverlappingPair(Native, proxy0.Native, proxy1.Native));

    public bool NeedsBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => btSortedOverlappingPairCache_needsBroadphaseCollision(Native, proxy0.Native, proxy1.Native);

    public override IntPtr RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPair(Native, proxy0.Native, proxy1.Native, dispatcher.Native);

    public override void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPairsContainingProxy(Native, proxy0.Native, dispatcher.Native);
}

public class NullPairCache : OverlappingPairCache
{
    public NullPairCache()
    {
        IntPtr native = btNullPairCache_new();
        InitializeUserOwned(native);
    }

    public override BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
        => new BroadphasePair(btOverlappingPairCallback_addOverlappingPair(Native, proxy0.Native, proxy1.Native));

    public override IntPtr RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPair(Native, proxy0.Native, proxy1.Native, dispatcher.Native);

    public override void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, Dispatcher dispatcher)
        => btOverlappingPairCallback_removeOverlappingPairsContainingProxy(Native, proxy0.Native, dispatcher.Native);
}
