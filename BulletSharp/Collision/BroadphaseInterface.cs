using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public abstract class BroadphaseAabbCallback : BulletDisposableObject
{
    internal ProcessUnmanagedDelegate _process;

#pragma warning disable IDE0060 // Remove unused parameter
    internal BroadphaseAabbCallback(ConstructionInfo info)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _process = ProcessUnmanaged;
    }

    protected BroadphaseAabbCallback()
    {
        _process = ProcessUnmanaged;
        IntPtr native = btBroadphaseAabbCallbackWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_process));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    internal delegate bool ProcessUnmanagedDelegate(IntPtr proxy);

    public abstract bool Process(BroadphaseProxy proxy);

    protected override void Dispose(bool disposing)
        => btBroadphaseAabbCallback_delete(Native);

    private bool ProcessUnmanaged(IntPtr proxy)
        => Process(BroadphaseProxy.GetManaged(proxy));
}

public abstract class BroadphaseRayCallback : BroadphaseAabbCallback
{
    private UIntArray? _signs;

    protected BroadphaseRayCallback()
        : base(ConstructionInfo.Null)
    {
        IntPtr native = btBroadphaseRayCallbackWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_process));
        InitializeUserOwned(native);
    }

    public float LambdaMax
    {
        get => btBroadphaseRayCallback_getLambda_max(Native);
        set => btBroadphaseRayCallback_setLambda_max(Native, value);
    }

    public Vector3 RayDirectionInverse
    {
        get
        {
            Vector3 value;
            btBroadphaseRayCallback_getRayDirectionInverse(Native, out value);
            return value;
        }
        set => btBroadphaseRayCallback_setRayDirectionInverse(Native, ref value);
    }

    public UIntArray Signs
    {
        get
        {
            if (_signs == null)
            {
                _signs = new UIntArray(btBroadphaseRayCallback_getSigns(Native), 3);
            }

            return _signs;
        }
    }
}

public abstract class BroadphaseInterface : BulletDisposableObject
{
    protected OverlappingPairCache? _overlappingPairCache;

    protected internal BroadphaseInterface()
    {
    }

    public OverlappingPairCache? OverlappingPairCache => _overlappingPairCache;

    public void AabbTestRef(ref Vector3 aabbMin, ref Vector3 aabbMax, BroadphaseAabbCallback callback)
        => btBroadphaseInterface_aabbTest(Native, ref aabbMin, ref aabbMax, callback.Native);

    public void AabbTest(Vector3 aabbMin, Vector3 aabbMax, BroadphaseAabbCallback callback)
        => btBroadphaseInterface_aabbTest(Native, ref aabbMin, ref aabbMax, callback.Native);

    public void CalculateOverlappingPairs(Dispatcher dispatcher)
        => btBroadphaseInterface_calculateOverlappingPairs(Native, dispatcher.Native);

    public abstract BroadphaseProxy CreateProxy(ref Vector3 aabbMin, ref Vector3 aabbMax, int shapeType, IntPtr userPtr, int collisionFilterGroup, int collisionFilterMask, Dispatcher dispatcher);

    public void DestroyProxy(BroadphaseProxy proxy, Dispatcher dispatcher)
        => btBroadphaseInterface_destroyProxy(Native, proxy.Native, dispatcher.Native);

    public void GetAabb(BroadphaseProxy proxy, out Vector3 aabbMin, out Vector3 aabbMax)
        => btBroadphaseInterface_getAabb(Native, proxy.Native, out aabbMin, out aabbMax);

    public void GetBroadphaseAabb(out Vector3 aabbMin, out Vector3 aabbMax)
        => btBroadphaseInterface_getBroadphaseAabb(Native, out aabbMin, out aabbMax);

    public void PrintStats()
        => btBroadphaseInterface_printStats(Native);

    public void RayTestRef(ref Vector3 rayFrom, ref Vector3 rayTo, BroadphaseRayCallback rayCallback)
        => btBroadphaseInterface_rayTest(Native, ref rayFrom, ref rayTo, rayCallback.Native);

    public void RayTest(Vector3 rayFrom, Vector3 rayTo, BroadphaseRayCallback rayCallback)
        => btBroadphaseInterface_rayTest(Native, ref rayFrom, ref rayTo, rayCallback.Native);

    public void RayTestRef(ref Vector3 rayFrom, ref Vector3 rayTo, BroadphaseRayCallback rayCallback, ref Vector3 aabbMin, ref Vector3 aabbMax)
        => btBroadphaseInterface_rayTest3(Native, ref rayFrom, ref rayTo, rayCallback.Native, ref aabbMin, ref aabbMax);

    public void RayTest(Vector3 rayFrom, Vector3 rayTo, BroadphaseRayCallback rayCallback, Vector3 aabbMin, Vector3 aabbMax)
        => btBroadphaseInterface_rayTest3(Native, ref rayFrom, ref rayTo, rayCallback.Native, ref aabbMin, ref aabbMax);

    public void ResetPool(Dispatcher dispatcher)
        => btBroadphaseInterface_resetPool(Native, dispatcher.Native);

    public void SetAabbRef(BroadphaseProxy proxy, ref Vector3 aabbMin, ref Vector3 aabbMax, Dispatcher dispatcher)
        => btBroadphaseInterface_setAabb(Native, proxy.Native, ref aabbMin, ref aabbMax, dispatcher.Native);

    public void SetAabb(BroadphaseProxy proxy, Vector3 aabbMin, Vector3 aabbMax, Dispatcher dispatcher)
        => btBroadphaseInterface_setAabb(Native, proxy.Native, ref aabbMin, ref aabbMax, dispatcher.Native);

    protected internal void InitializeMembers(OverlappingPairCache overlappingPairCache)
        => _overlappingPairCache = overlappingPairCache;

    protected override void Dispose(bool disposing)
        => btBroadphaseInterface_delete(Native);
}
