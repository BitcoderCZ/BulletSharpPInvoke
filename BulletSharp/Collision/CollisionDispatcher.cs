using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public delegate void NearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo);

[Flags]
public enum DispatcherFlags
{
    None = 0,
    StaticStaticReported = 1,
    UseRelativeContactBreakingThreshold = 2,
    DisableContactPoolDynamicAllocation = 4,
}

public class CollisionDispatcher : Dispatcher
{
    protected CollisionConfiguration _collisionConfiguration;
    private NearCallback? _nearCallback;
    private List<CollisionAlgorithmCreateFunc>? _collisionCreateFuncs;
    private NearCallbackUnmanagedDelegate? _nearCallbackUnmanaged;
    private IntPtr _nearCallbackUnmanagedPtr;

    public CollisionDispatcher(CollisionConfiguration collisionConfiguration)
    {
        IntPtr native = btCollisionDispatcher_new(collisionConfiguration.Native);
        InitializeUserOwned(native);

        _collisionConfiguration = collisionConfiguration;
    }

    protected internal CollisionDispatcher()
    {
        _collisionConfiguration = null!;
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void NearCallbackUnmanagedDelegate(IntPtr collisionPair, IntPtr dispatcher, IntPtr dispatchInfo);

    public CollisionConfiguration CollisionConfiguration
    {
        get => _collisionConfiguration;
        set
        {
            btCollisionDispatcher_setCollisionConfiguration(Native, value.Native);
            _collisionConfiguration = value;
        }
    }

    public DispatcherFlags DispatcherFlags
    {
        get => btCollisionDispatcher_getDispatcherFlags(Native);
        set => btCollisionDispatcher_setDispatcherFlags(Native, value);
    }

    public NearCallback? NearCallback
    {
        get => _nearCallback;
        set
        {
            _nearCallback = value;

            if (value == null)
            {
                btCollisionDispatcher_setNearCallback(Native, IntPtr.Zero);
                _nearCallbackUnmanaged = null;
                return;
            }

            if (_nearCallbackUnmanaged == null)
            {
                _nearCallbackUnmanaged = new NearCallbackUnmanagedDelegate(NearCallbackUnmanaged);
                _nearCallbackUnmanagedPtr = Marshal.GetFunctionPointerForDelegate(_nearCallbackUnmanaged);
            }

            btCollisionDispatcher_setNearCallback(Native, _nearCallbackUnmanagedPtr);
        }
    }

    public static void DefaultNearCallback(BroadphasePair collisionPair, CollisionDispatcher dispatcher, DispatcherInfo dispatchInfo)
        => btCollisionDispatcher_defaultNearCallback(collisionPair.Native, dispatcher.Native, dispatchInfo.Native);

    public void RegisterCollisionCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1, CollisionAlgorithmCreateFunc createFunc)
    {
        if (_collisionCreateFuncs == null)
        {
            _collisionCreateFuncs = [];
        }

        _collisionCreateFuncs.Add(createFunc);

        btCollisionDispatcher_registerCollisionCreateFunc(Native, proxyType0, proxyType1, createFunc.Native);
    }

    public void RegisterClosestPointsCreateFunc(BroadphaseNativeType proxyType0, BroadphaseNativeType proxyType1, CollisionAlgorithmCreateFunc createFunc)
    {
        if (_collisionCreateFuncs == null)
        {
            _collisionCreateFuncs = [];
        }

        _collisionCreateFuncs.Add(createFunc);

        btCollisionDispatcher_registerClosestPointsCreateFunc(Native, proxyType0, proxyType1, createFunc.Native);
    }

    private void NearCallbackUnmanaged(IntPtr collisionPair, IntPtr dispatcher, IntPtr dispatchInfo)
    {
        System.Diagnostics.Debug.Assert(dispatcher == Native);
        System.Diagnostics.Debug.Assert(_nearCallback is not null);

        _nearCallback(new BroadphasePair(collisionPair), this, new DispatcherInfo(dispatchInfo));
    }
}
