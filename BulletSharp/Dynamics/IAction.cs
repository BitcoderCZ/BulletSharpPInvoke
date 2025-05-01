using System;
using System.Runtime.InteropServices;
using System.Security;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public interface IAction
{
    void DebugDraw(DebugDraw debugDrawer);

    void UpdateAction(CollisionWorld collisionWorld, float deltaTimeStep);
}

internal class ActionInterfaceWrapper : BulletDisposableObject
{
    private readonly IAction _actionInterface;
    private readonly DynamicsWorld _world;

    private readonly DebugDrawUnmanagedDelegate _debugDraw;
    private readonly UpdateActionUnmanagedDelegate _updateAction;

    public ActionInterfaceWrapper(IAction actionInterface, DynamicsWorld world)
    {
        _debugDraw = new DebugDrawUnmanagedDelegate(DebugDrawUnmanaged);
        _updateAction = new UpdateActionUnmanagedDelegate(UpdateActionUnmanaged);

        IntPtr native = btActionInterfaceWrapper_new(
            Marshal.GetFunctionPointerForDelegate(_debugDraw),
            Marshal.GetFunctionPointerForDelegate(_updateAction));
        InitializeUserOwned(native);

        _actionInterface = actionInterface;
        _world = world;
    }

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void DebugDrawUnmanagedDelegate(IntPtr debugDrawer);

    [UnmanagedFunctionPointer(BulletSharp.Native.Conv)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void UpdateActionUnmanagedDelegate(IntPtr collisionWorld, float deltaTimeStep);

    protected override void Dispose(bool disposing)
        => btActionInterface_delete(Native);

    private void DebugDrawUnmanaged(IntPtr debugDrawer)
    {
        DebugDraw? managed = DebugDraw.GetManaged(debugDrawer);

        System.Diagnostics.Debug.Assert(managed is not null, $"{nameof(managed)} shoudn't be null.");

        _actionInterface.DebugDraw(managed);
    }

    private void UpdateActionUnmanaged(IntPtr collisionWorld, float deltaTimeStep)
        => _actionInterface.UpdateAction(_world, deltaTimeStep);
}
