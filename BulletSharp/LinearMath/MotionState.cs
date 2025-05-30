using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public abstract class MotionState : BulletDisposableObject
{
    private readonly GetWorldTransformUnmanagedDelegate? _getWorldTransform;
    private readonly SetWorldTransformUnmanagedDelegate? _setWorldTransform;

#pragma warning disable IDE0060 // Remove unused parameter
    internal MotionState(ConstructionInfo? info)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }

    protected MotionState()
    {
        _getWorldTransform = new GetWorldTransformUnmanagedDelegate(GetWorldTransformUnmanaged);
        _setWorldTransform = new SetWorldTransformUnmanagedDelegate(SetWorldTransformUnmanaged);

        IntPtr native = btMotionStateWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_getWorldTransform),
            Marshal.GetFunctionPointerForDelegate(_setWorldTransform));
        InitializeUserOwned(native);
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void GetWorldTransformUnmanagedDelegate(out Matrix4x4 worldTrans);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void SetWorldTransformUnmanagedDelegate(ref Matrix4x4 worldTrans);

    public Matrix4x4 WorldTransform
    {
        get
        {
            Matrix4x4 transform;
            GetWorldTransform(out transform);
            return transform;
        }
        set => SetWorldTransform(ref value);
    }

    public abstract void GetWorldTransform(out Matrix4x4 worldTrans);

    public abstract void SetWorldTransform(ref Matrix4x4 worldTrans);

    protected override void Dispose(bool disposing)
        => btMotionState_delete(Native);

    private void GetWorldTransformUnmanaged(out Matrix4x4 worldTrans)
        => GetWorldTransform(out worldTrans);

    private void SetWorldTransformUnmanaged(ref Matrix4x4 worldTrans)
        => SetWorldTransform(ref worldTrans);
}
