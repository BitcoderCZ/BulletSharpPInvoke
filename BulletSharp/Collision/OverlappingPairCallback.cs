using System;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public abstract class OverlappingPairCallback : BulletDisposableObject
{
#pragma warning disable IDE0060
    internal OverlappingPairCallback(ConstructionInfo? info)
#pragma warning restore IDE0060 // Odebrat nepoužívaný parametr
    {
    }

    /*
		protected OverlappingPairCallback()
		{
			Native = btOverlappingPairCallbackWrapper_new();
		}
		*/

    public abstract BroadphasePair AddOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1);

    public abstract IntPtr RemoveOverlappingPair(BroadphaseProxy proxy0, BroadphaseProxy proxy1, Dispatcher dispatcher);

    public abstract void RemoveOverlappingPairsContainingProxy(BroadphaseProxy proxy0, Dispatcher dispatcher);

    protected override void Dispose(bool disposing)
    {
        if (IsUserOwned)
        {
            btOverlappingPairCallback_delete(Native);
        }
    }
}
