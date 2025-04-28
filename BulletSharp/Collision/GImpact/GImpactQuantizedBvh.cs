using System;
using System.Numerics;
using static BulletSharp.UnsafeNativeMethods;

namespace BulletSharp;

public class GImpactQuantizedBvhNode : BulletDisposableObject
{
    public GImpactQuantizedBvhNode()
    {
        IntPtr native = BT_QUANTIZED_BVH_NODE_new();
        InitializeUserOwned(native);
    }

    internal GImpactQuantizedBvhNode(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public int DataIndex
    {
        get => BT_QUANTIZED_BVH_NODE_getDataIndex(Native);
        set => BT_QUANTIZED_BVH_NODE_setDataIndex(Native, value);
    }

    public int EscapeIndex
    {
        get => BT_QUANTIZED_BVH_NODE_getEscapeIndex(Native);
        set => BT_QUANTIZED_BVH_NODE_setEscapeIndex(Native, value);
    }

    public int EscapeIndexOrDataIndex
    {
        get => BT_QUANTIZED_BVH_NODE_getEscapeIndexOrDataIndex(Native);
        set => BT_QUANTIZED_BVH_NODE_setEscapeIndexOrDataIndex(Native, value);
    }

    public bool IsLeafNode => BT_QUANTIZED_BVH_NODE_isLeafNode(Native);
    /*
		public UShortArray QuantizedAabbMax
		{
			get => BT_QUANTIZED_BVH_NODE_getQuantizedAabbMax(Native);
		}

		public UShortArray QuantizedAabbMin
		{
			get => BT_QUANTIZED_BVH_NODE_getQuantizedAabbMin(Native);
		}
		*/

    public bool TestQuantizedBoxOverlapp(ushort[] quantizedMin, ushort[] quantizedMax)
        => BT_QUANTIZED_BVH_NODE_testQuantizedBoxOverlapp(Native, quantizedMin, quantizedMax);

    protected override void Dispose(bool disposing)
        => BT_QUANTIZED_BVH_NODE_delete(Native);
}

public class GImpactQuantizedBvhNodeArray : BulletObject
{
    internal GImpactQuantizedBvhNodeArray(IntPtr native)
    {
        Initialize(native);
    }

    /*
		public GimGImpactQuantizedBvhNodeArray()
		{
			Native = GIM_QUANTIZED_BVH_NODE_ARRAY_new();
		}
		*/
}

public class QuantizedBvhTree : BulletDisposableObject
{
    public QuantizedBvhTree()
    {
        IntPtr native = btQuantizedBvhTree_new();
        InitializeUserOwned(native);
    }

    internal QuantizedBvhTree(IntPtr native, BulletObject owner)
    {
        InitializeSubObject(native, owner);
    }

    public int NodeCount => btQuantizedBvhTree_getNodeCount(Native);

    public void BuildTree(GimBvhDataArray primitiveBoxes)
        => btQuantizedBvhTree_build_tree(Native, primitiveBoxes.Native);

    public void ClearNodes()
        => btQuantizedBvhTree_clearNodes(Native);

    /*
		public GImpactQuantizedBvhNode GetNodePointer(int index = 0)
		{
			return btQuantizedBvhTree_get_node_pointer(Native, index);
		}
		*/

    public int GetEscapeNodeIndex(int nodeIndex)
        => btQuantizedBvhTree_getEscapeNodeIndex(Native, nodeIndex);

    public int GetLeftNode(int nodeIndex)
        => btQuantizedBvhTree_getLeftNode(Native, nodeIndex);

    public void GetNodeBound(int nodeIndex, Aabb bound)
        => btQuantizedBvhTree_getNodeBound(Native, nodeIndex, bound.Native);

    public int GetNodeData(int nodeIndex)
        => btQuantizedBvhTree_getNodeData(Native, nodeIndex);

    public int GetRightNode(int nodeIndex)
        => btQuantizedBvhTree_getRightNode(Native, nodeIndex);

    public bool IsLeafNode(int nodeIndex)
        => btQuantizedBvhTree_isLeafNode(Native, nodeIndex);

    public void QuantizePoint(ushort[] quantizedpoint, Vector3 point)
        => btQuantizedBvhTree_quantizePoint(Native, quantizedpoint, ref point);

    public void SetNodeBound(int nodeIndex, Aabb bound)
        => btQuantizedBvhTree_setNodeBound(Native, nodeIndex, bound.Native);

    public bool TestQuantizedBoxOverlap(int nodeIndex, ushort[] quantizedMin, ushort[] quantizedMax)
        => btQuantizedBvhTree_testQuantizedBoxOverlapp(Native, nodeIndex, quantizedMin, quantizedMax);

    protected override void Dispose(bool disposing) => btQuantizedBvhTree_delete(Native);
}

public class GImpactQuantizedBvh : BulletDisposableObject
{
    private Aabb _globalBox;

    private PrimitiveManagerBase _primitiveManager;

    public GImpactQuantizedBvh()
    {
        IntPtr native = btGImpactQuantizedBvh_new();
        InitializeUserOwned(native);
    }

    public GImpactQuantizedBvh(PrimitiveManagerBase primitiveManager)
    {
        IntPtr native = btGImpactQuantizedBvh_new2(primitiveManager.Native);
        InitializeUserOwned(native);
        _primitiveManager = primitiveManager;
    }

    public Aabb GlobalBox => _globalBox ?? (_globalBox = new Aabb(btGImpactQuantizedBvh_getGlobalBox(Native), this));

    public bool HasHierarchy => btGImpactQuantizedBvh_hasHierarchy(Native);

    public bool IsTrimesh => btGImpactQuantizedBvh_isTrimesh(Native);

    public int NodeCount => btGImpactQuantizedBvh_getNodeCount(Native);

    public PrimitiveManagerBase PrimitiveManager
    {
        get => _primitiveManager;
        set
        {
            btGImpactQuantizedBvh_setPrimitiveManager(Native, value.Native);
            _primitiveManager = value;
        }
    }

    /*
		public bool BoxQuery(Aabb box, AlignedIntArray collidedResults)
		{
			return btGImpactQuantizedBvh_boxQuery(Native, box.Native, collidedResults.Native);
		}

		public bool BoxQueryTrans(Aabb box, Matrix transform, AlignedIntArray collidedResults)
		{
			return btGImpactQuantizedBvh_boxQueryTrans(Native, box.Native, ref transform,
				collidedResults.Native);
		}
		*/

    public static void FindCollision(GImpactQuantizedBvh boxset1, Matrix4x4 trans1, GImpactQuantizedBvh boxset2, Matrix4x4 trans2, PairSet collisionPairs)
        => btGImpactQuantizedBvh_find_collision(boxset1.Native, ref trans1, boxset2.Native, ref trans2, collisionPairs.Native);

    public void BuildSet()
        => btGImpactQuantizedBvh_buildSet(Native);

    /*
		public GImpactQuantizedBvhNode GetNodePointer(int index = 0)
		{
			return btGImpactQuantizedBvh_get_node_pointer(Native, index);
		}
		*/
    public int GetEscapeNodeIndex(int nodeIndex)
        => btGImpactQuantizedBvh_getEscapeNodeIndex(Native, nodeIndex);

    public int GetLeftNode(int nodeIndex)
        => btGImpactQuantizedBvh_getLeftNode(Native, nodeIndex);

    public void GetNodeBound(int nodeIndex, Aabb bound)
        => btGImpactQuantizedBvh_getNodeBound(Native, nodeIndex, bound.Native);

    public int GetNodeData(int nodeIndex)
        => btGImpactQuantizedBvh_getNodeData(Native, nodeIndex);

    public void GetNodeTriangle(int nodeIndex, PrimitiveTriangle triangle)
        => btGImpactQuantizedBvh_getNodeTriangle(Native, nodeIndex, triangle.Native);

    public int GetRightNode(int nodeIndex)
        => btGImpactQuantizedBvh_getRightNode(Native, nodeIndex);

    public bool IsLeafNode(int nodeIndex)
        => btGImpactQuantizedBvh_isLeafNode(Native, nodeIndex);

    /*
		public bool RayQuery(Vector3 rayDir, Vector3 rayOrigin, AlignedIntArray collidedResults)
		{
			return btGImpactQuantizedBvh_rayQuery(Native, ref rayDir, ref rayOrigin,
				collidedResults.Native);
		}
		*/

    public void SetNodeBound(int nodeIndex, Aabb bound)
        => btGImpactQuantizedBvh_setNodeBound(Native, nodeIndex, bound.Native);

    public void Update()
        => btGImpactQuantizedBvh_update(Native);

    protected override void Dispose(bool disposing) => btGImpactQuantizedBvh_delete(Native);
}
