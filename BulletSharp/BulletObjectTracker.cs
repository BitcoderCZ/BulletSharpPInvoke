using System.Collections.Generic;
using System.Linq;
#if !BULLET_OBJECT_TRACKING
using System.Diagnostics;
#endif

namespace BulletSharp;

public sealed class BulletObjectTracker
{
    private readonly object _userOwnedObjectsLock = new object();

    private BulletObjectTracker()
    {
    }

    private HashSet<BulletObject> UserOwnedObjects { get; set; } = [];

    public IList<BulletObject> GetUserOwnedObjects()
    {
        lock (_userOwnedObjectsLock)
        {
            return UserOwnedObjects.ToList();
        }
    }

#if BULLET_OBJECT_TRACKING
		public static BulletObjectTracker Current { get; } = new BulletObjectTracker();

		internal static void Add(BulletDisposableObject obj)
		{
			Current.AddRef(obj);
		}

		internal static void Remove(BulletDisposableObject obj)
		{
			Current.RemoveRef(obj);
		}

		private void AddRef(BulletDisposableObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (obj.Owner == null)
			{
				lock (_userOwnedObjectsLock)
				{
					if (_userOwnedObjects.Contains(obj))
					{
						throw new Exception("Adding an object that is already being tracked. " +
							"Object info: " + obj.GetType());
					}
					_userOwnedObjects.Add(obj);
				}
			}
		}

		private void RemoveRef(BulletDisposableObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (obj.Owner == null)
			{
				lock (_userOwnedObjectsLock)
				{
					if (_userOwnedObjects.Contains(obj) == false)
					{
						throw new Exception("Removing object that is not being tracked. " +
							"Object info: " + obj.GetType());
					}
					_userOwnedObjects.Remove(obj);
				}
			}
		}
#else
#pragma warning disable SA1201 // Elements should appear in the correct order
    public static BulletObjectTracker Current { get; } = null;
#pragma warning restore SA1201 // Elements should appear in the correct order

    [Conditional("BULLET_OBJECT_TRACKING")]
    internal static void Add(BulletDisposableObject obj)
    {
    }

    [Conditional("BULLET_OBJECT_TRACKING")]
    internal static void Remove(BulletDisposableObject obj)
    {
    }
#endif
}
